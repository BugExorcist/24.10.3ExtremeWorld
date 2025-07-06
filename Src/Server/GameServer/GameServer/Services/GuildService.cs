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
    public class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequset>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinReq);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinRes);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildSetNoticeRequest>(this.OnGuildSetNotice);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate: :GuildName:{0} character:{1}}", request.GuildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已经有公会";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已存在";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.SendResponse();
        }

        /// <summary>
        /// 返回所有公会，如果是公会的成员，则返回加入的公会信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="requset"></param>
        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequset requset)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList: :character:{0}:{1}", character.Id, character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Result = Result.Success;
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo(character));
            sender.SendResponse();
        }

        private void OnGuildJoinReq(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinReq: :character:{0}:{1}  GuildId:{2}", request.Apply.characterId, request.Apply.Name, request.Apply.GuildId);
            Guild guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;

            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if (leader != null)
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                sender.SendResponse();
            }
        }

        private void OnGuildJoinRes(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRes: :character:{0}:{1}  GuildId:{2}", response.Apply.characterId, response.Apply.Name, response.Apply.GuildId);
            Guild guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            requester.Session.Response.guildJoinRes = new GuildJoinResponse();
            requester.Session.Response.guildJoinRes.Result = Result.Success;
            requester.Session.Response.guildJoinRes.Apply = response.Apply;
            guild.JoinAppove(response.Apply);
            if (response.Apply.Result == ApplyResult.Accept)
            {   //接受了公会请求
                if (requester != null)
                {
                    requester.Session.Character.Guild = guild;

                    requester.Session.Response.guildJoinRes.Apply.Result = response.Apply.Result;
                    requester.SendResponse();
                }
            }
            if (response.Apply.Result == ApplyResult.Reject)
            {
                if (requester != null)
                {
                    requester.Session.Response.guildJoinRes.Apply.Result = response.Apply.Result;
                    requester.Session.Response.guildJoinRes.Errormsg = "申请被拒绝";
                    requester.SendResponse();
                }
                var apply = character.Guild.Data.GuildApplies.FirstOrDefault(v => v.CharacterID == response.Apply.characterId && v.GuildId == character.Guild.Id);
                if (apply != null)
                    DBService.Instance.Entities.GuildApplies.Remove(apply);
            }
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave: :character:{0}:{1}", character.Id, character.Name);
            character.Guild.Leave(character.Id);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            sender.Session.Response.guildLeave.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin: :character:{0}:{1}", character.Id, character.Name);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if (character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "您没有公会";
                sender.SendResponse();
                return;
            }
            if (character.Id == request.Target)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "无法对自己操作";
                sender.SendResponse();
                return;
            }

            var membertarget = character.Guild.GetDBMemeber(request.Target);
            var membersource = character.Guild.GetDBMemeber(character.Id);
            if (request.Command == GuildAdminCommand.Depose || request.Command == GuildAdminCommand.Promote)
            {
                if (membersource.Title != (int)GuildTitle.President)
                {
                    sender.Session.Response.guildAdmin.Result = Result.Failed;
                    sender.Session.Response.guildAdmin.Errormsg = "只有会长能进行此操作";
                    sender.SendResponse();
                    return;
                }
            }

            if (request.Command == GuildAdminCommand.Kickout)
            {
                if (membersource.Title != (int)GuildTitle.President && membertarget.Title == (int)GuildTitle.President)
                {
                    sender.Session.Response.guildAdmin.Result = Result.Failed;
                    sender.Session.Response.guildAdmin.Errormsg = "您无权踢出会长";
                    sender.SendResponse();
                    return;
                }
                if (membersource.Title != (int)GuildTitle.President && membertarget.Title == (int)GuildTitle.VicePresident)
                {
                    sender.Session.Response.guildAdmin.Result = Result.Failed;
                    sender.Session.Response.guildAdmin.Errormsg = "您无权踢出副会长";
                    sender.SendResponse();
                    return;
                }
            }

            character.Guild.ExecuteAdmin(request.Command, request.Target, character.Id);

            var target = SessionManager.Instance.GetSession(request.Target);
            if (target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Command = request;
                switch (request.Command)
                {
                    case GuildAdminCommand.Kickout:
                        target.Session.Response.guildAdmin.Errormsg = "您已被踢出公会";
                        break;
                    case GuildAdminCommand.Promote:
                        target.Session.Response.guildAdmin.Errormsg = "您已被提升为公会管理员";
                        break;
                    case GuildAdminCommand.Depose:
                        target.Session.Response.guildAdmin.Errormsg = "您已被降级为普通成员";
                        break;
                    case GuildAdminCommand.Transfer:
                        target.Session.Response.guildAdmin.Errormsg = "公会会长被转让给你";
                        break;
                }
                target.SendResponse();
            }
            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = request;
            sender.Session.Response.guildAdmin.Errormsg = "执行成功";
            sender.SendResponse();
        }

        private void OnGuildSetNotice(NetConnection<NetSession> sender, GuildSetNoticeRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildSetNotice: :character:{0}:{1}", character.Id, character.Name);
            sender.Session.Response.guildSetNotice = new GuildSetNoticeResponse();
            if (character.Guild == null)
            {
                sender.Session.Response.guildSetNotice.Result = Result.Failed;
                sender.Session.Response.guildSetNotice.Errormsg = "您没有公会";
                sender.SendResponse();
            }
            if (character.Guild.GetTatle(character.Id) == GuildTitle.None)
            {
                sender.Session.Response.guildSetNotice.Result = Result.Failed;
                sender.Session.Response.guildSetNotice.Errormsg = "您没有权限";
                sender.SendResponse();
            }
            character.Guild.SetNotice(request.Notice);
            sender.Session.Response.guildSetNotice.Result = Result.Success;
            sender.SendResponse();
        }
    }
}
