using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequset);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }
        public void Init()
        {
            TeamManager.Instance.Init();
        }
        private void OnTeamInviteRequset(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteRequset: :FromID:{0} FromName:{1} ToId:{2} ToName:{3}", request.FronId, request.FromName, request.ToId, request.ToName);
            //TODO:执行一些前置数据校验


            if (request.ToId == 0)
            {
                foreach (var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.Data.Name == request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }
            NetConnection<NetSession> friend = null;
            if (request.ToId > 0)
            {
                if (character.FriendManager.GetFriendInfo(request.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "已经是好友了";
                    sender.SendResponse();
                    return;
                }
                friend = SessionManager.Instance.GetSession(request.ToId);
            }
            if (friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "好友不存在或者离线";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardRequest: :FromID:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            friend.Session.Response.friendAddReq = request;
            friend.SendResponse();
        }

        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse: :character:{0} Result:{1} FroId:{2} ToId:{3}", character.Id, response.Request, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddRes = response;

            if (response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "请求者离线";
                }
                else
                {
                    //添加好友
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove: :character:{0} FriendReletionID:{1}", character.Id, request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            if (character.FriendManager.RemoveFriendByID(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                // 删除别人好友中的自己
                var friend = SessionManager.Instance.GetSession(request.friendId);
                if (friend != null)
                {
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(character.Id);
                }
                else
                {
                    //不在线
                    this.RemoveFriend(request.friendId, character.Id);
                }
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;
            DBService.Instance.Save();
            sender.SendResponse();
        }
    }
}
