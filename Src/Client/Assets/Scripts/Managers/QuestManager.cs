using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None = 0,//无任务
        Complete,//拥有已完成任务
        Available,//拥有可接受任务
        Incomplete,//拥有未完成任务
    }

    class QuestManager : Singleton<QuestManager>
    {
        public List<NQuestInfo> questInfos;
        //任务ID-任务
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        //npcID-任务状态-任务列表
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        private void InitQuests()
        {
            //初始化已接任务
            foreach (var info in questInfos)
            {
                Quest quest = new Quest(info);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Info.QuestId] = quest;
            }
            //初始化可接任务
            foreach (var kv in DataManager.Instance.Quests)
            {
                if (kv.Value.LimitClass != User.Instance.CurrentCharacter.Class && kv.Value.LimitClass != CharacterClass.None)
                    continue;//不符合职业
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                    continue;//等级不够
                if (this.allQuests.ContainsKey(kv.Key))
                    continue;//已经接取

                if (kv.Value.PreQuest > 0)//有前置任务
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))
                    {
                        if (preQuest.Info == null)
                            continue;//前置任务未接取
                        if (preQuest.Info.Status != QuestStatus.Finished)
                            continue;//前置任务未完成
                    }
                    else
                        continue;
                }
                Quest quest = new Quest(kv.Value);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int acceptNPC, Quest quest)
        {
            //throw new NotImplementedException();
        }

        public NpcQuestStatus GetQuestStatusByNpc(int iD)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(iD, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return NpcQuestStatus.Complete;
                if (status[NpcQuestStatus.Available].Count > 0)
                    return NpcQuestStatus.Available;
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return NpcQuestStatus.Incomplete;
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int iD)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(iD, out status))
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                if (status[NpcQuestStatus.Available].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
            }
            return false;
        }

        bool ShowQuestDialog(Quest quest)
        {
            return false;
        }

        void OnQuestDialogClose(UIWindow sander, UIWindow.WindowResult result)
        {

        }
    }
}
