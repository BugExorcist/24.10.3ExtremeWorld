using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UIRideItem : ListView.ListViewItem
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

    public Item item;

    public void SetItemInfo(Item item, UIRide owner, bool equiped)
    {
        this.item = item;

        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = "Lv." + this.item.Define.Level.ToString();
        if (this.icon  != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }
}
