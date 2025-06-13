using Battle;
using Models;
using UnityEngine;


public class UISkillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;


    public void UpdateSkills()
    {
        if (User.Instance.CurrentCharacter == null) return;
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        int skillIdx = 0;
        foreach (var skill in Skills)
        {
            slots[skillIdx++].SetSkill(skill);
        }
    }
}
