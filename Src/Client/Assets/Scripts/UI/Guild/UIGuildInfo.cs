using Common.Data;
using SkillBridge.Message;
using TMPro;
using UnityEngine;

public class UIGuildInfo : MonoBehaviour
{
    public TMP_Text guildName;
    public TMP_Text guildID;
    public TMP_Text guildPresident;

    public TMP_Text guildNotice;
    public TMP_Text memberCount;

    private NGuildInfo info = null;
    public NGuildInfo Info
    {
        get { return info; }
        set
        {
            this.info = value;
            this.UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (this.info == null)
        {
            this.guildName.text = "无";
            this.guildID.text = "ID: - ";
            this.guildPresident.text = "会长：无";
            this.guildNotice.text = "";
            this.memberCount.text = string.Format("成员数量：0/{0}", GameDefine.GuildMaxMemberCount);
        }
        else
        {
            this.guildName.text = this.info.GuildName;
            this.guildID.text = "ID:" + this.info.Id;
            this.guildPresident.text = "会长：" + this.info.leaderName;
            this.guildNotice.text = this.info.Notice;
            this.memberCount.text = string.Format("成员数量：{0}/{1}",this.info.memberCount , GameDefine.GuildMaxMemberCount);
        }
    }
}
