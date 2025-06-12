using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    internal class Bullet
    {
        private Skill skill;
        private Creature target;
        // 此消息会发送两次 第一次是技能释放消息（isBullet = false） 第二次是技能命中消息（isBullet = true）
        NSkillHitInfo hitInfo;
        bool TimeMode = true;
        //子弹飞行总时间
        float duration = 0;
        //子弹当前飞行时间
        float flyTime = 0;
        //是否已经停止
        public bool Stoped = false;

        public Bullet(Skill skill, Creature target, NSkillHitInfo hitInfo)
        {
            this.skill = skill;
            this.target = target;
            this.hitInfo = hitInfo;
            int distance = skill.Owner.Distance(target);
            if (TimeMode)
            {
                duration = distance / this.skill.Define.BulletSpeed;
            }
            Log.InfoFormat("Bullet[{0}].CastBullet[{1}] Target:{2} Distance:{3} Time:{4}", this.skill.Define.Name, this.skill.Define.BulletResource, distance, this.duration);
        }

        public void Update()
        {
            if (this.Stoped) return;
            if (TimeMode)
            {
                this.UpdateTime();
            }
            else
            {
                this.UpdatePos();
            }
        }
        private void UpdateTime()
        {
            this.flyTime += Time.deltaTime;
            if (this.flyTime > duration)
            {
                this.hitInfo.isBullet = true;
                this.skill.DoHit(this.hitInfo);
                this.Stoped = true;
            }
        }

        private void UpdatePos()
        {
            /*
             int distance = skill.Owner.Distance(target);
            if (distance > 50)
            {
                pos += speed + Time.deltaTime;
            }
            else
            {
                this.hitInfo.isBullet = true;
                this.skill.DoHit(this.hitInfo);
                this.toped = true;
            }

             */
        }
    }
}
