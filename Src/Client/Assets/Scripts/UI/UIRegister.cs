using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services;

public class UIRegister : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField repassword;
    public Button buttonLogin;
    public Button buttonBack;
    public Toggle toggle;

    void Start()
    {
        UserService.Instance.OnRegister = this.OnRegister;
    }

    void OnRegister(SkillBridge.Message.Result result, string msg)
    {
        MessageBox.Show(string.Format("{0}: {1}",result, msg));
    }

    void Update()
    {

    }

    public void OnClickRegister()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            MessageBox.Show("�������˺�");
            return;
        }
        if (string.IsNullOrEmpty (password.text))
        {
            MessageBox.Show("����������");
            return;
        }
        if (string.IsNullOrEmpty(repassword.text))
        {
            MessageBox.Show("���ٴ���������");
            return;
        }
        if (this.password.text != this.repassword.text)
        {
            MessageBox.Show("�������벻һ��");
            return;
        }
        if (!this.toggle.isOn)
        {
            MessageBox.Show("��ͬ���������");
            return;
        }
        UserService.Instance.SendRegister(this.username.text, this.password.text);
    }
}
