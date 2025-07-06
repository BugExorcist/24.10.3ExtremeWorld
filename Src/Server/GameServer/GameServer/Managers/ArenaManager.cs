using Common;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class ArenaManager : Singleton<ArenaManager>
    {
        public const int ArenaMapId = 5;
        public const int MaxInstance = 100;//最大副本数量（和服务器性能有关）

        Queue<int> InstanceIndexes = new Queue<int>();

        Arena[]  Arenas = new Arena[MaxInstance];

        public void Init()
        {
            for(int i = 0; i < MaxInstance; i++)
            {
                InstanceIndexes.Enqueue(i);
            }
        }

        public Arena NewArena(ArenaInfo info, NetConnection<NetSession> red, NetConnection<NetSession> blue)
        {
            var instance = InstanceIndexes.Dequeue();
            var map = MapManager.Instance.GetInstance(ArenaMapId, instance);
            Arena arena = new Arena(map, info, red, blue);
            this.Arenas[instance] = arena;
            arena.PlayerEnter();
            return arena;
        }

        internal void Update()
        {
            for(int i = 0; i < this.Arenas.Length; i++)
            {
                if (this.Arenas[i] != null)
                {
                    Arenas[i].Update();
                }
            }
        }

        internal Arena GetArena(int arenaId)
        {
            if (Arenas[arenaId] == null) return null;

            return Arenas[arenaId];
        }
    }
}
