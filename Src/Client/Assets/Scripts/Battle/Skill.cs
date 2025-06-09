using Common.Data;
using Entities;
using SkillBridge.Message;
using Common.Battle;
using Managers;
using UnityEngine;
using System;

namespace Batttle
{
    public class Skill
    {
        public NSkillInfo Info;
        public Creature Owner;
        public SkillDefine Define;

        private float cd;
        private NDamageInfo Damage;
        private float castTime = 0;
        // 技能释放后经过的时间
        private float skillTime;
        private bool IsCasting = false;
        private int hit;

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

                int distance = (int)Vector3Int.Distance(this.Owner.position, target.position);
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
            this.hit = 0;
            this.cd = this.Define.CD;
            this.Damage = damage;
            this.Owner.PlayAnim(this.Define.SkillAnim);
        }


        public void OnUpdate(float delta)
        {
            if (this.IsCasting)
            {
                this.skillTime += delta;
                if (this.skillTime > 0.5f && this.hit == 0)
                {
                    this.DoHit();
                }
                if (this.skillTime >= this.Define.CD)
                {
                    this.skillTime = 0;
                }
            }
            UpdateCD(delta);
        }

        private void DoHit()
        {
            if (this.Damage != null)
            {
                var cha = CharacterManager.Instance.GetCharacter(this.Damage.entityId);
                cha.DoDamage(this.Damage);
            }
            this.hit++;
        }

        private void UpdateCD(float delta)
        {
            if (this.cd > 0)
                this.cd -= delta;
            if (this.cd < 0)
                this.cd = 0;
        }
    }
}
