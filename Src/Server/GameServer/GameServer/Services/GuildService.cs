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

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequset requset)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList: :character:{0}:{1}", character.Id, character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Result = Result.Success;
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
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
            if (response.Result == Result.Success)
            {//接受了公会请求
                guild.JoinAppove(response.Apply);
                DBService.Instance.Entities.Characters.FirstOrDefault(v => v.ID == response.Apply.characterId).GuildId = response.Apply.GuildId;
                DBService.Instance.Save();
            }
            if (requester != null)
            {
                requester.Session.Character.Guild = guild;
                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
            }
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave: :character:{0}:{1}", character.Id, character.Name);
            character.Guild.Leave(character);

            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            sender.Session.Response.guildLeave.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();
        }
    }
}
