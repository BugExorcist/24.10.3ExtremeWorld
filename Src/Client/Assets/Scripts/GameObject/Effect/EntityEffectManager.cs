using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityEffectManager : MonoBehaviour
{
    public Transform Root;

    private Dictionary<string, GameObject> Effects = new Dictionary<string, GameObject>();

    public Transform[] Props;

    private void Start()
    {
        this.Effects.Clear();
        if (this.Root.childCount > 0)
        {
            for (int i = 0; i < this.Root.childCount; i++)
            {
                this.Effects[this.Root.GetChild(i).name] = this.Root.GetChild(i).gameObject;
            }
        }

        if (Props != null)
        {
            for (int i  = 0; i < this.Props.Length; i++)
            {
                this.Effects[this.Props[i].name] = this.Props[i].gameObject;
            }
        }
    }

    /// <summary>
    /// 播放（动画）时间帧 特效
    /// </summary>
    internal void PlayEffect(string name)
    {
        Debug.LogFormat("PalyEffect:{0} : {1}", this.name, name);
        if (this.Effects.ContainsKey(name))
        {
            this.Effects[name].SetActive(true);
        }
    }

    /// <summary>
    /// 播放 逻辑 特效
    /// </summary>
    internal void PlayEffect(EffectType type, string name, Transform target, float duration)
    {
        if (type == EffectType.Bullet)
        {
            EffectController effect = InstantiateEffect(name);
            effect.Init(type, this.transform, target, duration);
            effect.gameObject.SetActive(true);
        }
        else
        {
            PlayEffect(name);
        }
    }
    /// <summary>
    /// 实列化子弹的EffectController
    /// </summary>
    private EffectController InstantiateEffect(string name)
    {
        GameObject profab;
        if (this.Effects.TryGetValue(name, out profab))
        {
            GameObject go = Instantiate(profab, GameObjectManager.Instance.transform, true);
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
            return go.GetComponent<EffectController>();
        }
        return null;
    }
}
