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
    internal class StoryManager : Singleton<StoryManager>
    {
        public const int MaxInstance = 100;//最大副本数量（和服务器性能有关）

        // 有多个地图有故事副本
        public class StoryMap
        {
            public Queue<int> InstanceIndexes = new Queue<int>();
            public Story[] Storys = new Story[MaxInstance];
        }

        Dictionary<int, StoryMap> Storys = new Dictionary<int, StoryMap>();

        public void Init()
        {
            foreach (var story in DataManager.Instance.Storys)
            {
                StoryMap map = new StoryMap();
                for (int i = 0; i < MaxInstance; i++)
                {
                    map.InstanceIndexes.Enqueue(i);
                }
                this.Storys[story.Key] = map;
            }
        }

        public Story NewStory(int storyId, NetConnection<NetSession> owner)
        {
            var storyMap = DataManager.Instance.Storys[storyId].MapId;
            var instance = this.Storys[storyId].InstanceIndexes.Dequeue();
            var map = MapManager.Instance.GetInstance(storyMap, instance);
            Story story = new Story(map, storyId, instance, owner);
            this.Storys[storyId].Storys[instance] = story;
            story.PlayerEnter();
            return story;
        }

        internal void Update()
        {
            
        }

        internal Story GetStory(int storyId, int instanceId)
        {
            return this.Storys[storyId].Storys[instanceId];
        }
    }
}
