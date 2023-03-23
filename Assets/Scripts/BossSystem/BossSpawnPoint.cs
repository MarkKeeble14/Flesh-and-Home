using UnityEngine;

public class BossSpawnPoint : MonoBehaviour
{
    public bool SpawnedOn { get; set; }
    public Vector3 InitPosition { get; private set; }

    private void Awake()
    {
        InitPosition = transform.localPosition;
    }

    public void SetMultipliedPosition(float xZMult)
    {
        transform.localPosition = new Vector3(InitPosition.x * xZMult, InitPosition.y, InitPosition.z * xZMult);
    }
}
