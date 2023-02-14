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
        playerTransform = FindObjectOfType<PlayerController>().transform;
        playerAimAt = FindObjectOfType<PlayerHealth>().transform;
    }

    [SerializeField] private GameObject loseScreen;

    public void OpenLoseScreen()
    {
        loseScreen.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;

    private Transform playerAimAt;
    public Transform PlayerAimAt => playerAimAt;
}
