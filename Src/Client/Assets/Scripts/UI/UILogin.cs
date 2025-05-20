using Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

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
            MessageBox.Show("�������˺�");
            return;
        }
        if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("����������");
            return;
        }
        if (!this.agree.isOn)
        {
            MessageBox.Show("��ͬ���������");
            return;
        }
        UserService.Instance.SendLogin(username.text, password.text);
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void OnLogin(Result result, string msg)
    {
        if (result == Result.Success)
        {
            //MessageBox.Show(string.Format("{0}: {1}", result, msg));
            // ��¼�ɹ��������ɫѡ�񳡾�
            SceneManager.Instance.LoadScene("CharSelect");
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        }
        else  
        {
            MessageBox.Show(msg, "����", MessageBoxType.Error);
        }
    }
}
