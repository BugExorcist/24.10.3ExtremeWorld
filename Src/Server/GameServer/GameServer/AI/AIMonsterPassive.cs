using GameServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.AI
{
    internal class AIMonsterPassive : AIBase
    {
        public const string ID = "AIMonsterPassive";

        public AIMonsterPassive(Monster monster) : base(monster)
        {
        }
    }
}
