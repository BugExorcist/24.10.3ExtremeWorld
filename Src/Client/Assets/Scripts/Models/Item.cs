using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Item
    {
        public int Id;
        public int Count;

        public Item(NItemInfo intm)
        {
            this.Id = intm.Id;
            this.Count = intm.Count;
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.Id, this.Count);
        }
    }
}
