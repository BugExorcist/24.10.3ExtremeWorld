using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITabButton : MonoBehaviour
{
    private Sprite normalImage;//δ��ѡ�е�ͼƬ
    public Sprite activeImage;//��ѡ�е�ͼƬ
    public bool OnSecleted = false;
    public UITabView tabView;

    public int index = -1;
    public Image image;

    void Start()
    {
        this.image = this.GetComponent<Image>();
        this.normalImage = this.image.sprite;

        this.GetComponent<Button>().onClick.AddListener(Onclick);
    }

    private void Onclick()
    {
        this.tabView.SelectPage(this.index);
    }

    public void Select(bool v)
    {
        image.overrideSprite = v ? activeImage : normalImage;
    }
}
