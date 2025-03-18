using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITest : UIWindow
{
    public TMP_Text text;

    public void SetTitle(string titlt)
    {
        this.text.text = titlt;
    }
}
