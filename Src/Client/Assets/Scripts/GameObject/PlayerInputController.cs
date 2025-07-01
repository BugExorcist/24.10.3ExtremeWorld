using UnityEngine;
using Entities;
using SkillBridge.Message;
using Services;
using Managers;
using UnityEngine.AI;
using System;
using System.Collections;

public class PlayerInputController : MonoBehaviour
{
    public Rigidbody rb;
    CharacterState state;

    public Character character;
    public float rotateSpeed = 2.0f;
    public float turnAngle = 10;
    public int speed;
    public EntityController entityController;
    public bool onAir = false;

    private NavMeshAgent agent;
    private bool autoNav = false;

    /// <summary>
    /// 是否启用刚体
    /// </summary>
    public bool enableRigidbody
    { 
        get { return !this.rb.isKinematic; }
        set
        {
            this.rb.isKinematic = !value;
            this.rb.detectCollisions = value;

        }
    }


    void Start()
    {
        state = CharacterState.Idle;
        //if(this.character == null)
        //{
        //    DataManager.Instance.Load();
        //    NCharacterInfo cInfo = new NCharacterInfo();
        //    cInfo.Id = 1;
        //    cInfo.Name = "Test";
        //    cInfo.ConfigId = 1;
        //    cInfo.Entity = new NEntity();
        //    cInfo.Entity.Position = new NVector3();
        //    cInfo.Entity.Direction = new NVector3();
        //    cInfo.Entity.Direction.X = 0;
        //    cInfo.Entity.Direction.Y = 100;
        //    cInfo.Entity.Direction.Z = 0;
        //    cInfo.attDynamic = new NAttributeDynamic();
        //    this.character = new Character(cInfo);
        //    if (entityController != null)
        //        entityController.entity = this.character;
        //}

        if (agent == null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();
            agent.stoppingDistance = 1.5f;
            agent.updatePosition = false;
        }
    }

    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    IEnumerator BeginNav(Vector3 target)
    {
        agent.updatePosition = true;
        agent.SetDestination(target);
        yield return null;
        autoNav = true;
        if (state != CharacterState.Move)
        {
            state = CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = this.character.speed / 100f;
        }
    }

    public void StopNav()
    {
        autoNav = false;
        agent.ResetPath();
        if (state != CharacterState.Idle)
        {
            state = CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        agent.updatePosition = false;
        NavPathRenderer.Instance.SetPath(null, Vector3.zero);
    }

    public void NavMove()
    {
        // 路径计算是否完毕
        if (agent.pathPending) return;
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {   //  路径计算失败
            StopNav();
            return;
        }
        if (agent.pathStatus != NavMeshPathStatus.PathComplete) return;

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1)
        {
            StopNav();
            return;
        }

        NavPathRenderer.Instance.SetPath(agent.path, agent.destination);
        if (agent.isStopped || agent.remainingDistance < 1.5f)
        {
            StopNav();
            return;
        }
    }

    void FixedUpdate()
    {
        if (character == null || !character.ready)
            return;
        if (autoNav)
        {
            NavMove();
            return;
        }

        if (InputManager.Instance != null && InputManager.Instance.IsInputMode)
            return;

        float v = Input.GetAxis("Vertical");
        if (v > 0.1)
        {
            if(state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            //垂直方向速度 + 角色面向方向的速度
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }   
        else if (v < -0.1)
        {
            if (state != CharacterState.Move)
            {
                state = CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }
        if(Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if (h < - 0.1 || h > 0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);

            if(rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }
        }
    }
    Vector3 lastPos;

    private void LateUpdate()
    {
        if (character == null || !character.ready)
            return;
        // 同步位置
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rb.transform.position;
        if (this.character == null) return;
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;
        // 同步方向
        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);

        agent.nextPosition = this.transform.position;

        if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }
    public void SendEntityEvent(EntityEvent enetityEvent, int param = 0)
    {
        if(entityController != null)
        {
            entityController.OnEntityEvent(enetityEvent, param);
            MapService.Instance.SendMapEntitySync(enetityEvent, this.character.EntityData, param);
        }
    }

    internal void OnLeaveLevel()
    {
        this.enableRigidbody = false;
        this.rb.velocity = Vector3.zero;//速度赋值为0
    }

    internal void OnEnterLevel()
    {
        this.rb.velocity = Vector3.zero;
        this.entityController.UpdateTransform();
        this.lastPos = this.rb.transform.position;
        this.enableRigidbody = true;
    }
}