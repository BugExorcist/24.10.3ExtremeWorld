using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class BattleManager : Singleton<BattleManager>
    {
        static long bid = 0;

        public void Init()
        {

        }

        public void ProcessBattleMessage(NetConnection<NetSession> sender, SkillCastRequest request)
        {
            Log.InfoFormat("BattleManager.ProcessBattleMessage: skill:{0} caster:{1} target:{2} pos:{3}", request.castInfo.skillId, request.castInfo.casterId, request.castInfo.targetId, request.castInfo.Position.String());
            Creature character = sender.Session.Character;
            Battle.Battle battle = MapManager.Instance[character.Info.mapId].Battle;

            battle.ProcessBattleMessage(sender, request);
        }
    }
}
