using Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour
{
    public TMP_Text avaverName;

    public Character character;

    public UIBuffIcons UIBuffIcons;

    // Start is called before the first frame update
    void Start()
    {
        if(this.character != null)
        {
            UIBuffIcons.SetOwner(this.character);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateInfo();
    }

    void UpdateInfo()
    {
        if(this.character != null)
        {
            string name = this.character.Name + " Lv." + this.character.Info.Level;
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
