using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UIWindow;

public class UIMain : MonoSingleton<UIMain>
{
    public TMP_Text avaterName;
    public TMP_Text avaterLevel;
    public TMP_Text id;

    protected override void OnStart()
    {
        this.id.text = string.Format("ID:{0}", User.Instance.CurrentCharacter.Id);
        UpdataUIAvatar();
    }

    void UpdataUIAvatar()
    {
        this.avaterName.text = User.Instance.CurrentCharacter.Name;
        this.avaterLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }


    public void BackToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    /// <summary>
    /// UI≤‚ ‘
    /// </summary>
    //public void OnClickTest()
    //{
    //    UITest test = UIManager.Instance.Show<UITest>();
    //    test.OnClose += Test_OnClose;
    //}

    //private void Test_OnClose(UIWindow sender, WindowResult result)
    //{

    //}

    public void OnClickbag()
    {
        UIManager.Instance.Show<UIBag>();
    }

    public void OnClickEquip()
    {
       UIManager.Instance.Show<UICharEquip>();
    }
    
    public void OnClickQuest()
    {
       UIManager.Instance.Show<UIQuestSystem>();
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }
}
