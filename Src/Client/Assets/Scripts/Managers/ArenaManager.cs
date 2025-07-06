using Common.Data;
using Models;
using Services;
using SkillBridge.Message;

namespace Managers
{
    internal class ArenaManager : Singleton<ArenaManager>
    {
        private ArenaInfo ArenaInfo;
        public int Round = 0;
        public int ArenaTeam = 0; // 0 = red team, 1 = blue team
        internal void Init()
        {

        }


        internal void EnterArena(ArenaInfo arenaInfo)
        {
            this.ArenaInfo = arenaInfo;
            User.Instance.CurrentCharacter.ready = false;
        }

        internal void ExitArena(ArenaInfo arenaInfo)
        {
            this.ArenaInfo = null;
        }

        internal void SendReady()
        {
            ArenaService.Instance.SendArenaReadyRequest(this.ArenaInfo.ArenaId);
        }

        internal void OnReady(int round, ArenaInfo arenaInfo)
        {
            this.Round = round;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowCountDown();
            User.Instance.CurrentCharacter.ready = false;
        }

        internal void OnRoundStart(int round, ArenaInfo arenaInfo)
        {
            User.Instance.CurrentCharacter.ready = true;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundStart(round, arenaInfo);
            User.Instance.CurrentCharacter.ready = true;
            User.Instance.CurrentCharacter.Attributes.RecoverHPAndMP();
        }


        internal void OnRoundEnd(int round, ArenaInfo arenaInfo)
        {
            User.Instance.CurrentCharacter.ready = false;
            if (this.ArenaInfo.Red.EntityId == User.Instance.CurrentCharacter.entityId)
            {
                TeleporterDefine redPoint = DataManager.Instance.Teleporters[9];
                User.Instance.CurrentCharacterObject.transform.position = GameObjectTool.LogicToWorld(redPoint.Position);
            }
            else
            {
                TeleporterDefine bluePoint = DataManager.Instance.Teleporters[10];
                User.Instance.CurrentCharacterObject.transform.position = GameObjectTool.LogicToWorld(bluePoint.Position);
            }
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundResult(round, arenaInfo);
        }
    }
}
