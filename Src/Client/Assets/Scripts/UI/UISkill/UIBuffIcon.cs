using Common.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Battle;
using SkillBridge.Message;
using Managers;
using Models;

public class UIBuffIcon : MonoBehaviour
{
    public Image icon;
    public Image overlay;
    public TMP_Text cdText;
    Buff buff;

    private void Start()
    {

    }


    private void Update()
    {
        if (this.buff == null) return;
        if (this.buff.time > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!cdText.enabled) cdText.enabled = true;
            overlay.fillAmount = this.buff.time / this.buff.Define.Duration;
            this.cdText.text = ((int)Math.Ceiling(this.buff.Define.Duration - this.buff.time)).ToString();
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (cdText.enabled) cdText.enabled = false;
        }
    }


    public void SetItem(Buff buff)
    {
        this.buff = buff;
        if (this.icon != null)
        {
            this.icon.overrideSprite = Resloader.Load<Sprite>(this.buff.Define.Icon);
            this.icon.SetAllDirty();
        }
    }
}
