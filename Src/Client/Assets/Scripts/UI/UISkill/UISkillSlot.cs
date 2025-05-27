using Common.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image overlay;
    public TMP_Text cdText;
    private SkillDefine skill;

    float overlaySpeed = 0;
    float cdRemain = 0;//cd剩余时间

    private void Update()
    {
        if (this.overlay.fillAmount > 0)
        {
            overlay.fillAmount = this.cdRemain / this.skill.CD;
            this.cdText.text = ((int)Math.Ceiling(this.cdRemain)).ToString();
            this.cdRemain -= Time.deltaTime;
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (cdText.enabled) cdText.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.overlay.fillAmount > 0)
        {
            MessageBox.Show("技能：" + this.skill.Name + " 正在冷却中");
        }
        else
        {
            MessageBox.Show("释放技能" + this.skill.Name);
            this.SetCD(this.skill.CD);
        }
    }

    private void SetCD(float cd)
    {
        if (!overlay.enabled) overlay.enabled = true;
        if (!cdText.enabled) cdText.enabled = true;
        this.cdText.text = ((int)Math.Floor(this.cdRemain)).ToString();
        overlay.fillAmount = 1f;
        overlaySpeed = 1f / cd;
        cdRemain = cd;
    }

    public void SetSkill(SkillDefine define)
    {
        this.skill = define;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Icon);
        this.SetCD(this.skill.CD);
    }
}
