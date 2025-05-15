using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem, ISetItemInfo<NFriendInfo>
{
    public TMP_Text name;
    public TMP_Text level;
    public TMP_Text type;
    public TMP_Text status;

    public Image Bg;
    private Sprite normalBg;
    public Sprite selectedBg;

    private void Start()
    {
        normalBg = Bg.GetComponent<Sprite>();
    }

    public override void onSelected(bool selected)
    {
        this.Bg.overrideSprite = selected ? selectedBg : normalBg;
    }

    public NFriendInfo Info;

    //public void SetFriendInfo(NFriendInfo info)
    //{
    //    this.Info = info;
    //    if (this.name != null) this.name.text = this.Info.friendInfo.Name;
    //    if (this.level != null) this.level.text = this.Info.friendInfo.Level.ToString();
    //    if (this.type != null) this.type.text = this.Info.friendInfo.Class.ToString();
    //    if (this.status!= null) this.status.text = this.Info.Status == 1 ? "在线" : "离线";
    //}

    public void SetItemInfo(NFriendInfo info)
    {
        this.Info = info;
        if (this.name != null) this.name.text = this.Info.friendInfo.Name;
        if (this.level != null) this.level.text = this.Info.friendInfo.Level.ToString();
        if (this.type != null) this.type.text = this.Info.friendInfo.Class.ToString();
        if (this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : "离线";
    }
}
