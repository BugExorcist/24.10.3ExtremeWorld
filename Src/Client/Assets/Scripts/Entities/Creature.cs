using System;
using System.Collections.Generic;
using Battle;
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
        public BuffManager buffMgr;
        public EffectManager effectMgr;

        public Action<Buff> OnBuffAdd;
        public Action<Buff> OnBuffRemove;

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
            this.buffMgr = new BuffManager(this);
            this.effectMgr = new EffectManager(this);
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

        internal void CsatSkill(int skillId, Creature target, NVector3 pos)
        {
            this.SetStandby(true);
            Skill skill = this.SkillMgr.GetSkill(skillId);
            skill.BeginCast(target, pos);
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
            this.buffMgr.OnUpdate(delta);
        }

        public void DoDamage(NDamageInfo damage, bool playHurt)
        {
            Debug.LogFormat("DoDamage:{0} DMG:{1} CRIT:{2}", this.Name, damage.Damage, damage.Crit);
            this.Attributes.HP -= damage.Damage;
            if (playHurt) this.PlayAnim("Hurt");
            if (this.Controller != null)
            {
                UIWorldElementManager.Instance.ShowPopupText(PopupType.Damage, this.Controller.GetTransform().position + this.GetPopupOffset(), -damage.Damage, damage.Crit);
            }
        }

        internal void DoSkillHit(NSkillHitInfo hitInfo)
        {
            Skill skill = this.SkillMgr.GetSkill(hitInfo.skillId);
            skill.DoHit(hitInfo);
        }

        internal int Distance(Creature target)
        {
            return (int)Vector3Int.Distance(this.position, target.position);
        }

        internal void DoBuffAction(NBuffInfo buff)
        {
            switch (buff.Action)
            {
                case BuffAction.Add:
                    this.AddBuff(buff.buffId, buff.buffType, buff.casterId);
                    break;
                case BuffAction.Remove:
                    this.RemoveBuff(buff.buffId);
                    break;
                case BuffAction.Hit:
                    this.DoDamage(buff.Damage, false);
                    break;
                default:
                    break;
            }
        }


        private void AddBuff(int buffId, int buffType, int casterId)
        {
            var buff = this.buffMgr.AddBuff(buffId, buffType, casterId);
            if (buff != null)
                this.OnBuffAdd?.Invoke(buff);
        }

        public void RemoveBuff(int buffId)
        {
            var buff = this.buffMgr.RemoveBuff(buffId);
            if (buff != null)
                this.OnBuffRemove?.Invoke(buff);
        }

        internal void AddBuffEffext(BuffEffect effect)
        {
            this.effectMgr.AddEffect(effect);
        }

        internal void RemoveBuffEffect(BuffEffect effect)
        {
            this.effectMgr.RemoveEffect(effect);
        }

        internal void FaceTo(Vector3Int position)
        {
            this.SetDirection(GameObjectTool.WorldToLogic(GameObjectTool.LogicToWorld(position - this.position).normalized));
            this.UpdateEntityData();
            if (this.Controller != null)
            {
                this.Controller.UpdateDirection();
            }
        
        }
        /// <summary>
        /// 目标类播放特效
        /// </summary>
        internal void PlayEffect(EffectType type, string name, Creature target, float duration = 0)
        {
            if (string.IsNullOrEmpty(name)) return;
            if (this.Controller != null)
                this.Controller.PlayEffect(type, name, target, duration);
        }
        /// <summary>
        /// 位置类播放特效
        /// </summary>
        internal void PlayEffect(EffectType type, string name, NVector3 position)
        {
            if (string.IsNullOrEmpty (name)) return;
            if (this.Controller != null)
                this.Controller.PlayEffect(type, name, position, 0);
        }

        /// <summary>
        /// 获取受击特效高度
        /// </summary>
        internal Vector3 GetHitOffset()
        {
            return new Vector3(0, this.Define.Height * 0.8f, 0);
        }

        /// <summary>
        /// 获取伤害飘字高度
        /// </summary>
        internal Vector3 GetPopupOffset()
        {
            return new Vector3(0, this.Define.Height, 0);
        }
    }
}
