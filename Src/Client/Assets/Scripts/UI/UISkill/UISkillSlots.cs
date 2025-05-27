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
        var Skills = DataManager.Instance.Skills[(int)User.Instance.CurrentCharacterInfo.Class];
        int skillIdx = 0;
        foreach (var kv in Skills)
        {
            slots[skillIdx++].SetSkill(kv.Value);
        }
    }
}
