using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    public EntityEffectManager EffctManager;

    void PlayEffect(string name)
    {
        Debug.LogFormat("AnimationEventController£ºPalyeffect:{0} : {1}", this.name, name);
        EffctManager.PlayEffect(name);
    }

    void PalySound(string name)
    {
        Debug.LogFormat("AnimationEventController£ºPalySound:{0} : {1}", this.name, name);
    }
}
