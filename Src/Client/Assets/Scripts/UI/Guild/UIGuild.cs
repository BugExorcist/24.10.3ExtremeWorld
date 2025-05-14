using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listView;
    public UIGuildInfo uiInfo;
    private UIGuildItem selectedItem;

    private void Start()
    {
        this.listView.onItemSelected += this.OnItemSelected;
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        UpdateUI();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;
    }

    private void InitItems()
    {
        foreach (var info in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, listView.transform);
            UIGuildMemberItem item = go.GetComponent<UIGuildMemberItem>();
            item.SetGuildInfo(info);
            this.listView.AddItem(item);
        }
    }

    private void ClearList()
    {
        this.listView.RemoveAll();
    }

    public void OnClickApplyList()
    {
       
    }
    public void OnClickLeave()
    {

    }
    public void OnClickChat()
    {

    }
    public void OnClickKickOut()
    {

    }
    public void OnClickPromote()
    {

    }
    public void OnClickDepose()
    {

    }
    public void OnClickTransfer()
    {

    }
    public void OnClickSetNotice()
    {

    }
}
