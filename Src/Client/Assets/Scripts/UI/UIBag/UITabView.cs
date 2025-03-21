using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITabView : MonoBehaviour
{
    public UITabButton[] tabButtons;
    public GameObject[] tabPages;

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
            this.index = index;
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabPages[i].SetActive(index == i);
                tabButtons[i].Select(index == i);
            }
        }
    }
}
