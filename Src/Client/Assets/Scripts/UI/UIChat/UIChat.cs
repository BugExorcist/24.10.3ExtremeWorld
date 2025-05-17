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
    /// չʾѡ�е�Ƶ��
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
                this.chatText.text = "<��>";
                SetElementSize();
            }
        }
        else
        {
            this.chatText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ����˽��ʱ ��ʾ�����������Ԫ�صĴ�С
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
    /// ʧȥ����ʱ���� ��Ҫ����inputField��OnEndEdit�¼�
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
    {   //����Ƶ��û���ۺ� idxҪ+1
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1)) return;
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx + 1))
        {   //����Ƶ��ʧ�ܣ��ָ�UI��ѡ��Ƶ��
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {   //�ɹ���ˢ��UI
            this.ReFreshUI();
        }
    }
}
