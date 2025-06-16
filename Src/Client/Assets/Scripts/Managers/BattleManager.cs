using Battle;
using Entities;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public class BattleManager : Singleton<BattleManager>
    {
        public delegate void TargetChangedHandler(Creature target);
        public event TargetChangedHandler OnTargetChanged;


        private Creature currentTarget;
        /// <summary>
        /// 目标类技能的目标
        /// </summary>
        public Creature CurrentTarget
        {
            get { return currentTarget; }
            set
            {
                this.SetTarget(value);
            }
        }
      
        private NVector3 currentPosition;
        /// <summary>
        /// 位置类技能的位置
        /// </summary>
        public NVector3 CurrentPosition
        {
            get { return currentPosition; }
            set
            {
                this.SetPosition(value);
            }
        }

        public void Init()
        {

        }

        private void SetTarget(Creature target)
        {
            if (this.currentTarget != target)
            {
                this.currentTarget = target;
                Debug.LogFormat("BattleManager.SetTarget[{0}:{1}]", target.entityId, target.Name);
                OnTargetChanged?.Invoke(target);
            }
        }

        private void SetPosition(NVector3 position)
        {
            this.currentPosition = position;
            Debug.LogFormat("BattleManager.SrtPosition[{0}]", position);
        }

        public void CastSkill(Skill skill)
        {
            int target = currentTarget != null ? currentTarget.entityId : 0;
            BattleService.Instance.SendSkillCast(skill.Define.ID, skill.Owner.entityId, target, currentPosition);
        }
    }
}
