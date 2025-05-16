using Managers;
using Services;
using SkillBridge.Message;
using UnityEngine;

public class UIGuildApplyList : UIWindow
{
    public GameObject itemPrefab;
    public ListView listView;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        UpdateList();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    private void UpdateList()
    {
        ClearList();
        InitItems();
    }

    private void InitItems()
    {
        foreach (var info in GuildManager.Instance.guildInfo.Applies)
        {
            this.listView.AddItem<NGuildApplyInfo ,UIGuildApplyItem>(info, itemPrefab);
            //GameObject go = Instantiate(itemPrefab, listView.transform);
            //UIGuildApplyItem item = go.GetComponent<UIGuildApplyItem>();
            //item.SetItemInfo(info);
            //this.listView.AddItem(item);
        }
    }

    private void ClearList()
    {
        this.listView.RemoveAll();
    }
}
