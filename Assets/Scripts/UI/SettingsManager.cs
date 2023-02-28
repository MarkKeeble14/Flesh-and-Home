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
    [Tooltip("The amount to multiply each axis by to make it so x is sensitivity is relative to y")]
    [SerializeField] private Vector2 axisMultipliers = new Vector2(2, 0.34f);
    private CinemachinePOV playerPOV;

    private void Awake()
    {
        // Get player POV
        playerPOV = playerPOVCam.GetCinemachineComponent<CinemachinePOV>();
    }

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
        PlayerPrefs.SetFloat(mouseSensitivityKey, percent);
        playerPOV.m_HorizontalAxis.m_MaxSpeed = axisMultipliers.x * Mathf.Lerp(minMaxMouseSensitivity.x, minMaxMouseSensitivity.y, percent);
        playerPOV.m_VerticalAxis.m_MaxSpeed = axisMultipliers.y * Mathf.Lerp(minMaxMouseSensitivity.x, minMaxMouseSensitivity.y, percent);
        mouseSensitivitySlider.value = percent;
    }

    private void Start()
    {
        // Music Volume 
        InitFloatSetting(musicVolumeKey, defaultMusicVolume, musicVolumeSlider, v => SetMusicVolume(v));

        // SFX Volume
        InitFloatSetting(sfxVolumeKey, defaultSFXVolume, sfxVolumeSlider, v => SetSFXVolume(v));

        // Mouse Sensitivity
        InitFloatSetting(mouseSensitivityKey, defaultMouseSensitivity, mouseSensitivitySlider, v => SetMouseSensitivity(v));
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

