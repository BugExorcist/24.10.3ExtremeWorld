using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public GameObject UIGameTips;
    public GameObject UILoading;
    public GameObject UILogin;

    public Slider progressBar;
    public TMP_Text progressText;
    public TMP_Text progressNumber;

    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        // չʾ������Ϸ�Ҹ�
        UIGameTips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        // չʾ����ҳ��
        UILoading.SetActive(true);
        UIGameTips.SetActive(false);
        // ��������
        yield return DataManager.Instance.LoadDate();

        //��ʼ������
        MapService.Instance.Init();
        MapItemService.Instance.Init();
        MapItemManager.Instance.Init();
        UserService.Instance.Init();
        //TestManager.Instance.Init();
        StatusService.Instance.Init();
        FriendService.Instance.Init();
        ShopManager.Instance.Init();
        TeamService.Instance.Init();
        GuildService.Instance.Init();
        ChatService.Instance.Init();
        BattleManager.Instance.Init();
        ArenaService.Instance.Init();
        StoryService.Instance.Init();
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);

        // ģ�����������
        for (float i = 50;i < 100;)
        {
            i++;
            progressBar.value = i;
            progressNumber.text = i.ToString("0") + "%";
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        // չʾ��¼ҳ��
        UILogin.SetActive(true);
        UILoading.SetActive(false);
        yield return null;
    }

    private void Update()
    {
        
    }
}