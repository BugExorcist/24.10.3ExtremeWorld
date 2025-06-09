using Entities;
using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Security.Cryptography;
using System.Xml.Linq;
using UnityEngine;

namespace Services
{
    public class BattleService : Singleton<BattleService>, IDisposable
    {
        public BattleService()
        {
            MessageDistributer.Instance.Subscribe<SkillCastResponse>(this.OnSkillCsat);
        }

        public void Init()
        {

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<SkillCastResponse>(this.OnSkillCsat);
        }
        
        public void SendSkillCast(int skillId, int casterId, int targetId, NVector3 position)
        {
            if (position == null) position = new NVector3();
            Debug.LogFormat("SendSkillCast:Skill:{0} caster:{1} target:{2} pos:{3}", skillId, casterId, targetId, position.ToString());
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.skillCast = new SkillCastRequest();
            message.Request.skillCast.castInfo = new NSkillCastInfo();
            message.Request.skillCast.castInfo.skillId = skillId;
            message.Request.skillCast.castInfo.casterId = casterId;
            message.Request.skillCast.castInfo.targetId = targetId;
            message.Request.skillCast.castInfo.Position = position;
            NetClient.Instance.SendMessage(message);
        }

        private void OnSkillCsat(object sender, SkillCastResponse message)
        {
            Debug.LogFormat("OnSkillCast:Skill:{0} caster:{1} target:{2} pos:{3} result:{4}", message.castInfo.skillId, message.castInfo.casterId, message.castInfo.targetId, message.castInfo.Position.ToString(), message.Result);
            if (message.Result == Result.Success)
            {
                Creature caster = EntityManager.Instance.GetEntity(message.castInfo.casterId) as Creature;
                if (caster != null)
                {
                    Creature target = EntityManager.Instance.GetEntity(message.castInfo.targetId) as Creature;
                    caster.CsatSkill(message.castInfo.skillId, target, message.castInfo.Position, message.Damage);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }

    }
}
