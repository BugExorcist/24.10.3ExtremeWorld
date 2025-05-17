using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{
    public void ExitToCharacterSelect()
    {
        SceneManager.Instance.LoadScene("CharacterSelect");
        Services.UserService.Instance.SendGameLeave();
    }

    public void ExitGame()
    {
        Services.UserService.Instance.SendGameLeave(true);
    }
}
