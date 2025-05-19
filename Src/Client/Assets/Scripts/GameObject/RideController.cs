using SkillBridge.Message;
using UnityEngine;

public class RideController : MonoBehaviour
{
    public Transform mountPoint;//骑乘点
    public EntityController rider;
    public Vector3 offset;
    private Animator anim;

    void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    void Update()
    {
        if (this.mountPoint == null || this.rider == null) return;
        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    public void SetRide(EntityController ride)
    {
        this.rider = ride;
    }

    /// <summary>
    /// 坐骑的动画设置
    /// </summary>
    /// <param name="entityEvent"></param>
    /// <param name="param"></param>
    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EntityEvent.MoveFwd:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
        }
    }
}

