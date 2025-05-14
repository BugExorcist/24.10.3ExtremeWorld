using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction<bool> OnGuildCreateResult;
        public UnityAction<List<NGuildInfo>> OnGuildListResult;
        public UnityAction OnGuildUpdate;
        public void Init()
        {

        }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
        }
        public void SendGuildCreate(string name, string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = name;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("OnGuildCreateResponse  : {0}", response.Result);
            this.OnGuildCreateResult?.Invoke(response.Result == Result.Success);
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0}公会创建成功", response.guildInfo.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0}公会创建失败", response.guildInfo.GuildName), "公会");
            }
        }
        public void SendGuildJoinRequest(int id)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            message.Request.guildJoinReq.Apply.GuildId = id;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} 申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm, "允许", "拒绝");
            confirm.OnYes += () =>
            {
                this.SendGuildJoinResponse(true, request);
            };
            confirm.OnNo += () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }

        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinReq.Apply = request.Apply;
            message.Request.guildJoinReq.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse  : {0}", response.Result);
            if (response.Result == Result.Success)
                MessageBox.Show("加入公会成功", "公会");
            else
                MessageBox.Show("加入公会失败 " + response.Errormsg, "公会");
        }
        /// <summary>
        /// 取得公会数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuild(object sender, GuildResponse response)
        {
            Debug.LogFormat("OnGuild : {0}  {1}:{2}", response.Result, response.guildInfo.Id, response.guildInfo.GuildName) ;
            GuildManager.Instance.Init(response.guildInfo);
            this.OnGuildUpdate?.Invoke();
        }

        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            }
            else
                MessageBox.Show("离开公会失败 " + response.Errormsg, "公会", MessageBoxType.Error);
        }

        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequset();
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildList(object sender, GuildListResponse response)
        {
            this.OnGuildListResult?.Invoke(response.Guilds);
        }
    }
}
