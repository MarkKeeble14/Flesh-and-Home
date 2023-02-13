using UnityEngine;

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

    private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;

    private Transform playerAimAt;
    public Transform PlayerAimAt => playerAimAt;
}
