using GameServer.AI;
using GameServer.Battle;
using GameServer.Core;
using GameServer.Models;
using SkillBridge.Message;
using System;

namespace GameServer.Entities
{
    public class Monster : Creature
    {
        public Map Map;

        AIAgent AI;
        private Vector3Int moveTarget;
        Vector3 movePositon;

        public Monster(int tid, int level, Vector3Int pos, Vector3Int dir) : base(CharacterType.Monster, tid, level, pos, dir)
        {
            InitSkill();
            this.AI = new AIAgent(this);
        }

        public void OnEnterMap(Map map)
        {
            this.Map = map;
        }

        public override void Update()
        {
            base.Update();
            this.UpdateMovement();
            this.AI.Update();
        }


        public Skill FindSkill(BattleContext context, Common.Battle.SkillType type)
        {
            Skill cancast = null;
            foreach (Skill skill in this.SkillMgr.Skills)
            {
                if ((skill.Define.Type & type) != skill.Define.Type) continue;
                var result = skill.CanCast(context);
                if (result == SkillResult.Casting)
                {
                    return null;
                }
                if (result == SkillResult.Ok)
                {
                    cancast = skill;
                }
            }
            return cancast;
        }

        protected override void OnDamage(NDamageInfo damage, Creature source)
        {
            if (this.AI != null)
                this.AI.OnDamage(damage, source);
        }

        internal void MoveTo(Vector3Int position)
        {
            if (State == CharacterState.Idle)
            {
                State = CharacterState.Move;
            }
            if (this.moveTarget != position)
            {
                this.moveTarget = position;
                this.movePositon = Position;
                //与目标的距离
                var dist = (this.moveTarget - this.Position);
                //目标方向
                this.Direction = dist.normalized;
                this.Speed = this.Define.Speed;

                NEntitySync sync = new NEntitySync()
                {
                    Entity = this.EntityData,
                    Event = EntityEvent.MoveFwd,
                    Id = this.entityId,
                };

                this.Map.UpdateEntity(sync);
            }
        }

        /// <summary>
        /// 更新服务器的怪物移动
        /// </summary>
        private void UpdateMovement()
        {
            if (State == CharacterState.Move)
            {
                if (this.Distance(this.moveTarget) < 50)
                {
                    this.StopMove();
                }
                if(this.Speed > 0)
                {
                    Vector3 dir = this.Direction;
                    //借用浮点型来计算位置移动叠加，然后每帧再赋值给Int类型的Position
                    this.movePositon += dir * this.Speed * Time.deltaTime / 100f;
                    this.Position = this.movePositon;
                }
            }
        }

        internal void StopMove()
        {
            this.State = CharacterState.Idle;
            this.moveTarget = Vector3Int.zero;
            this.Speed = 0;
            NEntitySync sync = new NEntitySync()
            {
                Entity = this.EntityData,
                Event = EntityEvent.Idle,
                Id = this.entityId,
            };

            this.Map.UpdateEntity(sync);
        }
    }
}
