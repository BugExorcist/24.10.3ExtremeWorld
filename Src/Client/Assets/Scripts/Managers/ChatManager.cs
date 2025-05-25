using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Managers
{
    public class ChatManager : Singleton<ChatManager>
    {
        public Action OnChat;

        /// <summary>
        /// 本地频道枚举，与UI上频道的顺序相对应
        /// </summary>
        public enum LocalChannel
        { 
            All = 0,    //所有（综合）
            Local = 1,  //本地
            World  = 2, //世界
            Team  = 3,  //队伍
            Guild = 4,  //公会
            Private = 5,//私聊
        }

        /// <summary>
        /// 频道过滤器，把协议中的通讯频道转换成本地的枚举
        /// </summary>
        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.System | ChatChannel.Private | ChatChannel.Guild |ChatChannel.Team,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
        };

        public void Init()
        {
            foreach (var messages in this.Messages)
                messages.Clear();
        }

        public LocalChannel displayChannel;
        public LocalChannel sendChannel;

        /// <summary>
        /// 将本地频道转换为协议中的通讯频道
        /// </summary>
        public ChatChannel SendChannel
        {
            get
            {
                switch(sendChannel)
                {
                    case LocalChannel.Local:return ChatChannel.Local;
                    case LocalChannel.World:return ChatChannel.World;
                    case LocalChannel.Team:return ChatChannel.Team;
                    case LocalChannel.Guild:return ChatChannel.Guild;
                    case LocalChannel.Private:return ChatChannel.Private;
                        default:return ChatChannel.Local;
                }
            }
        }

        public int PrivateID = 0;
        public string PrivateName = "";
        // 临时列表消息
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        /// <summary>
        /// 设置私聊对象信息
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="targetName"></param>
        internal void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;
            OnChat?.Invoke();
        }

        /// <summary>
        /// 发送聊天
        /// </summary>
        public void SendChat(string text, int toId = 0, string toName = "")
        {
            if (this.SendChannel == ChatChannel.Private)
            {
                toId = this.PrivateID;
                toName = this.PrivateName;
            }
            ChatService.Instance.SendChat(this.SendChannel, text, toId, toName);
        }

        public bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍！");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (!GuildManager.Instance.HasGuild)
                {
                    this.AddSystemMessage("你没有加入任何公会！");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.Log("Set Channel: " + sendChannel);
            return true;
        }

        public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((this.ChannelFilter[i] & channel) == channel)
                    this.Messages[i].AddRange(messages);
            }
            OnChat?.Invoke();
        }

        private void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = from,
            });
            OnChat?.Invoke();
        }

        public string GetCurrentMessage()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage msg)
        {
            switch(msg.Channel)
            {
                case ChatChannel.Local://白色
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.World://蓝绿色
                    return string.Format("<#00FFE0>[世界]</color>{0}<#00FFE0>{1}</color>", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.System://中灰色 加粗
                    return string.Format("<#808080><b>[系统]{0}</b></color>", msg.Message);
                case ChatChannel.Private://紫色
                    return string.Format("<#9400D3>[私聊]</color>{0}<#9400D3>→</color>{1}<#9400D3>{2}</color>", FormatFromPlayer(msg), FormatToPlayer(msg), msg.Message);
                case ChatChannel.Team://橙红色
                    return string.Format("<#FF4500>[队伍]</color>{0}<#FF4500>{1}</color>", FormatFromPlayer(msg), msg.Message);
                case ChatChannel.Guild://橙色
                    return string.Format("<#FFA500>[公会]</color>{0}<#FFA500>{1}</color>", FormatFromPlayer(msg), msg.Message);
                default:
                    return "";
            }
        }

        private string FormatFromPlayer(ChatMessage msg)
        {   //深绿色
            if (msg.FromId == User.Instance.CurrentCharacterInfo.Id)
                return "<link=\"\"><#008000><u>[我]</u></color></link>";
            else
                return string.Format("<link=\"c:{0}:{1}\"><#008000><u>[{1}]</u></color></link>", msg.FromId, msg.FromName);
        }

        private object FormatToPlayer(ChatMessage msg)
        {   //深绿色
            if (msg.ToId == User.Instance.CurrentCharacterInfo.Id)
                return "<link=\"\"><#008000><u>[我]</u></color></link>";
            else
                return string.Format("<link=\"c:{0}:{1}\"><#008000><u>[{1}]</u></color></link>", msg.ToId, msg.ToName);
        }
    }
}
