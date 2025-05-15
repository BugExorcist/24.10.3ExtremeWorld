using Services;
using SkillBridge.Message;
using TMPro;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem, ISetItemInfo<NGuildApplyInfo>
{
    public NGuildApplyInfo Info;
    public TMP_Text name;
    public TMP_Text level;
    public TMP_Text type;
    public Button accept;
    public Button refuse;


    private void Start()
    {
        this.accept.onClick.AddListener(this.OnAccept);
        this.refuse.onClick.AddListener(this.OnRefese);
    }

    public void SetItemInfo(NGuildApplyInfo info)
    {
        this.Info = info;
        if (this.name != null) this.name.text = this.Info.Name;
        if (this.level != null) this.level.text = this.Info.Level.ToString();
        if (this.type != null) this.type.text = this.Info.Class.ToString();
    }

    public void OnAccept()
    {
        MessageBox.Show(string.Format("确定要通过【{0}】的申请吗？", this.Info.Name), "同意申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }


    public void OnRefese()
    {
        MessageBox.Show(string.Format("确定要拒绝【{0}】的申请吗？", this.Info.Name), "拒绝申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}

