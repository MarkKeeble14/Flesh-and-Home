using UnityEngine;

public class SpawnInitialInstructions : MonoBehaviour
{
    [SerializeField] private float duration;

    private void Start()
    {
        TextPopupManager._Instance.SpawnText("Press Space to Jump\nPress Left Shift in the Air to Dash\nPress Escape for Settings", duration);
    }
}
