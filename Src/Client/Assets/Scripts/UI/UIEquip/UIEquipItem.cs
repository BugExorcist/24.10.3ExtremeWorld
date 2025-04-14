using Managers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TMP_Text title;
    public TMP_Text level;
    public TMP_Text limitClass;//限制职业
    public TMP_Text limitCategory;//限制种类

    public Image background;
    public Sprite nurmalBg;
    public Sprite selsctedBg;

    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            if (nurmalBg != null && selsctedBg != null)
                this.background.overrideSprite = selected ? selsctedBg : nurmalBg;
        }
    }

    public int index { get ; set; }
    private UICharEquip owner;
    private Item item;

    bool isEquiped;// false = 左边的列表item  true = 右边已装备的item
    public void SetEquipItem(int idx, Item item, UICharEquip owner, bool equiped)
    {
        this.owner = owner;
        this.item = item;
        this.index = idx;
        this.isEquiped = equiped;

        if (this.title != null) this.title.text = this.item.Define.Name;
        if (this.level != null) this.level.text = "Lv." + this.item.Define.Level.ToString();
        if (this.limitClass != null) this.limitClass.text = this.item.Define.LimitClass.ToString();
        if (this.limitCategory != null) this.limitCategory.text = item.Define.Category;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }

    private bool inUnEquip = false;//防止多次发送脱装备请求
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log(name + " Game Object Clicked!");
        if (this.isEquiped)
        {
            if (!inUnEquip)
            {
                inUnEquip = true;
                UnEquip();
            }
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;
            }
            else
            {
                this.Selected = true;
                this.owner.SelectItem(this);
            }
        }
    }

    private bool inselcet = false;
    private void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备[{0}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newmsg = MessageBox.Show(string.Format("要替换装备[{0}]吗？", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                newmsg.OnYes = () =>
                {
                    this.owner.DoEquip(this.item);
                };
            }
            else
                this.owner.DoEquip(this.item);
        };
    }

    private void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备[{0}]吗？", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            this.owner.UnEquip(this.item);
            this.inUnEquip = false;
        };
    }
}