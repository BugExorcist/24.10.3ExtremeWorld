using Common.Data;
using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;

        public event Action OnUpdataGold;

        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        public void AddGold(int value)
        {
            this.CurrentCharacterInfo.Gold += value;
            //通知UI刷新
            OnUpdataGold?.Invoke();
        }

        public MapDefine CurrentMapData { get; set; }

        public Character CurrentCharacter { get; set; }

        public NCharacterInfo CurrentCharacterInfo { get; set; }

        public PlayerInputController CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public int CurrentRide = 0;
        public void Ride(int rideId)
        {
            if (CurrentRide != rideId)
            {
                this.CurrentRide = rideId;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                this.CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
        }

        public delegate void CharacterInitHandle();
        public event CharacterInitHandle OnCharacterInit;

        public void CharacterInited()
        {
            OnCharacterInit?.Invoke();
        }
    }
}
