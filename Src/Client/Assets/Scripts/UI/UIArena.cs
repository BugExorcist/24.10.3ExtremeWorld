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
        StartCoroutine(StartFight());
    }

    IEnumerator StartFight()
    {
        countDownText.text = "FIGHT";
        yield return new WaitForSeconds(1f);
        roundText.enabled = false;
        countDownText.enabled = false;
    }

    internal void ShowRoundResult(int round, ArenaInfo arenaInfo)
    {
        countDownText.enabled = true;
        if (round < 3)
        {
            roundText.enabled = true;
            foreach (var v in arenaInfo.Rounds)
            {
                if (round == v.Round)
                {
                    if (v.Winner == ArenaManager.Instance.ArenaTeam)
                    {
                        countDownText.text = "YOU WIN";
                    }
                    else if (v.Winner == -1)
                    {
                        countDownText.text = "DRAW";
                    }
                    else
                    {
                        countDownText.text = "YOU LOSE";
                    }
                }
            }
        }
        else
        {
            if (arenaInfo.Winner == ArenaManager.Instance.ArenaTeam)
                countDownText.text = "YOU ARE THE WINNER";
            else
                countDownText.text = "YOU ARE THE LOSER";
        }
    }
}
