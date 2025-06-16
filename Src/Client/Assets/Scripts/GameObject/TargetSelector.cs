using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoSingleton<TargetSelector>
{
    Projector projector;
    bool actived = false;

    Vector3 center;
    //技能释放范围
    private float range;
    private float size;
    Vector3 offset = new Vector3(0f, 2f, 0f);

    protected Action<Vector3> selectPoint;

    protected override void OnStart()
    {
        projector = this.GetComponentInChildren<Projector>();
        projector.gameObject.SetActive(actived);
    }

    public void Active(bool active)
    {
        this.actived = active;
        if (projector == null) return;

        projector.gameObject.SetActive(active);
        projector.orthographicSize = this.size * 0.5f;
    }

    private void Update()
    {
        if (!actived) return;
        if (this.projector == null) return;
        InputManager.Instance.IsInputMode = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//摄像机需要设置MainCamera的Tag这里才能找到
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 100f, LayerMask.GetMask("Terrain")))//检测射线是否与地面相交
        {
            Vector3 hitPoint = hitInfo.point;
            Vector3 dist = hitPoint - this.center;

            if (dist.magnitude > this.range)
            {
                hitPoint = this.center + dist.normalized * this.range;
            }

            this.projector.gameObject.transform.position = hitPoint + offset;
            if (Input.GetMouseButtonDown(0))
            {
                this.selectPoint?.Invoke(hitPoint);
                this.Active(false);
                InputManager.Instance.IsInputMode = false;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            this.Active(false);
            InputManager.Instance.IsInputMode = false;
        }
    }

    public static void ShowSelector(Vector3Int center, int range, int size, Action<Vector3> onPositionSelected)
    {
        if (Instance == null) return;
        Instance.center = GameObjectTool.LogicToWorld(center);
        Instance.range = GameObjectTool.LogicToWorld(range);
        Instance.size = GameObjectTool.LogicToWorld(size);
        Instance.selectPoint = onPositionSelected;
        Instance.Active(true);
    }
}
