using Common.Data;
using Managers;
using SkillBridge.Message;
using System.Collections.Generic;

namespace Entities
{
    public class Character : Creature
    {
        public Character(NCharacterInfo info) : base(info)
        {
        }

        public override List<EquipDefine> GetEquip()
        {
            return EquipManager.Instance.GetEquipDefines();
        }
    }
}
