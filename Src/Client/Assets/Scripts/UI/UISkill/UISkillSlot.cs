using Common.Data;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using Batttle;
using Common.Battle;

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image overlay;
    public TMP_Text cdText;
    private Skill skill;

    float overlaySpeed = 0;
    float cdRemain = 0;//cd剩余时间

    private void Update()
    {
        if (this.overlay.fillAmount > 0)
        {
            overlay.fillAmount = this.cdRemain / this.skill.Define.CD;
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
        SkillResult result = skill.CanCast();
        switch (result)
        {
            case SkillResult.InvalidTarget:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 目标无效");
                return;
            case SkillResult.OutOfMP:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 释放MP不足");
                return;
            case SkillResult.Cooldown:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 正在冷却中");
                return;

        }
        MessageBox.Show("释放技能" + this.skill.Define.Name);
        this.SetCD(this.skill.Define.CD);
        this.skill.Cast();
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

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        if (this.icon != null) this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
        this.SetCD(this.skill.Define.CD);
    }
}
