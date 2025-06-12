using Common;
using Common.Battle;
using Common.Data;
using Common.Utils;
using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{

    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;
        public SkillStatus Status;

        private float cd;
        public float CD
        {
            get { return cd; }
        }

        public bool Instant
        {
            get
            {
                if (this.Define.CastTime > 0) return false;
                if (this.Define.Bullet) return false;
                if (this.Define.Duration > 0) return false;
                if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0) return false;
                return true;
            }
        }

        private float castingTime = 0;
        private float skillTime = 0;
        private int Hit = 0;
        BattleContext Context;
        List<Bullet> Bullets = new List<Bullet>();

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }

        internal SkillResult CanCast(BattleContext context)
        {
            if (this.Status != SkillStatus.None)
            {
                return SkillResult.Casting;
            }
            if (this.Define.CastTarget == TargetType.Target)
            {
                if (context.Target == null || context.Target == this.Owner)
                {
                    return SkillResult.InvalidTarget;
                }
                int distance = this.Owner.Distance(context.Target);
                if (distance > this.Define.CastRange)
                {
                    return SkillResult.OutOfRange;
                }
            }

            if (this.Define.CastTarget == TargetType.Position)
            {
                if (context.CastSkill.Position == null)
                    return SkillResult.InvalidTarget;
                if (this.Owner.Distance(context.Position) > this.Define.CastRange)
                    return SkillResult.OutOfRange;
            }

            if (this.Owner.Attributes.MP < this.Define.MPCost)
            {
                return SkillResult.OutOfMp;
            }

            if (this.cd > 0)
            { 
                return SkillResult.CoolDown;
            }

            return SkillResult.Ok;
        }

        internal SkillResult Cast(BattleContext context)
        {
            SkillResult result = this.CanCast(context);
            if (result == SkillResult.Ok)
            {
                this.Hit = 0;
                this.skillTime = 0;
                this.castingTime = 0;
                this.cd = this.Define.CD;
                this.Context = context;

                if (this.Instant)
                {
                    this.DoHit();
                }
                else
                {
                    if (this.Define.CastTime > 0)
                    {
                        this.Status = SkillStatus.Casting;
                    }
                    else
                    {
                        this.Status = SkillStatus.Running;
                    }
                }
                this.cd = this.Define.CD;
            }
            Log.InfoFormat("Skill[{0}].Cast: :result:{1} statues:{2}", this.Define.Name, result, this.Status);
            return result;
        }

        internal void Update()
        {
            UpdateCD();
            if (this.Status == SkillStatus.Casting)
            {
                this.UpdateCasting();
            }
            else if (this.Status == SkillStatus.Running)
            {
                this.UpdateSkill();
            }
        }


        private void UpdateCasting()
        {
            if (this.castingTime < this.Define.CastTime)
            {
                this.castingTime += Time.deltaTime;
            }
            else
            {
                this.castingTime = 0;
                this.Status = SkillStatus.Running;
                Log.InfoFormat("Skill[{0}].UpdateCasting Finish", this.Define.Name);
            }
        }

        private void UpdateCD()
        {
            if (this.cd > 0)
            {
                this.cd -= Time.deltaTime;
            }
            if (cd < 0)
                this.cd = 0;
        }

        private void UpdateSkill()
        {
            this.skillTime += Time.deltaTime;

            if (this.Define.Duration > 0)
            {   //持续技能
                if (this.skillTime > this.Define.Interval * (this.Hit + 1))
                {
                    this.DoHit();
                }

                if (this.skillTime >= Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    Log.InfoFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }
            else if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
            {   //次数技能
                if (this.Hit < this.Define.HitTimes.Count)
                {
                    if (this.skillTime > this.Define.HitTimes[this.Hit])
                    {
                        this.DoHit();
                    }
                }
                else
                {
                    if (!this.Define.Bullet)
                    {
                        this.Status = SkillStatus.None;
                        Log.InfoFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                    }
                    
                }
            }
        }

        NSkillHitInfo InitHitInfo(bool isBullet)
        {
            NSkillHitInfo hitInfo = new NSkillHitInfo();
            hitInfo.casterId = this.Context.Caster.entityId;
            hitInfo.skillId = this.Info.Id;
            hitInfo.hitId = this.Hit;
            hitInfo.isBullet = isBullet;
            return hitInfo;
        }

        public void DoHit()
        {
            NSkillHitInfo hitInfo = this.InitHitInfo(false);
            Log.InfoFormat("Skill[{0}].DoHit[{1}]", this.Define.Name, this.Hit);
            this.Hit++;
            if (this.Define.Bullet)
            {   //如果是子弹第一次DoHit，不造成伤害，子弹内部会调用有hitInfo的DoHit
                CastBullet(hitInfo);
                return;
            }
            DoHit(hitInfo);
        }

        public void DoHit(NSkillHitInfo hitInfo)
        { 
            if (this.Define.AOERange > 0)
            {
                this.HitRange();
                return;
            }

            if (this.Define.CastTarget == TargetType.Target)
            {
                this.HitTarget(Context.Target);
            }
        }

        private void HitTarget(Creature target)
        {
            if (this.Define.CastTarget == TargetType.Self && (target != Context.Caster)) return;
            else if (target == Context.Caster) return;

            NDamageInfo damage = this.CalcSkillDamage(Context.Caster, target);
            Log.InfoFormat("Skill[{0}].HitTarget[{1}] Damage:{2} Crit:{3}", this.Define.Name, target.Name, damage.Damage, damage.Crit);
            target.DoDamage(damage);
            this.HitInfo.Damages.Add(damage);
        }

        /// <summary>
        /// 计算技能对目标造成的伤害
        /// </summary>
        /// <remarks>
        /// 伤害计算公式：
        /// <para>物理伤害 = (技能基础伤害 + 角色AD * 物理攻击系数) * (1 - 目标物理防御/(物理防御 + 100))</para>
        /// <para>法术伤害 = (技能基础伤害 + 角色AP * 魔法攻击系数) * (1 - 目标魔法防御/(魔法防御 + 100))</para>
        /// <para>暴击伤害 = 最终伤害 * 2 (100%暴击伤害加成)</para>
        /// <para>伤害规则：</para>
        /// <list type="tips">
        /// <item><description>最小伤害值为1</description></item>
        /// <item><description>最终伤害会有±5%的随机浮动 (最终伤害 * [0.95, 1.05])</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caster">施法者实体</param>
        /// <param name="target">目标实体</param>
        /// <returns>包含伤害信息的NDamageInfo对象</returns>
        private NDamageInfo CalcSkillDamage(Creature caster, Creature target)
        {
            float ad = this.Define.AD + caster.Attributes.AD * this.Define.ADFactor;
            float ap = this.Define.AP + caster.Attributes.AP * this.Define.APFactor;

            float addmg = ad * (1 -  target.Attributes.DEF / (target.Attributes.DEF + 100));
            float apdmg = ap * (1 - target.Attributes.MDEF / (target.Attributes.MDEF + 100));

            float finalDmg = addmg + apdmg;
            bool isCrit = IsCrit(caster.Attributes.CRI);
            if (isCrit)
                finalDmg *= 2f;

            //随机浮动
            finalDmg = finalDmg * (0.95f + (float)MathUtil.Random.NextDouble() * 0.1f);

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = target.entityId;
            damage.Damage = Math.Max(1, (int)finalDmg);
            damage.Crit = isCrit;
            return damage;
        }

        private bool IsCrit(float cirt)
        {
            return MathUtil.Random.NextDouble() < cirt;
        }

        private void CastBullet(NSkillHitInfo hitInfo)
        {
            Log.InfoFormat("Skill[{0}].CastBullet[{1}]", this.Define.Name, this.Define.BulletResource);

            Bullet bullet = new Bullet(this, this.Context.Target, hitInfo);
            this.Bullets.Add(bullet);
        }

        private void HitRange()
        {
            Vector3Int pos;
            if (this.Define.CastTarget == Common.Battle.TargetType.Target)
            {
                pos = Context.Target.Position;
            }
            else if (this.Define.CastTarget == Common.Battle.TargetType.Position)
            {
                pos = Context.Position;
            }
            else
            {
                pos = this.Owner.Position;
            }

            List<Creature> units = this.Context.Battle.FindUnitsInRange(pos, (int)this.Define.AOERange);
            foreach(var target in units)
            {
                this.HitTarget(target);
            }
        }
    }
}
