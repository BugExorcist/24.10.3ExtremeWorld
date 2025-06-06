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
        private Creature Owner;
        public SkillDefine Define;

        public float CD;

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Info = info;
            this.Owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.Owner.Define.Class][this.Info.Id];
        }

        public SkillResult CanCast()
        {
            if (this.Define.CastTarget == TargetType.Target && BattleManager.Instance.Target == null)
            {
                return SkillResult.InvalidTarget;
            }
            if (this.Define.CastTarget == TargetType.Position && BattleManager.Instance.Position == Vector3.negativeInfinity)
            {
                return SkillResult.InvalidTarget;
            }
            if (this.Define.MPCost > this.Owner.Attributes.MP)
            {
                return SkillResult.OutOfMP;
            }
            if (this.CD > 0)
            {
                return SkillResult.Cooldown;
            }
            return SkillResult.OK;
        }

        internal void Cast()
        {
            throw new NotImplementedException();
        }
    }
}
