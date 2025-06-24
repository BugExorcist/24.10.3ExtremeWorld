using Common.Data;
using Entities;
using Common.Battle;
using System;
using UnityEngine;

namespace Battle
{
    public class Buff
    {
        internal bool Stoped;
        public Creature Owner;
        public int BuffId;
        public BuffDefine Define;
        public int CasterId;
        public float time;

        public Buff(Creature owner, int buffId, BuffDefine define, int casterId)
        {
            this.Owner = owner;
            this.BuffId = buffId;
            this.Define = define;
            this.CasterId = casterId;
            this.Stoped = false;
            time = 0;
            this.OnAdd();
        }

        private void OnAdd()
        {
            Debug.LogFormat("Buff.OnAdd: {0}:{1}", this.BuffId, this.Define.Name);
            if (this.Define.Effect != BuffEffect.None)
            {
                this.Owner.AddBuffEffext(this.Define.Effect);
            }
            AddAttr();
        }

        internal void OnRemove()
        {
            Debug.LogFormat("Buff.OnRemove: {0}:{1}", this.BuffId, this.Define.Name);
            RemoveAttr();

            Stoped = true;
            if (this.Define.Effect != BuffEffect.None)
            {
                this.Owner.RemoveBuffEffect(this.Define.Effect);
            }
        }

        private void AddAttr()
        {
            if (this.Define.DEFRantio != 0)
            {
                this.Owner.Attributes.Buff.DEF += this.Owner.Attributes.Basic.DEF * this.Define.DEFRantio;
            }
            this.Owner.Attributes.InitFinalAttributes();
        }

        private void RemoveAttr()
        {
            if (this.Define.DEFRantio != 0)
            {
                this.Owner.Attributes.Buff.DEF -= this.Owner.Attributes.Basic.DEF * this.Define.DEFRantio;
            }
            this.Owner.Attributes.InitFinalAttributes();
        }


        internal void OnUpdate(float delta)
        {
            if (Stoped) return;

            this.time += delta;

            if (time > this.Define.Duration)
            {
                this.OnRemove();
            }
        }
    }
}
