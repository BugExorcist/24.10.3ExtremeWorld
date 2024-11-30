using Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILogin : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public Button buttonLogin;
    public Button buttonResister;
    public Toggle remember;
    public Toggle agree;
    
    void Start()
    {
        UserService.Instance.OnLogin = this.OnLogin;
    }

    void Update()
    {
        
    }

    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (!this.agree.isOn)
        {
            MessageBox.Show("请同意服务条款");
            return;
        }
        UserService.Instance.SendLogin(username.text, password.text);
    }

    public void OnLogin(SkillBridge.Message.Result result, string msg)
    {
        MessageBox.Show(string.Format("{0}: {1}", result, msg));
    }
}
