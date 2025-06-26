using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager>
{
    public GameObject nameBarPrefab;
    public GameObject npcStatusProfab;
    public GameObject popupTextProfab;

    private Dictionary<Transform, GameObject> elementNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elemantStatus = new Dictionary<Transform, GameObject>();

    protected override void OnStart()
    {
        this.nameBarPrefab.SetActive(false);
        this.popupTextProfab.SetActive(false);
    }

    public void AddCharacterNameBar(Transform owner, Character character)
    {
        GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);
        goNameBar.name = "NameBar" + character.entityId;
        goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        goNameBar.SetActive(true);
        this.elementNames[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if(this.elementNames.ContainsKey(owner))
        {
            Destroy(this.elementNames[owner]);
            this.elementNames.Remove(owner);
        }
    }

    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        GameObject go = Instantiate(npcStatusProfab, this.transform);
        go.name = "NpcQuestStatus" + owner.name;
        go.GetComponent<UIWorldElement>().owner = owner;
        go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
        go.SetActive(true);
        this.elemantStatus[owner] = go;
    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elemantStatus.ContainsKey(owner))
        {
            Destroy(this.elemantStatus[owner]);
            this.elemantStatus.Remove(owner);
        }
    }

    public void ShowPopupText(PopupType type, Vector3 pos, float damage, bool isCrit)
    {
        GameObject go = Instantiate(popupTextProfab, pos, Quaternion.identity, this.transform);
        go.name = "Popup";
        go.GetComponent<UIPopupText>().InitPopup(type, damage, isCrit);
        go.SetActive(true);
    }
}
