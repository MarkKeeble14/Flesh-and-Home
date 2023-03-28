using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Cinemachine;
using System;
using UnityEngine.InputSystem;

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
    private InputManager inputManager;
    private bool holdBreath;

    private float holdDownMouseSpeed;

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


    private float xMaxSpeed;
    private float yMaxSpeed;
    public void SetMouseSensitivity(float percent)
    {
        // Get player POV
        if (!playerPOV)
            playerPOV = playerPOVCam.GetCinemachineComponent<CinemachinePOV>();

        PlayerPrefs.SetFloat(mouseSensitivityKey, percent);
        xMaxSpeed = horizontalSpeedMultiplier * Mathf.Lerp(minMaxMouseSensitivity.x, minMaxMouseSensitivity.y, percent);
        yMaxSpeed = verticalSpeedMultiplier * Mathf.Lerp(minMaxMouseSensitivity.x, minMaxMouseSensitivity.y, percent);

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
        inputManager = InputManager._Instance;
        Set();

    }

    private void Update() {
        if (inputManager.PlayerInputActions.Player.HoldBreath.IsPressed()) {
            holdDownMouseSpeed = .25f;
        } else {
            holdDownMouseSpeed = 1f;
        }
        playerPOV.m_HorizontalAxis.m_MaxSpeed = xMaxSpeed * holdDownMouseSpeed;
        playerPOV.m_VerticalAxis.m_MaxSpeed = yMaxSpeed * holdDownMouseSpeed;
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

