using Managers;
using SkillBridge.Message;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    public TextMeshProUGUI textArea;
    public TabView channelTab;

    public TMP_InputField inputField;
    public TMP_Text chatText;
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
        if (ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            this.chatText.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
            {
                this.chatText.text = ChatManager.Instance.PrivateName + ":";
                SetElementSize();
            }
            else
            {
                this.chatText.text = "<无>";
                SetElementSize();
            }
        }
        else
        {
            this.chatText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 设置私聊时 显示聊天对象名称元素的大小
    /// </summary>
    private void SetElementSize()
    {
        var layout1 = this.chatText.GetComponent<LayoutElement>();
        layout1.preferredWidth = this.chatText.text.Length * 15;
        var layout2 = this.inputField.GetComponent<LayoutElement>();
        layout2.preferredWidth = 300 - layout1.preferredWidth;
    }

    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
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
        this.chatText.text = "";
    }

    public void OnSendChannelChanged(int idx)
    {   //发送频道没有综合 idx要+1
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1)) return;
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
        {   //设置频道失败，恢复UI的选中频道
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {   //成功。刷新UI
            this.ReFreshUI();
        }
    }
}
