using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

internal class UISystemConfig : UIWindow
{
    public Image masterOff;
    public Image musicOff;
    public Image soundOff;

    public Toggle toggleMaster;
    public Toggle toggleMusic;
    public Toggle toggleSound;

    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSound;

    private void Start()
    {
        this.toggleMaster.isOn = Config.MasterOn;
        this.toggleMusic.isOn = Config.MusicOn;
        this.toggleSound.isOn = Config.SoundOn;
        this.sliderMaster.value = Config.MasterVolume;
        this.sliderMusic.value = Config.MusicVolume;
        this.sliderSound.value = Config.SoundVolume;
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    public void MasterToogle(bool on)
    {
        masterOff.enabled = !on;
        Config.MasterOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void MusicToogle(bool on)
    {
        musicOff.enabled = !on;
        Config.MusicOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void SoundToogle(bool on)
    {
        soundOff.enabled = !on;
        Config.SoundOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void MasterVolume(float vol)
    {
        Config.MasterVolume = (int)vol;
    }

    public void MusicVolume(float vol)
    {
        Config.MusicVolume = (int)vol;
    }

    public void SoundVolume(float vol)
    {
        Config.SoundVolume = (int)vol;
        PlaySoung();
    }

    float lastPlay = 0;
    private void PlaySoung()
    {
        if (Time.realtimeSinceStartup - lastPlay > 0.5)
        {
            lastPlay = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }

}
