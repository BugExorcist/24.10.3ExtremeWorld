using Common.Data;
using Entities;
using SkillBridge.Message;
using Common.Battle;
using Managers;
using UnityEngine;
using System;
using System.Collections.Generic;
using Common;

namespace Batttle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public Creature Target;
        public SkillDefine Define;
        private NDamageInfo Damage;

        private float cd = 0;
        private float castTime = 0;
        // 技能释放后经过的时间
        private float skillTime = 0;
        private bool IsCasting = false;
        private int Hit = 0;
        private SkillStatus Status;

        Dictionary<int, List<NDamageInfo>> HitMap = new Dictionary<int, List<NDamageInfo>>();

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

        public void BeginCast(NDamageInfo damage)
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.skillTime = 0;
            this.Hit = 0;
            this.cd = this.Define.CD;
            this.Damage = damage;
            this.Owner.PlayAnim(this.Define.SkillAnim);

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
        /// 根据配置的时机，对多次技能命中处理伤害
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
                    {
                        this.DoHit();
                    }
                }
                else
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
            List<NDamageInfo> damages;
            if (this.HitMap.TryGetValue(this.Hit, out damages))
            {
                DoHitDamages(damages);
            }
            this.Hit++;
        }

        private void UpdateCD(float delta)
        {
            if (this.cd > 0)
                this.cd -= delta;
            if (this.cd < 0)
                this.cd = 0;
        }

        /// <summary>
        /// 处理服务器发送的技能命中信息
        /// </summary>
        internal void DoHit(int hitId, List<NDamageInfo> damages)
        {
            if (hitId <= this.Hit)
            {   //如果服务器发送的技能命中次数小于本地命中次数，说明服务器的速度快于本地，本地还没播放本次hit，把命中的伤害缓存起来
                this.HitMap[hitId] = damages;
            }
            else
            {   //如果服务器发送的技能命中次数大于本地命中次数，说明本地已经播放了本次hit，把命中的伤害直接处理
                DoHitDamages(damages);
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
