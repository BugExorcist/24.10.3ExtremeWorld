using Common;
using Common.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    public class EffectManager
    {
        private Creature Owner;

        Dictionary<BuffEffect, int> Effects = new Dictionary<BuffEffect, int>();
        public EffectManager(Creature creature)
        {
            this.Owner = creature;
        }

        internal void AddEffect(BuffEffect effect)
        {
            Log.InfoFormat("[0].AddEffct {1}", this.Owner.Name, effect);
            if (!this.Effects.ContainsKey(effect))
            {
                this.Effects[effect] = 1;
            }
            else
            {
                this.Effects[effect]++;
            }
        }

        internal void RemoveEffect(BuffEffect effect)
        {
            Log.InfoFormat("[0].RemoveEffct {1}", this.Owner.Name, effect);
            if (this.Effects.ContainsKey(effect))
            {
                this.Effects[effect]--;
                if (this.Effects[effect] <= 0)
                {
                    this.Effects.Remove(effect);
                }
            }
        }

        internal bool HasEffect(BuffEffect effect)
        {
            return this.Effects.ContainsKey(effect);
        }
    }
}
