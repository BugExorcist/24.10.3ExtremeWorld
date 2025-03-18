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

        // 展示健康游戏忠告
        UIGameTips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        // 展示加载页面
        UILoading.SetActive(true);
        UIGameTips.SetActive(false);
        // 加载数据
        yield return DataManager.Instance.LoadDate();

        //初始化服务
        MapService.Instance.Init();
        UserService.Instance.Init();

        TestManager.Instance.Init();

        // 模拟进度条加载
        for (float i = 50;i < 100;)
        {
            i++;
            progressBar.value = i;
            progressNumber.text = i.ToString("0") + "%";
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        // 展示登录页面
        UILogin.SetActive(true);
        UILoading.SetActive(false);
        yield return null;
    }

    private void Update()
    {
        
    }
}