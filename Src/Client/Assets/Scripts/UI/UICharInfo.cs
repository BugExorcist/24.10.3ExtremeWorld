using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour
{
    public SkillBridge.Message.NCharacterInfo info;

    public TMP_Text charClass;
    public TMP_Text charName;
    public Image highlight;

    public bool Selected
    {
        get { return highlight.IsActive(); }
        set
        {
            highlight.gameObject.SetActive(value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(info != null)
        {
            this.charClass.text = this.info.Class.ToString();
            this.charName.text = this.info.Name.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
