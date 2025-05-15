using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow
{
    public GameObject itemPrefab;
    public ListView listView;
    public UIGuildInfo uiInfo;
    private UIGuildItem selectedItem;

    private void Start()
    {
        this.listView.onItemSelected += this.OnItemSelected;
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.SendGuildListRequest();
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    private void InitItems(List<NGuildInfo> guilds)
    {
        foreach (var info in guilds)
        {
            this.listView.AddItem<NGuildInfo, UIGuildItem>(info, itemPrefab);
            //GameObject go = Instantiate(itemPrefab, listView.transform);
            //UIGuildItem item = go.GetComponent<UIGuildItem>();
            //item.SetGuildInfo(info);
            //this.listView.AddItem(item);
        }
    }

    private void ClearList()
    {
        this.listView.RemoveAll();
    }

    public void OnClickJoin()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要加入的公会");
            return;
        }
        MessageBox.Show(string.Format("确定要加入公会【{0}】吗？", selectedItem.Info.GuildName), "申请加入工会", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);
        };
    }
}
