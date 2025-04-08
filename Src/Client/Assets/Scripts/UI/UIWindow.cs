using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;

    //ʵ������϶�����
    private RectTransform rootRectTransform = null;//Prifab���ڵ��RectTransform
    public RectTransform dragRectTransform = null;//�϶����ֲ��ֵ�RectTransform
    private Vector2 offset;

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
        UIManager.Instance.Closs(this.Type);
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

    private void OnMouseDown()
    {
        Debug.LogFormat(this.name + "Clicked");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��¼���λ����������Ͻǵ�ƫ����
        if (CanDrag(Input.mousePosition))
            offset = this.rootRectTransform.anchoredPosition - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // ���ݵ�ǰ���λ�õ��������λ��
        if (CanDrag(Input.mousePosition))
        {
            Vector2 newPosition = eventData.position + offset;
            this.rootRectTransform.anchoredPosition = newPosition;
        }
    }

    public bool CanDrag(Vector2 position)
    {   //����Ƿ������˿���ק���
        if (this.dragRectTransform == null)
            return false;
        Vector2 localPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(dragRectTransform, position, null, out localPosition))
        {
            if (dragRectTransform.rect.Contains(localPosition))
                return true;
        }
        return false;
    }

    public virtual void FlashUI() { }
}
