using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class StatusManager
    {
        Character Owner;
        private List<NStatus> Status { get; set; }
        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }
        }

        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            this.Status.Add(new NStatus
            {
                Id = id,
                Value = value,
                Action = action,
                Type = type
            });
        }

        public void AddGoldChange(int goldDelta)
        {
            if(goldDelta > 0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            }
            if(goldDelta < 0)
            {
                this.AddStatus(StatusType.Money, 0, -goldDelta, StatusAction.Delete);
            }
        }

        public void AddExpChange(int v)
        {
            this.AddStatus(StatusType.Exp, 0, v, StatusAction.Add);
        }

        public void AddLevelChange(int v)
        {
            this.AddStatus(StatusType.Level, 0, v, StatusAction.Add);
        }

        public void AddItemChange(int id, int count, StatusAction action)
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.statusNotify == null)
                message.statusNotify = new StatusNotify();
            foreach(var statu in this.Status)
            {
                message.statusNotify.Status.Add(statu);
            }
            this.Status.Clear();
        }
    }
}
