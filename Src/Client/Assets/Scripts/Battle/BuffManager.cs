using Common.Data;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    public class BuffManager
    {
        private Creature Owner;

        public Dictionary<int, Buff> Buffs = new Dictionary<int, Buff>();

        public BuffManager(Creature creature)
        {
            this.Owner = creature;
        }

        internal Buff AddBuff(int buffId, int buffType, int casterId)
        {
            BuffDefine define;
            if (DataManager.Instance.Buffs.TryGetValue(buffType, out define))
            {
                Buff buff = new Buff(this.Owner, buffId, define, casterId);
                this.Buffs.Add(buffId, buff);
                return buff;
            }
            return null;
        }

        internal Buff RemoveBuff(int buffId)
        {
            Buff buff;
            if (this.Buffs.TryGetValue(buffId, out buff))
            {
                buff.OnRemove();
                this.Buffs.Remove(buffId);
                return buff;
            }
            return null;
        }

        internal void OnUpdate(float delta)
        {
            List<int> needRemove = new List<int>();
            foreach (var buff in this.Buffs)
            {
                buff.Value.OnUpdate(delta);
                if (buff.Value.Stoped)
                {
                    needRemove.Add(buff.Key);
                }
            }
            foreach (var key in needRemove)
            {
                this.Owner.RemoveBuff(key);
            }
        }
    }
}
