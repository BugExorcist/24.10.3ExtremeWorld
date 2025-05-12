using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager
    {
        Character Owner;
        List<NFriendInfo> friends = new List<NFriendInfo>();
        bool friendChanged = false;
        public FriendManager(Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }
        public void GetFriendInfos(List<NFriendInfo> friends)
        {
            foreach (var friend in this.friends)
            {
                friends.Add(friend);
            }
        }

        private void InitFriends()
        {
            this.friends.Clear();
            foreach(var friend in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendId = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        public bool RemoveFriendByFriendId(int friendId)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendId == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        public bool RemoveFriendByID(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.TCharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendId);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if (character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendId;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;

                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            Log.InfoFormat("{0}:{1} GetFriendInfo : {2}:{3} Statu: {4}", this.Owner.Id, this.Owner.Info.Name, friendInfo.friendInfo.Id, friendInfo.friendInfo.Name, friendInfo.Status);
            return friendInfo;
        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == friendId)
                {
                    return f;
                }
            }
            return null;
        }

        public void UpdateFriendInfo(NCharacterInfo info, int status)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == info.Id)
                {
                    f.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }
        /// <summary>
        /// 离线通知
        /// </summary>
        public void OfflineNotify()
        {
            foreach (var friendInfo in this.friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if (friend != null)
                    friend.FriendManager.UpdateFriendInfo(this.Owner.Info, 0);
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                Log.InfoFormat("PostProcess > FriendManager: characterID:{0}:{1}",this.Owner.Id, this.Owner.Info.Name);
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false;
            }
        }
    }
}
