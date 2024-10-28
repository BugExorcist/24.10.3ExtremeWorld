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

        UIGameTips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        UIGameTips.SetActive(false);
        yield return new WaitForSeconds(1f);
        UILogin.SetActive(true);
        UILoading.SetActive(false);


        yield return null;
    }

    private void Update()
    {
        
    }
}