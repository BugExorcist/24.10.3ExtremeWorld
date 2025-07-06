using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIStory : UIWindow
{
    public TMP_Text Title;
    public TMP_Text descript;

    StoryDefine story;

    public void SetStory(StoryDefine define)
    {
        this.story = define;
        this.Title.text = define.Name;
        this.descript.text = define.Description;
    }

    public void OnClickStrat()
    {
        if (!StoryManager.Instance.StartStory(this.story.ID))
        {

        }
        this.Close();
    }
}
