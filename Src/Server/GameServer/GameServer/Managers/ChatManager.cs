using Common;
using Common.Data;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public List<ChatMessage> System = new List<ChatMessage>();
        public List<ChatMessage> World = new List<ChatMessage>();
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>(); // 每个地图维护一个聊天记录
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();  // 每个队伍维护一个聊天记录
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>(); // 每个公会维护一个聊天记录

        public void Init()
        {

        }

        public void AddMessage(Character from, ChatMessage message)
        { 
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    this.AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;
                case ChatChannel.Team:
                    if (from.Team != null)
                        this.AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.Guild:
                    if (from.Guild != null)
                        this.AddGuildMessage(from.Guild.Id, message);
                    break;
            }
        }

        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> list))
            {
                list = new List<ChatMessage>();
                this.Local[mapId] = list;
            }
            list.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        private void AddTeamMessage(int teamId, ChatMessage message)
        {
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> list))
            {
                list = new List<ChatMessage>();
                this.Team[teamId] = list;
            }
            list.Add(message);
        }

        private void AddGuildMessage(int guildId, ChatMessage message)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> list))
            {
                list = new List<ChatMessage>();
                this.Guild[guildId] = list;
            }
            list.Add(message);
        }

        public int GetLocalMessage(int mapId, int idx, List<ChatMessage> result)
        {
            if (!this.Local.TryGetValue(mapId, out List<ChatMessage> list))
                return 0;
            return GetNewMessage(idx, result, list);
        }

        public int GetGuildMessage(int guildId, int idx, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> list))
                return 0;
            return GetNewMessage(idx, result, list);
        }

        public int GetTeamMessage(int teamId, int idx, List<ChatMessage> result)
        {
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> list))
                return 0;
            return GetNewMessage(idx, result, list);
        }

        public int GetSystemMessage(int idx, List<ChatMessage> result)
        {
            return GetNewMessage(idx, result, this.System);
        }

        public int GetWorldMessage(int idx, List<ChatMessage> result)
        {
            return GetNewMessage(idx, result, this.World);
        }
        /// <summary>
        /// 获取最新的message.Count - idx条数据，如果idx为0，则返回近10分钟内最多20条消息
        /// </summary>
        /// <param name="idx">已经获取到的信息索引</param>
        /// <param name="result">返回的message列表</param>
        /// <param name="messages">message数据来源</param>
        /// <returns></returns>
        private int GetNewMessage(int idx, List<ChatMessage> result, List<ChatMessage> messages)
        {
            if (idx == 0)
            {
                if (messages.Count > GameDefine.MaxChatRecoredNums)
                {
                    int countIdx = messages.Count - GameDefine.MaxChatRecoredNums;
                    int timeIdx = countIdx;
                    var now = TimeUtil.timestamp;
                    for (; timeIdx < messages.Count; timeIdx++)
                    {
                        if (now - messages[timeIdx].Time <  GameDefine.MaxChatRecoredTime)
                        {
                            break;
                        }
                    }
                    idx = timeIdx;
                }
            }

            for(; idx < messages.Count; idx++)
            {
                result.Add(messages[idx]);
            }
            return idx;
        }
    }
}
