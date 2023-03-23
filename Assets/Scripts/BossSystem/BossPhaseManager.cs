using System.Collections.Generic;
using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    private int index;
    [SerializeField] private BossPhase[] bossPhaseOrder;
    [SerializeField] private Transform spawnPosition;
    public Transform SpawnPosition => spawnPosition;

    private BossPhaseBaseState currentPhase;
    public BossIntroPhase bossIntroPhase;
    public BossPhase1 bossPhase1;
    public BossPhase2 bossPhase2;
    public BossPhase3 bossPhase3;
    public BossAddOnsPhase addOnsPhase1;
    public BossAddOnsPhase addOnsPhase2;

    [SerializeField] private Transform armorPlateHolder;
    private List<OverheatableBossComponentEntity> armorPlating = new List<OverheatableBossComponentEntity>();
    public List<OverheatableBossComponentEntity> ArmorPlating { get => armorPlating; }
    private int numPlatesOnBegin;
    public int NumPlatesDestroyed
    {
        get
        {
            return numPlatesOnBegin - armorPlating.Count;
        }
    }

    [Header("References")]
    [Header("Shell")]
    [SerializeField] private NavMeshEnemy shellNavMeshEnemy;
    public NavMeshEnemy ShellEnemyMovement => shellNavMeshEnemy;
    [SerializeField] private Rigidbody shellNavMeshRigidbody;
    public Rigidbody ShellRigidBody => shellNavMeshRigidbody;
    [SerializeField] private Transform[] barrelRotatorHolders;
    [SerializeField] private Transform[] plateRotatorHolders;
    private List<BossRotateBarrels> barrelRotators = new List<BossRotateBarrels>();
    private List<BossRotateBarrels> plateRotators = new List<BossRotateBarrels>();

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

    [SerializeField] private OpenDoorTrigger enterDoor;
    [SerializeField] private OpenDoorTrigger exitDoor;

    private void Awake()
    {
        // Add Barrel Rotators
        foreach (Transform t in barrelRotatorHolders)
        {
            barrelRotators.AddRange(t.GetComponentsInChildren<BossRotateBarrels>());
        }

        // Add Plate Rotators
        foreach (Transform t in plateRotatorHolders)
        {
            plateRotators.AddRange(t.GetComponentsInChildren<BossRotateBarrels>());
        }

        // Add plates
        armorPlating.AddRange(armorPlateHolder.GetComponentsInChildren<OverheatableBossComponentEntity>());
        // Loop through plates; add an "on end action" to each (will be called when the object is "ended", so usually destroyed) which will simply remove it from the list
        foreach (OverheatableBossComponentEntity plate in armorPlating)
        {
            plate.AddAdditionalOnEndAction(() =>
            {
                armorPlating.Remove(plate);
            });
        }
        numPlatesOnBegin = armorPlating.Count;
    }

    public void SetBarrelRotatorsSpeed(float speed)
    {
        foreach (BossRotateBarrels rotateBarrels in barrelRotators)
        {
            rotateBarrels.SetRotateSpeed(speed);
        }
    }


    public void SetPlateRotatorsSpeed(float speed)
    {
        foreach (BossRotateBarrels rotateBarrels in plateRotators)
        {
            rotateBarrels.SetRotateSpeed(speed);
        }
    }

    private void Start()
    {
        // Get a reference to the player
        Player = GameManager._Instance.PlayerTransform;
        // Set initial state

        currentPhase = GetCurrentPhase();
        // EnterCurrentState
    }

    public void SwitchState(BossPhaseBaseState state)
    {
        if (isDone) return;
        currentPhase.ExitState(this);
        currentPhase = state;
        currentPhase.EnterState(this);
    }

    public void EnterCurrentState()
    {
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

            // enterDoor.LockOpened();
            exitDoor.LockOpened();

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
