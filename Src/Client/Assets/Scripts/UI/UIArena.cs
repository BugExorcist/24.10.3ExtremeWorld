using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIArena : MonoSingleton<UIArena>
{
    public Text roundText;
    public Text countDownText;

    protected override void OnStart()
    {
        roundText.enabled = false;
        countDownText.enabled = false;
        ArenaManager.Instance.SendReady();
    }

    public void ShowCountDown()
    {
        StartCoroutine(CountDown(10));
    }

    IEnumerator CountDown(int time)
    {
        int total = time;
        roundText.text = "Round " + ArenaManager.Instance.Round;
        roundText.enabled = true;
        countDownText.enabled = true;
        while (total > 0)
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_CountDown);
            countDownText.text = total.ToString();
            yield return new WaitForSeconds(1f);
            total--;
        }
        countDownText.text = "READY";
    }

    internal void ShowRoundStart(int round, ArenaInfo arenaInfo)
    {
        countDownText.text = "FIGHT";
    }

    internal void ShowRoundResult(int round, ArenaInfo arenaInfo)
    {
        countDownText.enabled = true;
        countDownText.text = "YOU WIN";
    }
}
