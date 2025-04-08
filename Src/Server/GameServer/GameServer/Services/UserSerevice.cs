using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
            
        }

        

        public void Init()
        {

        }
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null || user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户或密码不正确.";
            }
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "成功登录.";

                // 更新NUserInfo 和 NPlayerInfo
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;//Entity ID
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.Tid = c.ID;//存放DB ID
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            sender.SendResponse();
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userRegister = new UserRegisterResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "注册成功！";
            }

            sender.SendResponse();
        }

        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                MapID = 1,
                MapPosX = 5000,//初始出生位置
                MapPosY = 4000,
                MapPosZ = 810,
                Gold = 100000,//初始金币10w
            };
            //背包初始化
            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.TCharacterBags.Add(bag);

            character = DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";

            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = 0;//用来存放Entity ID
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.Tid = c.ID;//存放DB ID
                sender.Session.Response.createChar.Characters.Add(info);
            }

            sender.SendResponse();
        }

        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbcharacter = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterId:{0}:Name:{1} Map{2} POS({3},{4},{4})", dbcharacter.ID, dbcharacter.Name, dbcharacter.MapID, dbcharacter.MapPosX, dbcharacter.MapPosY, dbcharacter.MapPosZ);
            Character character = CharacterManager.Instance.AddCharacter(dbcharacter);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";

            sender.Session.Response.gameEnter.Character = character.Info;

            //道具系统测试===============
            int itemId = 1;
            bool HasItem = character.ItemManager.HasItem(itemId);
            Log.InfoFormat("HasItem:[{0}],{1}", itemId, HasItem);
            if (HasItem)
            {
                //character.ItemManager.RemoveItem(itemId, 1);

            }
            else
            {
                character.ItemManager.AddItem(1, 200);
                character.ItemManager.AddItem(2, 100);
                character.ItemManager.AddItem(3, 30);
                character.ItemManager.AddItem(4, 120);
            }
            Models.Item item = character.ItemManager.GetItem(itemId);
            Log.InfoFormat("Item[{0}][{1}]", itemId, item);

            DBService.Instance.Save();

            sender.SendResponse();
            //角色进入
            sender.Session.Character = character;
            MapManager.Instance[dbcharacter.MapID].CharacterEnter(sender, character);
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterId:{0}:Name:{1} Map{2} POS{3} ", character.Id, character.Info.Name, character.Info.mapId, character.Position.ToString());
            //角色离开
            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            sender.SendResponse();
        }

        public void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoceCharacter(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}