using Models;
using UnityEngine;


public class UISkillSlots : MonoBehaviour
{
    public UISkillSlot[] slots;

    void Start()
    {
        RefeshUI();
    }

    public void RefeshUI()
    {
        var Skills = User.Instance.CurrentCharacter.SkillMgr.Skills;
        int skillIdx = 0;
        foreach (var skill in Skills)
        {
            slots[skillIdx++].SetSkill(skill);
        }
    }
}
