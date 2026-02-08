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
    public Image quality;

    public void SetMainIcon(string iconName, string text, string qualityColor = "White")
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.mainText.text = text;
        if (this.quality != null)
        {
            this.quality.color = UIQualityColor.GetColor(qualityColor);
        }
    }
}
