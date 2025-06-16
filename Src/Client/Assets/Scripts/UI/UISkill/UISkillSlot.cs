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

public class UISkillSlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Image overlay;
    public TMP_Text cdText;
    private Skill skill;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        overlay.enabled = false;
        cdText.enabled = false;
    }

    private void Update()
    {
        if (this.skill.CD > 0)
        {
            if (!overlay.enabled) overlay.enabled = true;
            if (!cdText.enabled) cdText.enabled = true;
            overlay.fillAmount = this.skill.CD / this.skill.Define.CD;
            this.cdText.text = ((int)Math.Ceiling(this.skill.CD)).ToString();
        }
        else
        {
            if (overlay.enabled) overlay.enabled = false;
            if (cdText.enabled) cdText.enabled = false;
        }
    }

    public void OnPositionSelected(Vector3 position)
    {
        BattleManager.Instance.CurrentPosition = GameObjectTool.WorldToLogicN(position);
        this.CastSkill();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.skill.Define.CastTarget == Common.Battle.TargetType.Position)
        {
            TargetSelector.ShowSelector(User.Instance.CurrentCharacter.position ,this.skill.Define.CastRange, this.skill.Define.AOERange, OnPositionSelected);
            return;
        }
        CastSkill();
    }

    private void CastSkill()
    {
        SkillResult result = skill.CanCast(BattleManager.Instance.CurrentTarget);
        switch (result)
        {
            case SkillResult.CoolDown:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 正在冷却中");
                return;
            case SkillResult.InvalidTarget:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 目标无效");
                return;
            case SkillResult.OutOfMp:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 释放MP不足");
                return;
            case SkillResult.OutOfRange:
                MessageBox.Show("技能：" + this.skill.Define.Name + " 目标超出技能范围");
                return;
        }
        BattleManager.Instance.CastSkill(this.skill);
    }

    public void SetSkill(Skill skill)
    {
        this.skill = skill;
        if (this.icon != null)
        {
            this.icon.overrideSprite = Resloader.Load<Sprite>(this.skill.Define.Icon);
            this.icon.SetAllDirty();
        }
        Init();
    }
}
