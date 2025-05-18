using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
        }

        public void Init()
        {

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }

        public void SendChat(ChatChannel sendChannel, string text, int toId, string toName)
        {
            Debug.Log("SendChat");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.ToId = toId;
            message.Request.Chat.Message.ToName  = toName;
            message.Request.Chat.Message.Message = text;
            message.Request.Chat.Message.Channel = sendChannel;
            NetClient.Instance.SendMessage(message);
        }

        private void OnChat(object sender, ChatResponse response)
        {
            Debug.Log("OnChat");
            if (response.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.World, response.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Local, response.localMessages);
                ChatManager.Instance.AddMessages(ChatChannel.System, response.systemMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Team, response.teamMessages);
                ChatManager.Instance.AddMessages(ChatChannel.Guild, response.guildMessages);
            }
            else
            {
                MessageBox.Show(string.Format("聊天失败:{0}", response.Errormsg), "聊天");
            }
            ChatManager.Instance.AddMessages(ChatChannel.Private, response.privateMessages);
        }
    }
}
