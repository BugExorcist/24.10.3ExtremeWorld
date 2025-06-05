using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer.Battle
{
    public class SkillManager
    {
        private Creature Owner;
        public List<Skill> Skills { get; private set; }
        public List<NSkillInfo> Infos { get; private set;}


        public SkillManager(Creature owner)
        {
            this.Owner = owner;
            this.Skills = new List<Skill>();
            this.Infos = new List<NSkillInfo>();
            this.InitSkills();
        }

        private void InitSkills()
        {
            this.Skills.Clear();
            this.Infos.Clear();

            //  添加 数据库读取技能信息


            if (!DataManager.Instance.Skills.ContainsKey(this.Owner.Define.TID)) return;

            foreach(var define in DataManager.Instance.Skills[this.Owner.Define.TID])
            {
                NSkillInfo info = new NSkillInfo();
                info.Id = define.Key;
                if (define.Value.UnlockLevel > this.Owner.Info.Level)
                {
                    info.Level = 1;
                }
                else
                {
                    info.Level = 5;
                    //info.Level =  DBService.Instance.Entities.CharacterSkills.FirstOrDefault(v => v.SkillID == define.Value.ID && v.Owner.ID == this.Owner.Id).Level;
                }
                this.Infos.Add(info);
                this.Skills.Add(new Skill(info, this.Owner));
            }
        }

        public void AddSkill(Skill skill)
        {
            this.Skills.Add(skill);
        }
    }
}
