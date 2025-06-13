using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    internal class Bullet
    {
        Skill skill;
        int hit = 0;
        float flyTime = 0;
        float duration = 0;

        public bool Stoped = false;

        public Bullet(Skill skill)
        {
            this.skill = skill;
            var target = skill.Target;
            this.hit = skill.Hit;
            int distance = skill.Owner.Distance(target);
            duration = distance / this.skill.Define.BulletSpeed;
        }

        /// <summary>
        /// 更新子弹（时间模式）
        /// </summary>
        public void Update()
        {
            if (this.Stoped) return;
            this.flyTime += Time.deltaTime;
            if (this.flyTime > duration)
            {
                this.skill.DoHitDamages(this.hit);
                this.Stoped = true;
            }
        }
    }
}
