using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return Data.Id; } }
        public string Name { get { return Data.Name; } }
        public int timestamp;
        public TGuild Data;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.GuildApplies.FirstOrDefault(v => v.CharacterID == apply.characterId);
            if (oldApply != null)
                return false;

            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterID = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.GuildApplies.Add(dbApply);

            DBService.Instance.Save();
            this.timestamp = Time.timestamp;
            return true;
        }
        /// <summary>
        /// 确认申请
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        internal bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.GuildApplies.FirstOrDefault(v => v.CharacterID == apply.characterId && v.Result == 0) ;
            if (oldApply == null)
                return false;

            oldApply.Result = (int)apply.Result;
            if(apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
                DBService.Instance.Save();
            }
            this.timestamp = Time.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterID = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now
            };
            this.Data.Members.Add(dbMember);
            timestamp = Time.timestamp;
        }

        internal void Leave(Character character)
        {
            var member = this.Data.Members.FirstOrDefault(v => v.Id == character.Id);
            if (member != null)
                DBService.Instance.Entities.GuildMembers.Remove(member);
            this.timestamp = Time.timestamp;
        }

        internal void PostProcess(Character sender, NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(sender);
            }
        }

        internal NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)Time.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count
            };

            if (from != null)
            {//是公会成员才可以看到成员信息
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderID)//是会长才能看见申请信息
                    info.Applies.AddRange(GetApplyInfos());
            }
            return info;
        }

        private List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> list = new List<NGuildApplyInfo>();
            foreach (var info in this.Data.GuildApplies)
            {
                list.Add(new NGuildApplyInfo()
                {
                    characterId = info.CharacterID,
                    GuildId = info.GuildId,
                    Name = info.Name,
                    Class = info.Class,
                    Level = info.Level,
                    Result = (ApplyResult)info.Result,
                });
            }
            return list;
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach (var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterID,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)Time.GetTimestamp(member.JoinTime),
                    lastTime = (long)Time.GetTimestamp(member.LastTime),
                };
                var character = CharacterManager.Instance.GetCharacter(member.CharacterID);
                if (character != null)
                {   //如果在线 就更新信息
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                    if (member.Id == this.Data.LeaderID)
                        this.Leader = character;
                }
                else
                {   //不在线
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                    if (member.Id == this.Data.LeaderID)
                        this.Leader = null;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterID,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level
            };
        }
        /// <summary>
        /// 执行管理员命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="target"></param>
        /// <param name="id"></param>
        /// <exception cref="NotImplementedException"></exception>
        internal void ExecuteAdmin(GuildAdminCommand command, int target, int id)
        {
            throw new NotImplementedException();
        }

        internal void SetNotice(string notice)
        {
            DBService.Instance.Entities.Guilds.FirstOrDefault(v => v.Id == this.Id).Notice = notice;
            DBService.Instance.Save();
            this.timestamp = Time.timestamp;
        }
    }
}
