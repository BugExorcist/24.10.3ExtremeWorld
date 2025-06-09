using System.Collections.Generic;
using Batttle;
using Common.Battle;
using Common.Data;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Creature : Entity
    {
        public NCharacterInfo Info;
        public CharacterDefine Define;
        public Attributes Attributes;
        public SkillManager SkillMgr;

        bool battleState = false;
        public bool BattleState
        {
            get { return battleState; }
            set
            {
                if (battleState != value)
                {
                    battleState = value;
                    this.SetStandby(value);
                }
            }
        }

        public int Id
        {
            get { return Info.Id; }
        }

        public Skill CastringSkill = null;

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return this.Info.Type == CharacterType.Player; }
        }

        public bool IsCurrentPlayer
        {
            get
            {
                if (!IsPlayer) return false;
                return this.Id == Models.User.Instance.CurrentCharacterInfo.Id;
            }
        }

        public Creature(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.ConfigId];
            this.Attributes = new Attributes();
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquip(), this.Info.attDynamic);
            this.SkillMgr = new SkillManager(this);
        }

        public void UpdateInfo(NCharacterInfo info)
        {
            this.entityId = info.EntityId;
            this.EntityData = info.Entity;
            this.SetEntityData(info.Entity);
            this.Info = info;
            this.Attributes.Init(this.Define, this.Info.Level, this.GetEquip(), this.Info.attDynamic);
            this.SkillMgr.UpdateSkills();
        }

        public virtual List<EquipDefine> GetEquip()
        {
            return null;
        }

        public void MoveForward()
        {
            Debug.LogFormat("MoveForward");
            this.speed = this.Define.Speed;
        }

        public void MoveBack()
        {
            Debug.LogFormat("MoveBack");
            this.speed = -this.Define.Speed;
        }

        public void Stop()
        {
            Debug.LogFormat("Stop");
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            Debug.LogFormat("SetDirection:{0}", direction);
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            Debug.LogFormat("SetPosition:{0}", position);
            this.position = position;
        }

        internal void CsatSkill(int skillId, Creature target, NVector3 pos, NDamageInfo damage)
        {
            this.SetStandby(true);
            Skill skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast(damage);
        }

        private void SetStandby(bool v)
        {
            if (this.Controller != null)
            {
                this.Controller.SetStandBy(v);
            }
        }

        public void PlayAnim(string name)
        {
            if (this.Controller != null)
            {
                this.Controller.PlayAnim(name);
            }
        }

        public override void OnUpdate(float delta)
        {
            base.OnUpdate(delta);

            this.SkillMgr.OnUpdate(delta);
        }

        public void DoDamage(NDamageInfo damage)
        {
            Debug.LogFormat("DoDamage:{0}", damage);
            this.Attributes.HP -= damage.Damage;
            this.PlayAnim("Hurt");
        }
    }
}
