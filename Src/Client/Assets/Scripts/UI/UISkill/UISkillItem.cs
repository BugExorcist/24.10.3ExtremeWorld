using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Common.Data;

public class UISkillItem : ListView.ListViewItem
{
    public Image icon;
    public TMP_Text title;
    public TMP_Text level;

    public Image background;
    private Sprite normalbg;
    public Sprite selectedbg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedbg : normalbg;
    }

    private void Start()
    {
        this.normalbg = background.GetComponent<Sprite>();
    }

    public SkillDefine item;

    public void SetItemInfo(SkillDefine item, UISkill owner, bool equiped)
    {
        this.item = item;

        if (this.title != null) this.title.text = this.item.Name;
        if (this.level != null) this.level.text = item.UnlockLevel.ToString();
        if (this.icon  != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Icon);
    }
}
