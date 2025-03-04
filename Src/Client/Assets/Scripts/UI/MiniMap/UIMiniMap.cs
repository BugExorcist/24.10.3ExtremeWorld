using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour
{
    public Collider miniMapBoudingBox;
    public Image miniMap;
    public Image arrow;
    public TMP_Text mapName;

    private Transform playerTransforme;

    void Start()
    {
        InitMap();
    }

    void InitMap()
    {
        //初始化迷你地图
        this.mapName.text = User.Instance.CurrentMapData.Name;
        if(this.miniMap.overrideSprite == null)
            this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
        //获取角色位置
        this.playerTransforme = User.Instance.CurrentCharacterObject.transform;
    }

    void Update()
    {   //更新小地图
        if (miniMapBoudingBox == null || playerTransforme == null) return;
        float realWidth = miniMapBoudingBox.bounds.size.x;
        float realHeight = miniMapBoudingBox.bounds.size.z;
        //玩家相对真实地图的坐标
        float playerX = playerTransforme.position.x - miniMapBoudingBox.bounds.min.x;
        float playerY = playerTransforme.position.z - miniMapBoudingBox.bounds.min.z;

        float pivotX = playerX / realWidth;
        float pivotY = playerY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.miniMap.rectTransform.localPosition = Vector3.zero;
        //箭头旋转
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -this.playerTransforme.eulerAngles.y);
    }
}
