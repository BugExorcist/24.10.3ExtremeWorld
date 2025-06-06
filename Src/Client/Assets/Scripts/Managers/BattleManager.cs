using Batttle;
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
        private Creature currentTarget;
        public Creature CurrentTarget
        {
            get { return currentTarget; }
            set
            {
                this.SetTarget(value);
            }
        }
      
        private NVector3 currentPosition;
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
            this.currentTarget = target;
            Debug.LogFormat("BattleManager.SetTarget[{0}:{1}]", target.entityId, target.Name);
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
