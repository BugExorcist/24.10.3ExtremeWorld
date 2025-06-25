using UnityEngine;

namespace Entities
{
    public interface IEntityController
    {
        Transform GetTransform();
        void PlayAnim(string name);
        void PlayEffect(EffectType type, string name, Creature target, float duration);
        void SetStandBy(bool standby);
        void UpdateDirection();
    }
}
