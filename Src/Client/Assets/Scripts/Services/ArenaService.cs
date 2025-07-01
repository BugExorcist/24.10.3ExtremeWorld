using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Services
{
    internal class ArenaService : Singleton<ArenaService> , IDisposable
    {
        public void Init()
        {
            ArenaManager.Instance.Init();
        }

        public ArenaService()
        {
            MessageDistributer.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Subscribe<ArenaBeginResponse>(this.OnArenaBegin);
            MessageDistributer.Instance.Subscribe<ArenaEndResponse>(this.OnArenaEnd);
            MessageDistributer.Instance.Subscribe<ArenaReadyResponse>(this.OnArenaReady);
            MessageDistributer.Instance.Subscribe<ArenaRoundStratResponse>(this.OnArenaRoundStart);
            MessageDistributer.Instance.Subscribe<ArenaRoundEndResponse>(this.OnArenaRoundEnd);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer.Instance.Unsubscribe<ArenaBeginResponse>(this.OnArenaBegin);
            MessageDistributer.Instance.Unsubscribe<ArenaEndResponse>(this.OnArenaEnd);
            MessageDistributer.Instance.Unsubscribe<ArenaReadyResponse>(this.OnArenaReady);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundStratResponse>(this.OnArenaRoundStart);
            MessageDistributer.Instance.Unsubscribe<ArenaRoundEndResponse>(this.OnArenaRoundEnd);
        }


        /// <summary>
        /// 发送竞技邀请
        /// </summary>
        public void SendAranaChallengeResquest(int friendId, string friendNama)
        {
            Debug.Log("SendAranaChallengeResquest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeReq = new ArenaChallengeRequest();
            message.Request.arenaChallengeReq.ArenaInfo = new ArenaInfo();
            message.Request.arenaChallengeReq.ArenaInfo.Red = new ArenaPlayer()
            {
                EntityId = User.Instance.CurrentCharacter.Id,
                Name = User.Instance.CurrentCharacter.Name,
            };
            message.Request.arenaChallengeReq.ArenaInfo.Blue = new ArenaPlayer()
            {
                EntityId = friendId,
                Name = friendNama,
            };
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到竞技邀请
        /// </summary>
        private void OnArenaChallengeRequest(object sender, ArenaChallengeRequest request)
        {
            Debug.Log("OnArenaChallengeRequest");
            var confirm = MessageBox.Show(string.Format("{0} 邀请你竞技场对战", request.ArenaInfo.Red.Name), "竞技场对战", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                this.SendAranaChallengeResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                this.SendAranaChallengeResponse(false, request);
            };
        }

        /// <summary>
        /// 发送竞技邀请响应
        /// </summary>
        public void SendAranaChallengeResponse(bool accept, ArenaChallengeRequest request)
        {
            Debug.Log("SendAranaChallengeResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaChallengeRes = new ArenaChallengeResponse();
            message.Request.arenaChallengeRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.arenaChallengeRes.Errormsg = accept ? "" : "对方拒绝了挑战请求";
            message.Request.arenaChallengeRes.ArenaInfo = request.ArenaInfo;
            NetClient.Instance.SendMessage(message);
        }

        /// <summary>
        /// 收到竞技邀请响应
        /// </summary>
        private void OnArenaChallengeResponse(object sender, ArenaChallengeResponse response)
        {
            if (response.Result == Result.Failed)
            {
                MessageBox.Show(response.Errormsg, "竞技场对战");
            }
        }
        
        /// <summary>
        /// 开始竞技
        /// </summary>
        private void OnArenaBegin(object sender, ArenaBeginResponse message)
        {
            Debug.Log("OnArenaBegin");
            ArenaManager.Instance.EnterArena(message.ArenaInfo);
        }

        /// <summary>
        /// 结束竞技
        /// </summary>
        private void OnArenaEnd(object sender, ArenaEndResponse message)
        {
            Debug.Log("OnArenaEnd");
            ArenaManager.Instance.ExitArena(message.ArenaInfo);
        }

        internal void SendArenaReadyRequest(int arenaId)
        {
            Debug.Log("SendArenaReadyRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.arenaReady = new ArenaReadyRequest();
            message.Request.arenaReady.entityId = User.Instance.CurrentCharacter.entityId;
            message.Request.arenaReady.arenaId = arenaId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnArenaReady(object sender, ArenaReadyResponse response)
        {
            ArenaManager.Instance.OnReady(response.Round, response.ArenaInfo);
        }

        private void OnArenaRoundStart(object sender, ArenaRoundStratResponse response)
        {
            ArenaManager.Instance.OnRoundStart(response.Round, response.ArenaInfo);
        }

        private void OnArenaRoundEnd(object sender, ArenaRoundEndResponse response)
        {
            ArenaManager.Instance.OnRoundEnd(response.Round, response.ArenaInfo);
        }
    }
}
