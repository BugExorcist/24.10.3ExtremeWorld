using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMainCity : MonoSingleton<UIMainCity>
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");
        Services.UserService.Instance.SendGameLeave();
    }
}
