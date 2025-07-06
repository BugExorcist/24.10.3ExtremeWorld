using Common.Battle;
using Common.Data;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Entities
{
    // Creature：战斗的基本单元
    public class Creature : Entity
    {
        public int Id { get { return this.Info.Id; } }
        public string Name { get { return this.Info.Name; } }
        public NCharacterInfo Info;
        public CharacterDefine Define;

        public SkillManager SkillMgr;
        public BuffManager BuffMgr;
        public EffectManager EffectMgr;

        public Attributes Attributes;
        public bool IsDeath = false;

        public BattleState BattleState;
        public CharacterState State;

        public Map Map;

        public Creature(CharacterType type, int configId, int level, Vector3Int pos, Vector3Int dir) :
           base(pos, dir)
        {
            this.Define = DataManager.Instance.Characters[configId];
            this.Info = new NCharacterInfo();
            this.Info.Type = type;
            this.Info.Level = level;
            this.Info.ConfigId = configId;
            this.Info.Entity = this.EntityData;
            this.Info.EntityId = this.entityId;
            this.Info.Name = this.Define.Name;
            this.InitBuff();

            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquips(), this.Info.attDynamic);
            this.Info.attDynamic = this.Attributes.DynamicAttr;
        }

        internal virtual void OnEnterMap(Map map)
        {
            this.Map = map;
        }

        internal virtual void OnLeaveMap(Map map)
        {
            this.Map = null;
        }

        public virtual List<EquipDefine> GetEquips()
        {
            return null;
        }

        public void InitSkill()
        {
            SkillMgr = new SkillManager(this);
            this.Info.Skills.AddRange(this.SkillMgr.Infos);
        }

        private void InitBuff()
        {
            this.BuffMgr = new BuffManager(this);
            this.EffectMgr = new EffectManager(this);
        }

        internal void CastSkill(BattleContext context, int skillId)
        {
            Skill skill = this.SkillMgr.GetSkill(skillId);
            if (!this.SkillMgr.isCasting)
            {
                context.Result = skill.Cast(context);
                if (context.Result == SkillResult.Ok)
                {
                    this.BattleState = BattleState.InBattle;
                }

                if (context.CastSkill == null)//说明是Monster释放的技能
                {
                    if (context.Result == SkillResult.Ok)
                    {
                        context.CastSkill = new NSkillCastInfo()
                        {
                            casterId = this.entityId,
                            targetId = context.Target.entityId,
                            skillId = skill.Define.ID,
                            Position = new NVector3(),
                            Result = context.Result,
                        };
                        context.Battle.AddCastSkillInfo(context.CastSkill);
                    }
                }
                else//说明是Player释放的技能
                {
                    context.CastSkill.Result = context.Result;
                    context.Battle.AddCastSkillInfo(context.CastSkill);
                }
            }
        }

        internal void DoDamage(NDamageInfo damage, Creature sorce)
        {
            this.BattleState = BattleState.InBattle;
            this.Attributes.HP -= damage.Damage;
            if (this.Attributes.HP <= 0)
            {
                this.IsDeath = true;
                damage.WillDead = true;
            }
            this.OnDamage(damage, sorce);
        }


        public override void Update()
        {
            this.SkillMgr.Update();
            this.BuffMgr.Update();
        }

        internal int Distance(Creature target)
        {
            if (target != null)
                return (int)Vector3Int.Distance(this.Position, target.Position);
            return -1;
        }

        internal int Distance(Vector3Int pos)
        {
            return (int)Vector3Int.Distance(this.Position, pos);
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            this.BuffMgr.AddBuff(context, buffDefine);
        }

        protected virtual void OnDamage(NDamageInfo damage, Creature sorce)
        {

        }
    }
}
