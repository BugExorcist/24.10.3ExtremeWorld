using Common.Data;
using Entities;
using SkillBridge.Message;
using Common.Battle;
using Managers;
using UnityEngine;
using System;
using System.Collections.Generic;
using Common;

namespace Battle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public Creature Target;
        public SkillDefine Define;

        private float cd = 0;
        private float castTime = 0;
        // 技能释放后经过的时间
        private float skillTime = 0;
        private bool IsCasting = false;
        public int Hit = 0;
        private SkillStatus Status;
        //伤害缓存，如果伤害服务器发得早，则缓存
        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

        List<Bullet> Bullets = new List<Bullet>();

        public float CD
        {
            get { return cd; }
        }


        public Skill(NSkillInfo info, Creature owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
            this.cd = 0;
        }

        public SkillResult CanCast(Creature target)
        {
            if (this.Define.CastTarget == TargetType.Target)
            {
                if (target == null || target == this.Owner)
                    return SkillResult.InvalidTarget;

                int distance = this.Owner.Distance(target);
                if (distance > this.Define.CastRange)
                    return SkillResult.OutOfRange;
            }
            if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            {
                return SkillResult.InvalidTarget;
            }
            if (this.Define.MPCost > this.Owner.Attributes.MP)
            {
                return SkillResult.OutOfMp;
            }
            if (this.cd > 0)
            {
                return SkillResult.CoolDown;
            }
            return SkillResult.Ok;
        }

        public void BeginCast(Creature target)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.skillTime = 0;
            this.Hit = 0;
            this.cd = this.Define.CD;
            this.Target = target;
            this.Owner.PlayAnim(this.Define.SkillAnim);
            this.Bullets.Clear();
            this.HitMap.Clear();

            if (this.Define.CastTime > 0)
            {
                this.Status = SkillStatus.Casting;
            }
            else
            {
                this.Status = SkillStatus.Running;
            }

        }


        public void OnUpdate(float delta)
        {
            UpdateCD(delta);
            
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
            if (this.castTime < this.Define.CastTime)
            {
                this.castTime += Time.deltaTime;
            }
            else
            {
                this.castTime = 0;
                this.Status = SkillStatus.Running;
                Debug.LogFormat("Skill[{0}].UpdateCasting Finish", this.Define.Name);
            }
        }

        /// <summary>
        /// 根据配置，处理多次伤害技能的hit更新时机
        /// </summary>
        private void UpdateSkill()
        {
            this.skillTime += Time.deltaTime;
            if (this.Define.Duration > 0)
            {   //持续技能
                if (this.skillTime > this.Define.Interval * (this.Hit + 1))
                {
                    this.DoHit();
                }
                if (this.skillTime >= this.Define.Duration)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }
            else if (this.Define.HitTimes != null && this.Define.HitTimes.Count > 0)
            {   //次数技能
                if (this.Hit < this.Define.HitTimes.Count)
                {
                    if (this.skillTime >= this.Define.HitTimes[Hit])
                    {   //如果是子弹技能 这里就是产生子弹的时机
                        this.DoHit();
                    }
                }
                else
                {   //非子弹技能 结束时机
                    if (!this.Define.Bullet)
                    {
                        this.Status = SkillStatus.None;
                        this.IsCasting = false;
                        Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                    }
                }
            }
            if (this.Define.Bullet)
            {   //子弹技能 结束时机
                bool finish = true;
                foreach (Bullet bullet in this.Bullets)
                {
                    bullet.Update();
                    if (!bullet.Stoped) finish = false;
                }

                if (finish && this.Hit >= this.Define.HitTimes.Count)
                {
                    this.Status = SkillStatus.None;
                    this.IsCasting = false;
                    Debug.LogFormat("Skill[{0}].UpdateSkill Finish", this.Define.Name);
                }
            }

        }

        /// <summary>
        /// 更新技能的Hit次数
        /// </summary>
        private void DoHit()
        {
            if (this.Define.Bullet)
            {
                this.CastBullet();
            }
            else
            {
                this.DoHitDamages(this.Hit);
            }
            this.Hit++;
        }

        private void CastBullet()
        {
            Bullet bullet = new Bullet(this);
            Debug.LogFormat("Skill[{0}].CastBullet[{1}] Target:{2}", this.Define.Name, this.Define.Bullet, this.Target); ;
            this.Bullets.Add(bullet);
        }

        /// <summary>
        /// 根据本地的hit数处理缓存中的伤害
        /// </summary>
        public void DoHitDamages(int hit)
        {
            List<NDamageInfo> damages;
            if (this.HitMap.TryGetValue(hit, out damages))
            {
                DoHitDamages(damages);
            }
        }

        private void UpdateCD(float delta)
        {
            if (this.cd > 0)
                this.cd -= delta;
            if (this.cd < 0)
                this.cd = 0;
        }

        public void DoHit(NSkillHitInfo hitInfo)
        {
            if (hitInfo.isBullet || !this.Define.Bullet)
            {   //如果是子弹伤害 或者不是子弹技能
                this.DoHit(hitInfo.hitId, hitInfo.Damages);
            }

            //如果是子弹技能但标记为子弹伤害，则只表示发射子弹，还没命中
            //（实际上服务器在创建子弹没有伤害信息的时候不会发送子弹技能的NSkillHitInfo）

        }

        /// <summary>
        /// 处理服务器发送的技能命中信息
        /// </summary>
        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId >= this.Hit)
            {   //如果服务器发送的Hit数大于本地命中次数，说明服务器的速度快于本地，本地还没播放本次hit，把命中的伤害缓存起来
                this.HitMap[hitId] = damages;
            }
            else
            {   //如果服务器发送的技能命中次数小于本地命中次数，说明本地已经播放了本次hit，把命中的伤害直接处理
                DoHitDamages(damages);

                /*
                 * TODO：服务器在结算伤害后才发送NSkillHitInfo
                 * 本地到了发送子弹的时机Hit就会自增
                 * 导致理想状态下 服务器的发送的Hit数永远比客户端本地Hit小
                 * 客户端收到NSkillHitInfo会直接处理伤害逻辑，不论子弹是否真的命中了目标
                 */
            }
        }

        private void DoHitDamages(List<NDamageInfo> damages)
        {
            foreach (var damage in damages)
            {
                Creature target = EntityManager.Instance.GetEntity(damage.entityId) as Creature;
                if (target != null)
                {
                    target.DoDamage(damage);
                }
            }
        }
    }
}
