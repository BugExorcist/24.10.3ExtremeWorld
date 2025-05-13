using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamSetLeaderRequest>(this.OnSetLeader);

        }


        public void Init()
        {
            TeamManager.Instance.Init();
        }
        private void OnTeamInviteRequset(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamInviteRequset: :FromID:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            if (character == null)
                return;

            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "好友不在线";
                sender.SendResponse();
                return;
            }
            if (target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse();
                sender.Session.Response.teamInviteRes.Result = Result.Failed;
                sender.Session.Response.teamInviteRes.Errormsg = "对方已有队伍";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("ForwardRequest: :FromID:{0} FromName:{1} ToId:{2} ToName:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }

        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse: :character:{0} Result:{1} FroId:{2} ToId:{3}", character.Id, response.Request, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.teamInviteRes = response;

            if (response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者离线";
                }
                else
                {
                    //组队
                    TeamManager.Instance.AddTamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.Session.Response.teamInviteRes.Errormsg = string.Format("{0}加入队伍", character.Info.Name);
                    sender.Session.Response.teamInviteRes.Errormsg = string.Format("加入{0}的队伍", requester.Session.Character.Info.Name);
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            if (request.memberId == sender.Session.Character.Id)
            {
                Log.InfoFormat("OnTeamLeave: : TeamID:{0}memberId:{1}", request.TeamId, request.memberId);
                if (character.Team == null)
                {
                    sender.Session.Response.teamLeave.Result = Result.Failed;
                    sender.Session.Response.teamLeave.Errormsg = "当前没有组队";
                    sender.SendResponse();
                    return;
                }
                sender.Session.Character.Team.Leave(character);
                sender.Session.Response.teamLeave.characterId = request.memberId;
                sender.Session.Response.teamLeave.Result = Result.Success;
                sender.Session.Response.teamLeave.Errormsg = "退出成功";
                sender.SendResponse();
                return;
            }
            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.memberId);
            target.Session.Response.teamLeave = new TeamLeaveResponse();
            Character tcharacter = target.Session.Character;
            Log.InfoFormat("OnTeamLeave: : TeamID:{0}memberId:{1}",request.TeamId, request.memberId);
            if (character == null)
            {
                sender.Session.Response.teamLeave.Result = Result.Failed;
                sender.Session.Response.teamLeave.Errormsg = "该角色离线";
                sender.SendResponse();
                return;
            }
            if (character.Team == null)
            {
                sender.Session.Response.teamLeave.Result = Result.Failed;
                sender.Session.Response.teamLeave.Errormsg = "此角色当前没有组队";
                sender.SendResponse();
                return;
            }
            if(character.Team.Id != tcharacter.Team.Id)
            {
                sender.Session.Response.teamLeave.Result = Result.Failed;
                sender.Session.Response.teamLeave.Errormsg = "此角色不在你的队伍中";
                sender.SendResponse();
                return;
            }
            sender.Session.Character.Team.Leave(tcharacter);
            sender.Session.Response.teamLeave.characterId = request.memberId;
            sender.Session.Response.teamLeave.Result = Result.Failed;//Result.Failed不会隐藏TeamUI
            sender.Session.Response.teamLeave.Errormsg = "踢出成功";
            target.Session.Response.teamLeave.characterId = request.memberId;
            target.Session.Response.teamLeave.Result = Result.Success;
            target.Session.Response.teamLeave.Errormsg = "你被踢出队伍";
            sender.SendResponse();
            target.SendResponse();
        }

        private void OnSetLeader(NetConnection<NetSession> sender, TeamSetLeaderRequest request)
        {
            Log.InfoFormat("OnSetLeader: : TeamID:{0} newLeaderId:{1}", request.TeamId, request.LeaderId);
            Character character = sender.Session.Character;
            var target = SessionManager.Instance.GetSession(request.LeaderId);
            sender.Session.Response.teamSetLeader = new TeamSetLeaderResponse();
            if (target.Session.Character.Team.Id != request.TeamId || target == null)
            {
                sender.Session.Response.teamSetLeader.Result = Result.Failed;
                sender.Session.Response.teamSetLeader.Errormsg = "该角色离线或不在你的队伍中";
                sender.SendResponse();
                return;
            }
            character.Team.SetLeader(target.Session.Character);
            sender.Session.Response.teamSetLeader.Result = Result.Success;
            sender.Session.Response.teamSetLeader.Errormsg = "设置新队长成功";
            target.Session.Response.teamSetLeader = new TeamSetLeaderResponse();
            target.Session.Response.teamSetLeader.Result = Result.Success;
            target.Session.Response.teamSetLeader.Errormsg = "你被设置为队长";
            sender.SendResponse();
            target.SendResponse();
        }
    } 
}
