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
        private float castTime = 0;
        private bool IsCasting = false;

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
            //if (this.Define.CastTarget == TargetType.Target)
            //{
            //    if (target == null || target == this.Owner)
            //        return SkillResult.InvalidTarget;
            //}
            //if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.CurrentPosition == null)
            //{
            //    return SkillResult.InvalidTarget;
            //}
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

        public void BeginCast()
        {
            this.IsCasting = true;
            this.castTime = 0;
            this.cd = this.Define.CD;
            this.Owner.PlayAnim(this.Define.SkillAnim);
        }

        public void Cast()
        {
            
        }

        public void OnUpdate(float delta)
        {
            if (this.IsCasting)
            {

            }
            UpdateCD(delta);
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
