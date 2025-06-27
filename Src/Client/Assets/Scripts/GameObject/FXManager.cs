using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoSingleton<FXManager>
{
    public GameObject[] prefabs;

    private Dictionary<string, GameObject> Effects = new Dictionary<string, GameObject>();

    protected override void OnStart()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i].SetActive(false);
            this.Effects[this.prefabs[i].name] = this.prefabs[i];
        }
    }

    /// <summary>
    /// 实列化特效
    /// </summary>
    EffectController CreateEffect(string name, Vector3 position)
    {
        GameObject prefab;
        if (this.Effects.TryGetValue(name, out prefab))
        {
            GameObject go = Instantiate(prefab, FXManager.Instance.transform, true);
            go.transform.position = position;
            return go.GetComponent<EffectController>();
        }
        return null;
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    internal void PlayEffect(EffectType type, string name, Transform target, Vector3 pos, float duration)
    { 
        EffectController effect = FXManager.Instance.CreateEffect(name, pos);
        if (effect == null)
        {
            Debug.LogFormat("Effect:{0} not found!", name);
        }
        effect.Init(type, this.transform, target, pos, duration);
        effect.gameObject.SetActive(true);
    }
}
