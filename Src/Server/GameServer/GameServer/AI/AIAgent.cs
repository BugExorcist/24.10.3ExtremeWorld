using Common;
using GameServer.Battle;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.AI
{
    internal class AIAgent
    {
        public Monster monster;
        private AIBase ai;

        internal void Init()
        {

        }

        public AIAgent(Monster owner)
        {
            this.monster = owner;

            string aiName = owner.Define.AI;
            if (string.IsNullOrEmpty(aiName))
            {
                aiName = AIMonsterPassive.ID;
            }
            switch (aiName)
            {
                case AIMonsterPassive.ID:
                    this.ai = new AIMonsterPassive(monster);
                    break;
                case AIBoss.ID:
                    this.ai = new AIBoss(monster);
                    break;
            }

        }

        internal void Update()
        {
            if (this.ai == null)
            {
                Log.Info("AIAgent.Update:  ai not exist!");
                return;
            }
            this.ai.Update();
            
        }

        internal void OnDamage(NDamageInfo damage, Creature source)
        {
            if (this.ai != null)
            {
                this.ai.OnDamage(damage, source);
            }
        }

        
    }
}
