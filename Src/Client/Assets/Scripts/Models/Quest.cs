using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Quest
    {
        public QuestDefine Define;
        public NQuestInfo Info;//如果一个任务还没有接取，那么这个任务没有网络信息

        public Quest() { }

        public Quest(NQuestInfo info)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.Info = null;
        }

        //public string GetTypeName()
        //{
        //    return EnumUtil.GetEnumDescription(this.Define.Type);
        //}
    }
}
