using Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICreatureInfo : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text HP;
    public TMP_Text MP;
    public Slider HPBar;
    public Slider MPBar;
    public Image Avator;


    private Creature target;
    public Creature Target
    {
        get { return target; }
        set
        {
            this.target = value;
            this.UpdateUI();
            this.SetTargetAvator();
        }
    }

    private void SetTargetAvator()
    {
        
    }

    private void UpdateUI()
    {
        if (this.target == null) return;
        this.Name.text = string.Format("{0} Lv.{1}", target.Name, target.Info.Level);
        this.HPBar.maxValue = target.Attributes.MaxHP;
        this.MPBar.maxValue = target.Attributes.MaxMP;
        this.HPBar.value = target.Attributes.HP;
        this.MPBar.value = target.Attributes.MP;
        this.HP.text = string.Format("{0}/{1}", target.Attributes.HP, target.Attributes.MaxHP);
        this.MP.text = string.Format("{0}/{1}", target.Attributes.MP, target.Attributes.MaxMP);
    }

    private void Update()
    {
        this.UpdateUI();
    }
}
