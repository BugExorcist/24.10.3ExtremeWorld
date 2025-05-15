using Models;
using SkillBridge.Message;

namespace Managers
{
    public class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo guildInfo;
        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild
        {
            get { return guildInfo != null; }
        }

        internal void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild != null)
            {
                myMemberInfo = null;
                return;
            }
            else
            {
                foreach(var member in guildInfo.Members)
                {
                    if (member.characterId == User.Instance.CurrentCharacter.Id)
                    {
                        myMemberInfo = member;
                        break;
                    }
                }
            }
        }

        internal void ShowGuild()
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if (result == UIWindow.WindowResult.Yes)
                UIManager.Instance.Show<UIGuildPopCreate>();
            else if(result == UIWindow.WindowResult.No)
                UIManager.Instance.Show<UIGuildList>();
        }

        public string GetTitle(GuildTitle title)
        {
            if(title == GuildTitle.President)
                return "会长";
            else if (title == GuildTitle.VicePresident)
                return "副会长";
            else
                return "普通成员";
        }
    }
}
