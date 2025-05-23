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

    //实现鼠标拖动窗口
    private RectTransform rootRectTransform = null;//Prifab根节点的RectTransform
    public RectTransform dragRectTransform = null;//拖动把手部分的RectTransform
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
        //// 记录鼠标位置与对象左上角的偏移量
        //if (CanDrag(Input.mousePosition) && this.dragRectTransform != null)
        //    offset = this.rootRectTransform.anchoredPosition - eventData.position;

        //使用PointerEventData优化
        var ed = eventData as PointerEventData;
        if (this.dragRectTransform != null && ed.hovered.Contains(this.dragRectTransform.gameObject))
        {
            offset = this.rootRectTransform.anchoredPosition - ed.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //// 根据当前鼠标位置调整对象的位置
        //if (CanDrag(Input.mousePosition) && this.offset != Vector2.zero)
        //{
        //    Vector2 newPosition = eventData.position + offset;
        //    this.rootRectTransform.anchoredPosition = newPosition;
        //}

        //使用PointerEventData优化
        var ed = eventData as PointerEventData;
        if (this.offset != Vector2.zero && ed.hovered.Contains(this.dragRectTransform.gameObject))
        {
            Vector2 newPosition = eventData.position + offset;
            this.rootRectTransform.anchoredPosition = newPosition;
        }
    }

    //public bool CanDrag(Vector2 position)
    //{   //检测是否点击到了可拖拽组件
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
