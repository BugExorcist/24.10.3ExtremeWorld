using Common.Data;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    public class BuffManager
    {
        private Creature Owner;
        private List<Buff> Buffs = new List<Buff>();
        private int idx = 1;
        private int BuffID
        {
            get { return this.idx++; }
        }


        public BuffManager(Creature creature)
        {
            this.Owner = creature;
        }

        internal void AddBuff(BattleContext context, BuffDefine buffDefine)
        {
            foreach (Buff item in this.Buffs)
            {
                if (item.Define.ID == buffDefine.ID)
                {
                    item.ReSetTime();
                    return;
                }
            }
            Buff buff = new Buff(this.BuffID, this.Owner, buffDefine, context);
            this.Buffs.Add(buff);
        }

        internal void Update()
        {
            for(int i = 0; i < this.Buffs.Count; i++)
            {
                if (!this.Buffs[i].Stoped)
                    this.Buffs[i].Update();
            }
            this.Buffs.RemoveAll((b) => b.Stoped);
        }
    }
}
