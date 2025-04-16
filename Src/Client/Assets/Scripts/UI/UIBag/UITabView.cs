using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UITabView : MonoBehaviour
{
    public UITabButton[] tabButtons;
    public GameObject[] tabPages;

    public UnityAction<int> onTabSelect;

    public int index = -1;

    IEnumerator Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].index = i;
        }
        yield return new WaitForEndOfFrame();
        SelectPage(0);
    }

    public void SelectPage(int index)
    {
        if (this.index != index)
        {
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(index == i);
                if (i < tabPages.Length - 1)
                    tabPages[i].SetActive(i == index);
            }
            onTabSelect?.Invoke(index);
        }
    }
}
