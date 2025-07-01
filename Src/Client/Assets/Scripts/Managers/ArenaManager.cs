using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    internal class ArenaManager : Singleton<ArenaManager>
    {
        private ArenaInfo ArenaInfo;
        public int Round = 0;

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
        }

        internal void OnRoundStart(int round, ArenaInfo arenaInfo)
        {
            User.Instance.CurrentCharacter.ready = true;
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundStart(round, arenaInfo);
        }
    

        internal void OnRoundEnd(int round, ArenaInfo arenaInfo)
        {
            if (UIArena.Instance != null)
                UIArena.Instance.ShowRoundResult(round, arenaInfo);
            //TODO: 初始化角色的位置到出生点，给角色恢复状态
        }
    }
}
