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
        //��ʼ�������ͼ
        this.mapName.text = User.Instance.CurrentMapData.Name;
        if(this.miniMap.overrideSprite == null)
            this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();
        this.miniMap.SetNativeSize();
        this.miniMap.transform.localPosition = Vector3.zero;
        //��ȡ��ɫλ��
        this.playerTransforme = User.Instance.CurrentCharacterObject.transform;
    }

    void Update()
    {   //����С��ͼ
        if (miniMapBoudingBox == null || playerTransforme == null) return;
        float realWidth = miniMapBoudingBox.bounds.size.x;
        float realHeight = miniMapBoudingBox.bounds.size.z;
        //��������ʵ��ͼ������
        float playerX = playerTransforme.position.x - miniMapBoudingBox.bounds.min.x;
        float playerY = playerTransforme.position.z - miniMapBoudingBox.bounds.min.z;

        float pivotX = playerX / realWidth;
        float pivotY = playerY / realHeight;

        this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.miniMap.rectTransform.localPosition = Vector3.zero;
        //��ͷ��ת
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -this.playerTransforme.eulerAngles.y);
    }
}
