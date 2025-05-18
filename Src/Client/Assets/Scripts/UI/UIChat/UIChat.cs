using Managers;
using SkillBridge.Message;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Managers.ChatManager;

public class UIChat : MonoBehaviour
{
    public TextMeshProUGUI textArea;
    public TabView channelTab;

    public TMP_InputField inputField;
    public TMP_Text targetText;
    public TMP_Dropdown channelSelect;

    private void Start()
    {
        this.channelTab.onTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += ReFreshUI;
    }

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= ReFreshUI;
    }

    private void Update()
    {
        InputManager.Instance.IsInputMode = inputField.isFocused;
    }
    /// <summary>
    /// 展示选中的频道
    /// </summary>
    /// <param name="idx"></param>
    private void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        ReFreshUI();
    }

    private void ReFreshUI()
    {
        this.textArea.text = ChatManager.Instance.GetCurrentMessage();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if (ChatManager.Instance.sendChannel == LocalChannel.Private)
        {
            this.targetText.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
            {
                this.targetText.text = ChatManager.Instance.PrivateName + ":";
                SetElementSize();
            }
            else
            {
                this.targetText.text = "<无>";
                SetElementSize();
            }
        }
        else
        {
            this.targetText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置私聊时 显示聊天对象名称元素的大小
    /// </summary>
    private void SetElementSize()
    {
        var layout1 = this.targetText.GetComponent<LayoutElement>();
        layout1.preferredWidth = this.targetText.text.Length * 15;
        var layout2 = this.inputField.GetComponent<LayoutElement>();
        layout2.preferredWidth = 300 - layout1.preferredWidth;
    }

    public void OnClickSend()
    {
        OnEndInput(this.inputField.text);
    }

    /// <summary>
    /// 失去焦点时触发 需要绑定在inputField的OnEndEdit事件
    /// </summary>
    /// <param name="text"></param>
    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            ChatManager.Instance.SendChat(text);
        }
        this.inputField.text = "";
    }

    public void OnSendChannelChanged()
    {   //发送频道没有综合 idx要+1
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(this.channelSelect.value + 1)) return;
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)this.channelSelect.value + 1))
        {   //设置频道失败，恢复UI的选中频道
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {   //成功。刷新UI
            Debug.Log("成功切换频道->" + (ChatManager.LocalChannel)(this.channelSelect.value + 1));
            this.ReFreshUI();
        }
    }
}
