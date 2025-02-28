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
    // Start is called before the first frame update
    void Start()
    {
        if(this.character == null)
        {

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateInfo();

        this.transform.forward = Camera.main.transform.forward;
    }

    void UpdateInfo()
    {
        if(this.character != null)
        {
            string name = this.character.Name + "Lv." + this.character.Info.Level;
            if(name != this.avaverName.text)
            {
                this.avaverName.text = name;
            }
        }
    }
}
