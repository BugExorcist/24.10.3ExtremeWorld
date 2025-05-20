using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

internal class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer audioMixer;
    public AudioSource musicAudioSource;
    public AudioSource soundAudioSource;

    const string MusicPath = "Music/";
    const string SoundPath = "Sound/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn;}
        set
        {
            musicOn = value;
            this.MusicMute(!musicOn);
        }
    }

    private bool soundOn;
    public bool SoundOn
    {
        get { return soundOn;}
        set
        {
            soundOn = value;
            this.SoundMute(!soundOn);
        }
    }

    private bool masterOn;
    public bool MasterOn
    {
        get { return masterOn;}
        set
        {
            masterOn = value;
            this.MasterMute(!masterOn);
        }
    }

    private int masterVolume;
    public int MasterVolume
    {
        get { return masterVolume;}
        set
        {
            if (masterVolume != value)
            {
                masterVolume = value;
                if (soundOn) this.SetVolume("MasterVolume", masterVolume);
            }
        }
    }

    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume;}
        set
        {
            if (musicVolume != value)
            {
                musicVolume = value;
                if (musicOn) this.SetVolume("MusicVolume", musicVolume);
            }
        }
    }

    private int soundVolume;
    public int SoundVolume
    {
        get { return soundVolume;}
        set
        {
            if (soundVolume != value)
            {
                soundVolume = value;
                if (soundOn) this.SetVolume("SoundVolume", soundVolume);
            }
        }
    }

    private void Start()
    {
        this.MasterVolume = Config.MasterVolume;
        this.MasterOn = Config.MasterOn;
        this.MusicVolume = Config.MusicVolume;
        this.MusicOn = Config.MusicOn;
        this.SoundVolume = Config.SoundVolume;
        this.SoundOn = Config.SoundOn;
    }

    public void MasterMute(bool mute)
    {
        this.SetVolume("MasterVolume", mute ? 0 : masterVolume);
    }

    public void MusicMute(bool mute)
    {
        this.SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    public void SoundMute(bool mute)
    {
        this.SetVolume("SoundVolume", mute ? 0 : soundVolume);
    }

    private void SetVolume(string name, int value)
    {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlayMusic: {0} nuot existed", name);
            return;
        }
        if (musicAudioSource.isPlaying)
            musicAudioSource.Stop();

        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PlaySound(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(SoundPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("PlaySound: {0} nuot existed", name);
            return;
        }
        soundAudioSource.PlayOneShot(clip);
    }
}
