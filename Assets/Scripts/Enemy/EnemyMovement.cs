using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool Move = true;
    [HideInInspector] public Transform Target;
    [SerializeField] protected LayerMask isGround;

    [Header("Audio")]
    [SerializeField] protected AudioSource movementSource;


    protected void Start()
    {
        // Set Target (Currently just the player transform but technically this should be changeable maybe so we can have enemies that target other stuff, like corpses!)
        Target = GameManager._Instance.PlayerTransform;
    }

    public float DistanceToGround
    {
        get
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);
            Physics.Raycast(ray, out hit, Mathf.Infinity, isGround);
            return transform.position.y - hit.point.y;
        }
    }

    public float DistanceToTarget
    {
        get
        {
            return Vector3.Distance(transform.position, Target.position);
        }
    }
}
