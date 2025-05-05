using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

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

        public UnityAction onQuestStatusChanged;

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
                this.allQuests[quest.Info.QuestId] = quest;
            }
            //初始化可接任务
            this.CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        private void CheckAvailableQuests()
        {
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
                this.allQuests[quest.Define.ID] = quest;
            }
        }

        private void AddNpcQuest(int npcId, Quest quest)
        {
            if (!this.npcQuests.ContainsKey(npcId))
                this.npcQuests[npcId] = new Dictionary<NpcQuestStatus, List<Quest>>();

            List<Quest> availables;//可接
            List<Quest> complates;//完成
            List<Quest> incomplates;//未完成

            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Available, out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Available] = availables;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Complete] = complates;
            }
            if (!this.npcQuests[npcId].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcId][NpcQuestStatus.Incomplete] = incomplates;
            }

            if (quest.Info == null)
            {   //未接
                if (npcId == quest.Define.AcceptNPC && !this.npcQuests[npcId][NpcQuestStatus.Available].Contains(quest))
                    this.npcQuests[npcId][NpcQuestStatus.Available].Add(quest);
            }
            else
            {   //已接
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.Completed)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Complete].Contains(quest))
                        this.npcQuests[npcId][NpcQuestStatus.Complete].Add(quest);
                }
                if (npcId == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcId][NpcQuestStatus.Incomplete].Contains(quest))
                        this.npcQuests[npcId][NpcQuestStatus.Incomplete].Add(quest);
                }
            }
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

        public bool OpenNpcQuest(int npcId)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcId, out status))
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
            if (quest.Info == null || quest.Info.Status == QuestStatus.Completed)
            {   //任务未接或者任务已经完成
                UIQuestDialog dialog = UIManager.Instance.Show<UIQuestDialog>();
                dialog.SetQuest(quest);
                dialog.OnClose += OnQuestDialogClose;
                return true;
            }
            if (quest.Info != null || quest.Info.Status == QuestStatus.Completed)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                    MessageBox.Show(quest.Define.DialogIncomplete);
            }
            return true;
        }

        void OnQuestDialogClose(UIWindow sander, UIWindow.WindowResult result)
        {
            UIQuestDialog dialog = (UIQuestDialog)sander;
            if (result == UIWindow.WindowResult.Yes)
            {
                if (dialog.quest.Info == null)
                    QuestService.Instance.SendQuestAccept(dialog.quest);
                else if (dialog.quest.Info.Status == QuestStatus.Completed)
                    QuestService.Instance.SendQuestSubmit(dialog.quest);
            }
            else if(result == UIWindow.WindowResult.No)
            {
                MessageBox.Show(dialog.quest.Define.DialogDeny);
            }
        }

        Quest RefershQuestStatus(NQuestInfo info)
        {
            this.npcQuests.Clear();
            Quest result;
            if (this.allQuests.ContainsKey(info.QuestId))
            {   //已接任务 更新任务状态
                this.allQuests[info.QuestId].Info = info;
                result = this.allQuests[info.QuestId];
            }
            else
            {   //未接任务
                result = new Quest(info);
                this.allQuests[info.QuestId] = result;
            }

            CheckAvailableQuests();

            foreach (var kv in this.allQuests)
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }

            onQuestStatusChanged?.Invoke();
            return result;
        }

        internal void OnQuestAccept(NQuestInfo info)
        {
            var quest = this.RefershQuestStatus(info);
            MessageBox.Show(quest.Define.DialogAccept);
        }

        internal void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefershQuestStatus(info);
            MessageBox.Show(quest.Define.DialogFinish);
        }
    }
}
