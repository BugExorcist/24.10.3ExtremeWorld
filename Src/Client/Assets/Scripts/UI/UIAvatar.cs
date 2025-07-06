using Entities;
using Models;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAvatar : MonoBehaviour
{
    public TMP_Text Name;
    public TMP_Text Level;
    public TMP_Text HP;
    public TMP_Text MP;
    public Slider HPBar;
    public Slider MPBar;
    public Image[] Avatars;
    public UIBuffIcons UIBuffIcons;

    private Character player;

    public void SetAvatarInfo(Character palyer)
    {
        this.player = palyer;
        UIBuffIcons.SetOwner(this.player);
        UpdateUI();
        this.SetAvatorImage();
    }

    private void SetAvatorImage()
    {
        for (int i = 1; i < Avatars.Length; i++)
        {
            Avatars[i].gameObject.SetActive((int)this.player.Info.Class == i);
        }
    }

    private void UpdateUI()
    {
        if (this.player == null) return;
        this.Name.text = player.Name;
        this.Level.text = player.Info.Level.ToString();
        this.HPBar.maxValue = player.Attributes.MaxHP;
        this.MPBar.maxValue = player.Attributes.MaxMP;
        this.HPBar.value = User.Instance.CurrentCharacter.Attributes.HP;
        this.MPBar.value = User.Instance.CurrentCharacter.Attributes.MP;
        this.HP.text = string.Format("{0}/{1}", player.Attributes.HP, player.Attributes.MaxHP);
        this.MP.text = string.Format("{0}/{1}", player.Attributes.MP, player.Attributes.MaxMP);
    }

    private void Update()
    {
        this.UpdateUI();
    }
}
