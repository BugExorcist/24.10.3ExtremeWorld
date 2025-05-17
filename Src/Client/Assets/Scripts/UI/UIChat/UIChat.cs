using Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    public TextMeshProUGUI textArea;
    public TabView channelTab;

    public InputField inputField;
    public TMP_Text chatTarget;
    public Dropdown channelDropdown;

    private void Start()
    {
        this.channelTab.onTabSelect += OnDisplayChannelSelected;
        //ChatManager.Instace.OnChat += RefreshUI;
    }

    private void OnDestroy()
    {
        //ChatManager.Instace.OnChat -= RefreshUI;
    }

    private void Update()
    {
        InputManager.Instance.IsInputMode = inputField.isFocused;
    }

    private void OnDisplayChannelSelected(int arg0)
    {
        throw new NotImplementedException();
    }
}
