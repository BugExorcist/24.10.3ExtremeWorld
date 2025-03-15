using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnCloss;

    public virtual System.Type Type { get { return this.GetType(); } }

    public enum WindowResult
    {
        None = 0,
        Yes,
        No,
    }

    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Closs(this.Type);
        if(this.OnCloss != null)
        {
            this.OnCloss(this, result);
        }
        this.OnCloss = null;
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
}
