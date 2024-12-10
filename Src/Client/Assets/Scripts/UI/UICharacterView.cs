using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UICharacterView : MonoBehaviour
{
    public GameObject[] characrers;

    private int currentCharacter = 0;

    public int CurrentCharacter
    {
        get { return currentCharacter; }
        set 
        { 
            currentCharacter = value;
            UpdataCharacter();
        }
    }

    void UpdataCharacter()//更新角色视图
    {
        for(int i = 0; i < characrers.Length; i++)
        {
            characrers[i].SetActive(i == currentCharacter);
        }
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
