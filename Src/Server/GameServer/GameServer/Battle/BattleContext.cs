using GameServer.Core;
using GameServer.Entities;
using SkillBridge.Message;

namespace GameServer.Battle
{
    public class BattleContext
    {
        public Battle Battle;
        public Creature Caster;
        public Creature Target;
        public Vector3Int Position;

        public NSkillCastInfo CastSkill;

        public SkillResult Result;

        public BattleContext(Battle battle)
        {
            this.Battle = battle;
        }
    }
}
