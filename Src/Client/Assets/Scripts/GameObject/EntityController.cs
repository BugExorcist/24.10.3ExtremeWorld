using Entities;
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour, IEntityNotify, IEntityController
{

    public Animator anim;
    public Rigidbody rb;
    private AnimatorStateInfo currentBaseState;

    public Entity entity;

    public Vector3 position;
    public Vector3 direction;
    Quaternion rotation;

    public Vector3 lastPosition;
    Quaternion lastRotation;

    public float speed;
    public float animSpeed = 1.5f;
    public float jumpPower = 3.0f;

    public bool isPlayer = false;

    public RideController rideController;
    private int currentRide = 0;
    public Transform rideBone;//用来绑定骑乘的骨骼


    void Start()
    {
        if (entity != null)
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            this.UpdateTransform();
        }

        if (!this.isPlayer)
            rb.useGravity = false;
    }

    void UpdateTransform()
    {
        this.position = GameObjectTool.LogicToWorld(entity.position);
        this.direction = GameObjectTool.LogicToWorld(entity.direction);

        this.rb.MovePosition(this.position);
        this.transform.forward = this.direction;
        this.lastPosition = this.position;
        this.lastRotation = this.rotation;
    }

    void OnDestory()
    {
        if (entity != null)
        {
            Debug.LogFormat("{0} OnDestroy :ID:{1} POS:{2} DIR:{3} SPD:{4} ", this.name, entity.entityId, entity.position, entity.direction, entity.speed);
        }
        if(UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    void FixedUpdate()
    {
        if(this.entity == null)
            return;

        this.entity.OnUpdate(Time.fixedDeltaTime);

        if(!this.isPlayer)
        {
            this.UpdateTransform();
        }
    }

    public void OnEntityRemoved()
    {
        if(UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
        Destroy(this.gameObject);
    }

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
            case EntityEvent.MoveBack:
                anim.SetBool("Move", true);
                break;
            case EntityEvent.Jump:
                anim.SetTrigger("Jump");
                break;
            case EntityEvent.Ride:
                    this.Ride(param);
                break;
        }
        if (this.rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }

    /// <summary>
    /// 骑乘或者取消骑乘
    /// </summary>
    /// <param name="rideID">0-取消骑乘  其他-对应坐骑的ID</param>
    public void Ride(int rideID)
    {
        if (currentRide == rideID) return;
        currentRide = rideID;
        if(rideID > 0)
        {
            this.rideController = GameObjectManager.Instance.LoadRide(rideID, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }

        if (this.rideController == null)
        {
            this.anim.transform.localPosition = Vector3.zero;
            this.anim.SetLayerWeight(1, 0);
        }
        else
        {
            this.rideController.SetRide(this);
            this.anim.SetLayerWeight(1, 1);
        }
    }

    public void SetRidePosition(Vector3 position)
    {
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }

    public void OnEntityChanged(Entity entity)
    {
        Debug.LogFormat("OnEntityChanged : ID:{0} POS:{1} DIR:{2} SPD: {3}", entity.entityId, entity.position, entity.direction, entity.speed);


    }

    public void PlayAnim(string name)
    {
        this.anim.SetTrigger(name);
    }

    public void SetStandBy(bool standby)
    {
        this.anim.SetBool("Standby", standby);
    }

    private void OnMouseDown()
    {
        BattleManager.Instance.CurrentTarget = this.entity as Creature;
    }
}
