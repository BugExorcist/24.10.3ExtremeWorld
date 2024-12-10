using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterSelect : MonoBehaviour
{
    public GameObject selectPanel;
    public GameObject createPanel;

    public Image[] classImages;
    public Button[] selectButtons;
    public Button startButton;
    public TMP_Text discribeText;
    public TMP_InputField inputName;

    public UICharacterView characterView;

    private CharacterClass characterClass;
    void Start()
    {
        DataManager.Instance.Load();

        UICharacterCreateInit();
    }


    public void UICharavterSelectInit()
    {
        createPanel.SetActive(false);
        selectPanel.SetActive(true);

        selectButtons[0].onClick.Invoke();
    }

    public void UICharacterCreateInit()
    {
        createPanel.SetActive(true);
        selectPanel.SetActive(false);
    }

    public void OnSelectClass(int value)
    {
        this.characterClass = (CharacterClass)value + 1;
        characterView.CurrentCharacter = value;

        for (int i = 0; i < selectButtons.Length; i++)
        {
            classImages[i].gameObject.SetActive(i == value);
        }

        discribeText.text = DataManager.Instance.Characters[value + 1].Description;
    }




    

}
