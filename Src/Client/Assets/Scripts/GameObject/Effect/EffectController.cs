using UnityEngine;
using System.Collections;

internal class EffectController : MonoBehaviour
{
    public float lifeTime = 1.0f;
    float time = 0;

    EffectType type;
    Transform target;

    Vector3 targetPos;
    Vector3 startPos;
    Vector3 offset;

    private void OnEnable()
    {
        if (type != EffectType.Bullet)
        {
            StartCoroutine(Run());
        }
    }
    /// <summary>
    /// 不是子弹的效果
    /// </summary>
    IEnumerator Run()
    { 
        yield return new WaitForSeconds(this.lifeTime);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化子弹特效
    /// </summary>
    /// <param name="type">特效类型</param>
    /// <param name="source">子弹来源</param>
    /// <param name="target">子弹目标</param>
    /// <param name="offset">目标角色身高</param>
    /// <param name="duration">子弹飞行时间</param>
    internal void Init(EffectType type, Transform source, Transform target, Vector3 offset, float duration)
    {
        this.type = type;
        this.target = target;
        if (duration > 0)
            this.lifeTime = duration;
        this.time = 0;
        if (type == EffectType.Bullet)
        {
            this.startPos = this.transform.position;
            this.offset = offset;//身高
            this.targetPos = target.position + offset;
        }
        else if (type == EffectType.Hit)
        {
            this.transform.position = target.position + offset;
        }
    }

    private void Update()
    {
        if (this.type == EffectType.Bullet)
        {
            this.time += Time.deltaTime;
            if (this.target != null)
            {   // 如果目标还在，矫正位置
                this.targetPos = this.target.position + this.offset;
            }
            this.transform.LookAt(this.targetPos);
            if (Vector3.Distance(this.targetPos, this.transform.position) < 0.5f)
            {
                Destroy(this.gameObject);
                return;
            }
            if (this.lifeTime > 0 && this.time >= this.lifeTime)
            {   //如果子弹超出了设定的生存周期也要销毁
                Destroy(this.gameObject);
                return;
            }
            this.transform.position = Vector3.Lerp(this.transform.position, this.targetPos, Time.deltaTime / (this.lifeTime - this.time));
            /*
             为什么使用①(Time.deltaTime / (lifeTime - time)) 而不是 ②(time / lifeTime)  ？
            ①的子弹速度逐渐加快，确保在 lifeTime 结束时正好到达目标。
            ②的子弹会从 startPos 到 targetPos 匀速移动。
            ①的优点：1.如果目标移动，targetPos 会变化，①可以动态调整速度，即使目标移动，子弹仍会在 lifeTime 内到达最新位置。
            2.①的初始t小，子弹起步速度慢，更符合某些游戏弹道的物理感觉，②可能会显得不自然。
             */
        }
    }
}
