using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;

namespace GameServer.Services
{
    class BagService : Singleton<BagService>
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        public void Init()
        {

        }

        private void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest request)
        {
            Character character = sender.Session.Character;
            sender.Session.Response.bagSave = new BagSaveResponse();

            if (character == null || request == null || request.BagInfo == null)
            {
                sender.Session.Response.bagSave.Result = Result.Failed;
                sender.Session.Response.bagSave.Errormsg = "BagSaveRequest invalid";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("DagSaveRequest: character:{0} Unlocked:{1}", character.Id, request.BagInfo.Unlocked);

            character.Data.Bag.Items = request.BagInfo.Items;
            character.Data.Bag.Unlocked = request.BagInfo.Unlocked;
            DBService.Instance.Save();

            sender.Session.Response.bagSave.Result = Result.Success;
            sender.SendResponse();
        }
    }
}
