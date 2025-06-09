using Common.Battle;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Battle
{

    public class Skill
    {
        public NSkillInfo Info;
        private Creature owner;
        public SkillDefine Define;

        private float cd;
        public float CD
        {
            get { return cd; }
        }

        public Skill(NSkillInfo info, Creature owner)
        {
            this.Info = info;
            this.owner = owner;
            this.Define = DataManager.Instance.Skills[(int)this.owner.Define.Class][this.Info.Id];
        }

        internal SkillResult Cast(BattleContext context)
        {
            SkillResult result = SkillResult.Ok;
            if (this.cd > 0)
                return SkillResult.CoolDown;
            switch(this.Define.CastTarget)
            {
                case TargetType.None:
                    // 无目标技能
                    break;
                case TargetType.Target:
                    if (context.Target != null)
                    {
                        this.DoSkillDamage(context);
                        this.cd = this.Define.CD;
                    }
                    else
                        result = SkillResult.InvalidTarget;
                    break;
                case TargetType.Self:
                    if (context.Caster != null)
                    {
                        // 添加技能效果
                    }
                    break;
                case TargetType.Position:
                    if (context.CastSkill.Position != null)
                    {
                        // 添加技能效果
                    }
                    break;
            }

            return result;
        }

        private void DoSkillDamage(BattleContext context)
        {
            context.Damage = new NDamageInfo();
            context.Damage.entityId = context.Target.entityId;
            context.Damage.Damage = 100;
            context.Target.DoDamage(context.Damage);
        }

        internal void Update()
        {
            UpdateCD();
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
    }
}
