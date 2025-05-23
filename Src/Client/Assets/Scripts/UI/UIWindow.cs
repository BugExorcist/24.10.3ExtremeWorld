using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;
    public GameObject Root;

    //ʵ������϶�����
    private RectTransform rootRectTransform = null;//Prifab���ڵ��RectTransform
    public RectTransform dragRectTransform = null;//�϶����ֲ��ֵ�RectTransform
    private Vector2 offset = Vector2.zero;

    public virtual System.Type Type { get { return this.GetType(); } }

    private void Awake()
    {
        this.rootRectTransform = transform.Find("Root").GetComponent<RectTransform>();
    }

    public enum WindowResult
    {
        None = 0,
        Yes,
        No,
    }

    public void Close(WindowResult result = WindowResult.None)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        UIManager.Instance.Close(this.Type);
        //if(this.OnClose != null)
        //{
        //    this.OnClose(this, result);
        //}   ��ע�͵�д�����������еĹ�����ͬ
        OnClose?.Invoke(this, result);

        this.OnClose = null;
    }

    public virtual void OnCloseClick()
    {
        this.Close();
    }

    public virtual void OnYesClick()
    {
        this.Close(WindowResult.Yes);
    }

    public virtual void OnNoClick()
    {
        this.Close(WindowResult.No);
    }

    private void OnMouseDown()
    {
        Debug.LogFormat(this.name + "Clicked");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //// ��¼���λ����������Ͻǵ�ƫ����
        //if (CanDrag(Input.mousePosition) && this.dragRectTransform != null)
        //    offset = this.rootRectTransform.anchoredPosition - eventData.position;

        //ʹ��PointerEventData�Ż�
        var ed = eventData as PointerEventData;
        if (this.dragRectTransform != null && ed.hovered.Contains(this.dragRectTransform.gameObject))
        {
            offset = this.rootRectTransform.anchoredPosition - ed.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //// ���ݵ�ǰ���λ�õ��������λ��
        //if (CanDrag(Input.mousePosition) && this.offset != Vector2.zero)
        //{
        //    Vector2 newPosition = eventData.position + offset;
        //    this.rootRectTransform.anchoredPosition = newPosition;
        //}

        //ʹ��PointerEventData�Ż�
        var ed = eventData as PointerEventData;
        if (this.offset != Vector2.zero && ed.hovered.Contains(this.dragRectTransform.gameObject))
        {
            Vector2 newPosition = eventData.position + offset;
            this.rootRectTransform.anchoredPosition = newPosition;
        }
    }

    //public bool CanDrag(Vector2 position)
    //{   //����Ƿ������˿���ק���
    //    if (this.dragRectTransform == null)
    //        return false;
    //    Vector2 localPosition;
    //    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRectTransform, position, null, out localPosition))
    //    {
    //        if (dragRectTransform.rect.Contains(localPosition))
    //            return true;
    //    }
    //    return false;
    //}
}
