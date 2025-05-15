using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();
        public void Init()
        {
            this.Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.Guilds)
            {
                this.AddGuid(new Guild(guild));
            }
        }

        private void AddGuid(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = Time.timestamp;
        }

        internal bool CheckNameExisted(string guildName)
        {
            return GuildNames.Contains(guildName);
        }

        internal void CreateGuild(string guildName, string guildNotice, Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = guildName;
            dbGuild.Notice = guildNotice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.Data.Class, leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuid(guild);
        }

        internal Guild GetGuild(int guildId)
        {
            if (guildId == 0)
                return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }
        /// <summary>
        /// 获取当前所有公会信息
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> guilds = new List<NGuildInfo>();
            foreach (var guild in this.Guilds.Values)
            {   //不是公会成员不能获取所有信息
                guilds.Add(guild.GuildInfo(null));
            }
            return guilds;
        }
    }
}
