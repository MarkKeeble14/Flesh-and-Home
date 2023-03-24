using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cinemachine;
using System;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer mixer;

    [Header("Music")]
    [SerializeField] private Slider musicVolumeSlider;
    private string musicVolumeKey = "MusicVolume";
    [SerializeField] private float defaultMusicVolume = .8f;

    [Header("SFX")]
    private string sfxVolumeKey = "SFXVolume";
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private float defaultSFXVolume = .8f;

    [Header("Controls")]
    private string mouseSensitivityKey = "MouseSensitivity";
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Vector2 minMaxMouseSensitivity;
    [SerializeField] private float defaultMouseSensitivity;
    [SerializeField] private CinemachineVirtualCamera playerPOVCam;
    private CinemachinePOV playerPOV;
    private float horizontalSpeedMultiplier = 1f;
    private float verticalSpeedMultiplier = 1f;

    public void SetMusicVolume(float percent)
    {
        PlayerPrefs.SetFloat(musicVolumeKey, percent);
        mixer.SetFloat("MusicVolume", Mathf.Log10(percent) * 20);
        musicVolumeSlider.value = percent;
    }

    public void SetSFXVolume(float percent)
    {
        PlayerPrefs.SetFloat(sfxVolumeKey, percent);
        mixer.SetFloat("SFXVolume", Mathf.Log10(percent) * 20);
        sfxVolumeSlider.value = percent;
    }

    public void SetMouseSensitivity(float percent)
    {
        // Get player POV
        if (!playerPOV)
            playerPOV = playerPOVCam.GetCinemachineComponent<CinemachinePOV>();

        PlayerPrefs.SetFloat(mouseSensitivityKey, percent);
        playerPOV.m_HorizontalAxis.m_MaxSpeed = horizontalSpeedMultiplier * Mathf.Lerp(minMaxMouseSensitivity.x, minMaxMouseSensitivity.y, percent);
        playerPOV.m_VerticalAxis.m_MaxSpeed = verticalSpeedMultiplier * Mathf.Lerp(minMaxMouseSensitivity.x, minMaxMouseSensitivity.y, percent);
        mouseSensitivitySlider.value = percent;
    }

    public void Set()
    {
        // Get player POV
        if (playerPOV == null)
            playerPOV = playerPOVCam.GetCinemachineComponent<CinemachinePOV>();

        // Music Volume 
        InitFloatSetting(musicVolumeKey, defaultMusicVolume, musicVolumeSlider, v => SetMusicVolume(v));

        // SFX Volume
        InitFloatSetting(sfxVolumeKey, defaultSFXVolume, sfxVolumeSlider, v => SetSFXVolume(v));

        // Mouse Sensitivity
        InitFloatSetting(mouseSensitivityKey, defaultMouseSensitivity, mouseSensitivitySlider, v => SetMouseSensitivity(v));
    }

    private void Start()
    {
        Set();
    }

    private void InitFloatSetting(string key, float defaultValue, Slider slider, Action<float> InitAction)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetFloat(key, defaultValue);
            InitAction(defaultValue);
        }
        else
        {
            float value = PlayerPrefs.GetFloat(key);
            InitAction(value);
            slider.value = value;
        }
    }
}

