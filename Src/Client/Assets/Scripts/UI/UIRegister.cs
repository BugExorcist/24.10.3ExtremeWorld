using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UIRegister : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField password;
    public TMP_InputField repassword;
    public Button buttonLogin;
    public Button buttonBack;
    public Toggle agree;

    void Start()
    {
        UserService.Instance.OnRegister = this.OnRegister;
    }

    void Update()
    {

    }

    public void OnClickRegister()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        if (string.IsNullOrEmpty(username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        if (string.IsNullOrEmpty (password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(repassword.text))
        {
            MessageBox.Show("请再次输入密码");
            return;
        }
        if (this.password.text != this.repassword.text)
        {
            MessageBox.Show("两次密码不一致");
            return;
        }
        if (!this.agree.isOn)
        {
            MessageBox.Show("请同意服务条款");
            return;
        }
        UserService.Instance.SendRegister(this.username.text, this.password.text);
    }

    void OnRegister(Result result, string msg)
    {
        MessageBox.Show(string.Format("{0}: {1}", result, msg));
    }
}
