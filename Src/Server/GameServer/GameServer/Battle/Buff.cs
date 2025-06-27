using Common;
using Common.Data;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    public class Buff
    {
        public int BuffId;
        public bool Stoped { get; internal set; }
        private Creature Owner;
        private BuffDefine Define;
        private BattleContext Context;
        private float time = 0;
        private int hit = 0;


        public Buff(int buffID, Creature owner, BuffDefine buffDefine, BattleContext context)
        {
            this.BuffId = buffID;
            this.Owner = owner;
            this.Define = buffDefine;
            this.Context = context;
            this.Stoped = false;

            this.OnAdd();
        }


        private void OnAdd()
        {
            if (this.Define.Effect != Common.Battle.BuffEffect.None)
            {
                this.Owner.EffectMgr.AddEffect(this.Define.Effect);
            }

            AddAttr();

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this.Define.ID,
                casterId = this.Context.Caster.entityId,
                ownerId = this.Owner.entityId,
                Action = BuffAction.Add,
            };
            Context.Battle.AddBuffAction(buff);
        }

        private void OnRemove()
        {
            RemoveAttr();
            Stoped = true;
            if (this.Define.Effect != Common.Battle.BuffEffect.None)
            {
                this.Owner.EffectMgr.RemoveEffect(this.Define.Effect);
            }

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this.Define.ID,
                casterId = this.Context.Caster.entityId,
                ownerId = this.Owner.entityId,
                Action = BuffAction.Remove,
            };
            Context.Battle.AddBuffAction(buff);
        }
        private void AddAttr()
        {
            if (this.Define.DEFRantio != 0)
            {
                this.Owner.Attributes.Buff.DEF += this.Owner.Attributes.Basic.DEF * this.Define.DEFRantio;
                this.Owner.Attributes.InitFinalAttributes();
            }
        }

        private void RemoveAttr()
        {
            if (this.Define.DEFRantio != 0)
            {
                this.Owner.Attributes.Buff.DEF -= this.Owner.Attributes.Basic.DEF * this.Define.DEFRantio;
                this.Owner.Attributes.InitFinalAttributes();
            }
        }

        internal void Update()
        {
            if (Stoped) return;
            this.time += Time.deltaTime;

            if (this.Define.Interval > 0)
            {   // 有间隔时间
                if (this.time > this.Define.Interval * (this.hit + 1))
                {
                    this.DoBuffDamage();
                }
            }

            if (this.time > this.Define.Duration)
            {
                this.OnRemove();
            }
        }
        private NDamageInfo CalcBuffDamage(Creature caster)
        {
            float ad = this.Define.AD + caster.Attributes.AD * this.Define.ADFactor;
            float ap = this.Define.AP + caster.Attributes.AP * this.Define.APFactor;

            float addmg = ad * (1 - Owner.Attributes.DEF / (Owner.Attributes.DEF + 100));
            float apdmg = ap * (1 - Owner.Attributes.MDEF / (Owner.Attributes.MDEF + 100));

            float finalDmg = addmg + apdmg;

            NDamageInfo damage = new NDamageInfo();
            damage.entityId = Owner.entityId;
            damage.Damage = Math.Max(1, (int)finalDmg);
            return damage;
        }

        private void DoBuffDamage()
        {
            this.hit++;

            NDamageInfo damage = this.CalcBuffDamage(Context.Caster);
            Log.InfoFormat("Buff[{0}].DoBuffDamage[{1}] Damage:{2} Crit:{3}", this.Define.Name, this.Owner.Name, damage.Damage, damage.Crit);
            this.Owner.DoDamage(damage, Context.Caster);

            NBuffInfo buff = new NBuffInfo()
            {
                buffId = this.BuffId,
                buffType = this.Define.ID,
                casterId = this.Context.Caster.entityId,
                ownerId = this.Owner.entityId,
                Action = BuffAction.Hit,
                Damage = damage,
            };
            Context.Battle.AddBuffAction(buff);
        }
    }
}
