using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
using TMPro;

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

    public Transform uiCharList;
    public GameObject uiCharInfo;

    public List<GameObject> uiChars = new List<GameObject>();

    private CharacterClass characterClass;
    private int selectCharacterIdx = -1;

    void Start()
    {
        InitCharacterSelect(true);
        UserService.Instance.OnCharacterCreate = OnCharacterCreate;
    }


    public void InitCharacterSelect(bool value)
    {
        createPanel.SetActive(false);
        selectPanel.SetActive(true);

        if (value)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
        }

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            GameObject go = Instantiate(uiCharInfo, this.uiCharList);
            UICharInfo charInfo = go.GetComponent<UICharInfo>();
            charInfo.info = User.Instance.Info.Player.Characters[i];

            Button button = go.GetComponent<Button>();
            int idx = i;
            button.onClick.AddListener(() =>
            {
                OnSelectCharacter(idx);
            });

            uiChars.Add(go);
            go.SetActive(true);
        }
    }


    public void InitCharacterCreate()
    {
        createPanel.SetActive(true);
        selectPanel.SetActive(false);
    }

    public void OnClickCreate()
    {
        if (string.IsNullOrEmpty(this.inputName.text))
        {
            MessageBox.Show("请输入角色名字!");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.inputName.text, this.characterClass);
        InitCharacterSelect(true);
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

    public void OnCharacterCreate(Result result, string message)
    {
        if(result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }

    }

    public void OnSelectCharacter(int idx)
    {
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Tid, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        characterView.CurrentCharacter = (int)cha.Class - 1;

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo charinfo = this.uiChars[i].GetComponent<UICharInfo>();
            charinfo.Selected = idx == i;
        }
    }

    public void OnClickStart()
    {
        if(selectCharacterIdx >= 0)
        {
            MessageBox.Show("进入游戏", "进入游戏", MessageBoxType.Confirm);
        }
    }
}