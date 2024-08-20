using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public GameObject SettingsUI;
    
    public AudioMixer audioMixer;
    public Slider generalVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {   
            SettingsUI.SetActive(!SettingsUI.activeSelf);
        }
    }

    public void SetGeneralVolume()
    {
        float volume = generalVolumeSlider.value;
        audioMixer.SetFloat("General",Mathf.Log10(volume) * 20);
    }
    
    public void SetSFXVolume()
    {
        float volume = sfxVolumeSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
    
    public void SetMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }
    
}
