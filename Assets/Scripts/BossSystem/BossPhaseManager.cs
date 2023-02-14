using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    private int index;
    [SerializeField] private BossPhase[] bossPhaseOrder;

    private BossPhaseBaseState currentPhase;
    public BossIntroPhase bossIntroPhase;
    public BossPhase1 bossPhase1;
    public BossPhase2 bossPhase2;
    public BossPhase3 bossPhase3;
    public BossAddOnsPhase addOnsPhase1;
    public BossAddOnsPhase addOnsPhase2;

    [Header("References")]
    [Header("Shell")]
    [SerializeField] private NavMeshEnemy shellNavMeshEnemy;
    public NavMeshEnemy ShellEnemyMovement => shellNavMeshEnemy;
    [SerializeField] private Rigidbody shellNavMeshRigidbody;
    public Rigidbody ShellRigidBody => shellNavMeshRigidbody;

    [Header("Fleshy")]
    [SerializeField] private EnemyMovement fleshyBoss;
    public EnemyMovement FleshyEnemyMovement => fleshyBoss;

    [Header("Other Objects")]
    [SerializeField] private ImageSliderBar hpBar;
    public ImageSliderBar HPBar => hpBar;
    public Transform Player;

    [Header("Audio")]
    public AudioSource source;
    public TemporaryAudioSource temporaryAudioSource;

    private bool isDone;

    private void Start()
    {
        // Set initial state
        currentPhase = GetCurrentPhase();
        currentPhase.EnterState(this);

        // Get a reference to the player
        Player = GameManager._Instance.PlayerTransform;
    }

    public void SwitchState(BossPhaseBaseState state)
    {
        if (isDone) return;
        currentPhase.ExitState(this);
        currentPhase = state;
        currentPhase.EnterState(this);
    }

    private void Update()
    {
        if (isDone) return;
        // Update the current state
        currentPhase.UpdateState(this);
    }

    private BossPhaseBaseState GetCurrentPhase()
    {
        if (index > bossPhaseOrder.Length - 1)
        {
            Debug.Log("Out of Phases");
            isDone = true;
            return null;
        }

        switch (bossPhaseOrder[index])
        {
            case BossPhase.INTRO:
                return bossIntroPhase;
            case BossPhase.ADDON1:
                return addOnsPhase1;
            case BossPhase.ADDON2:
                return addOnsPhase2;
            case BossPhase.ATTACK1:
                return bossPhase1;
            case BossPhase.ATTACK2:
                return bossPhase2;
            case BossPhase.ATTACK3:
                return bossPhase3;
            default:
                throw new UnhandledSwitchCaseException();
        }
    }

    public void LoadNextPhase()
    {
        index++;
        SwitchState(GetCurrentPhase());
    }
}
