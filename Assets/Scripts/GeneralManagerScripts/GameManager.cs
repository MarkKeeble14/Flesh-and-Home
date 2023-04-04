using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance { get; private set; }
    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;

        // Find Player
        playerHealth = FindObjectOfType<PlayerHealth>();
        playerTransform = FindObjectOfType<PlayerController>().transform;
        playerAimAt = playerHealth.transform;
    }

    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject lastCheckpointButton;
    [SerializeField] private FloatStore fuelStore;

    public void OpenLoseScreen()
    {
        loseScreen.SetActive(true);
        Cursor.visible = true;

        // lastCheckpointButton.SetActive(ProgressionManager._Instance.AllowRestartFromLastCheckpoint);
        lastCheckpointButton.SetActive(false);
    }

    public void CloseLoseScreen()
    {
        loseScreen.SetActive(false);
        Cursor.visible = false;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        fuelStore.Reset();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;

    private Transform playerAimAt;
    public Transform PlayerAimAt => playerAimAt;

    [SerializeField] private UnlimitedFloatStore gameDuration;

    public float GetGameDuration()
    {
        // Returns the games duration in seconds
        return gameDuration.CurrentFloat;
    }

    private KillableEntity playerHealth;
    public KillableEntity PlayerHealth => playerHealth;

    public bool PlayerUseFuel { get; set; }
    public bool FuelFull => fuelStore.CurrentFloat == fuelStore.MaxFloat;

    private bool updateGameTimer;

    public void StartGameTimer()
    {
        gameDuration.Reset();
        updateGameTimer = true;
    }

    private void Update()
    {
        if (updateGameTimer)
        {
            gameDuration.AlterFloat(Time.deltaTime);
        }
    }
}
