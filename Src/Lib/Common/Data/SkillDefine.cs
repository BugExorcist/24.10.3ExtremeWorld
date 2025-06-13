using Common.Battle;
using System.Collections.Generic;

namespace Common.Data
{
    public class SkillDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon  { get; set; }
        public SkillType Type { get; set; }
        public TargetType CastTarget { get; set; }
        public int UnlockLevel { get; set; }
        public float CastRange { get; set; }
        public float CastTime { get; set; }
        public float CD { get; set; }
        public float MPCost { get; set; }
        public bool Bullet { get; set; }
        public float BulletSpeed { get; set; }
        public string BulletResource { get; set; }
        public float AOERange { get; set; }
        public string SkillAnim { get; set; }
        /// <summary>
        /// 技能持续时间
        /// </summary>
        public float Duration { get; set; }
        /// <summary>
        /// 持续技能的触发间隔
        /// </summary>
        public float Interval { get; set; }
        /// <summary>
        /// 击中时间
        /// </summary>
        public List<float> HitTimes { get; set; }
        public List<int> Buff { get; set; }
        public float AD  { get; set; }
        public float AP  { get; set; }
        /// <summary>
        /// 物理攻击系数
        /// </summary>
        public float ADFactor { get; set; }
        /// <summary>
        /// 法术攻击系数
        /// </summary>
        public float APFactor { get; set; }
        
    }
}
