using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;

    //实现鼠标拖动窗口
    private RectTransform rootRectTransform = null;//Prifab根节点的RectTransform
    public RectTransform dragRectTransform = null;//拖动把手部分的RectTransform
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
        //}   被注释的写法和下面这行的功能相同
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
        // 记录鼠标位置与对象左上角的偏移量
        if (CanDrag(Input.mousePosition))
            offset = this.rootRectTransform.anchoredPosition - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 根据当前鼠标位置调整对象的位置
        if (CanDrag(Input.mousePosition))
        {
            Vector2 newPosition = eventData.position + offset;
            this.rootRectTransform.anchoredPosition = newPosition;
        }
    }

    public bool CanDrag(Vector2 position)
    {   //检测是否点击到了可拖拽组件
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
