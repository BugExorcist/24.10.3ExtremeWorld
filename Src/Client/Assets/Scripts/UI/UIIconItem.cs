using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIconItem : MonoBehaviour
{
    public Image mainImage;
    public Image secondImage;

    public TMP_Text mainText;

    public void SetMainIcon(string iconName, string text)
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.mainText.text = text;
    }
}
