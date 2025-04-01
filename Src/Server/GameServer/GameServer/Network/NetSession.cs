using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    class NetSession : INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }

        internal void Disconnected()
        {
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }

        NetMessage message;

        public NetMessageResponse Response
        {
            get
            {
                if (message == null)
                {
                    message =  new NetMessage();
                }
                if(message.Response == null)
                {
                    message.Response = new NetMessageResponse();
                }
                return message.Response;
            }
        }

        public byte[] GetResponse()
        {
            if(message != null)
            {
                if(this.Character != null && this.Character.StatusManager.HasStatus)
                {
                    this.Character.StatusManager.ApplyResponse(Response);
                }
                byte[] data = PackageHandler.PackMessage(message);
                message = null;
                return data;
            }
            return null;
        }
    }
}
