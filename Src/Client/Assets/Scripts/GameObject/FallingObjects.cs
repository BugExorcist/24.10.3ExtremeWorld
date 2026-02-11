using Common.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FallingObjects : MonoBehaviour
{
    public SpriteRenderer sprite;
    public TextMeshPro text;

    public void SetSprite(ItemDefine item)
    {
        this.sprite.sprite = Resloader.Load<Sprite>(item.Icon);
        this.text.text = item.Name;
    }

    private void Update()
    {
        this.text.transform.forward = Camera.main.transform.forward;
    }
}
