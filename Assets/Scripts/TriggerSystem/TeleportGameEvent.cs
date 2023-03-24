using UnityEngine;
using System.Collections.Generic;

public class TeleportGameEvent : GameEvent
{
    [SerializeField] private List<UnlockType> requiredUnlocks = new List<UnlockType>();
    [SerializeField] private SpawnPosition teleportTo;
    private ChangeSpawnPosition playerChangeSpawnPosition;
    [SerializeField] private float fadeDuration = 1f;

    public override string EventString { get => ChangeSpawnPosition.GetTeleportLocationString(teleportTo); }

    private void Update()
    {
        // Restrict Active
        Active = ProgressionManager._Instance.HasAllUnlocks(requiredUnlocks);
    }

    protected override void Activate()
    {
        KillableEntity player = GameManager._Instance.PlayerTransform.GetComponent<KillableEntity>();
        playerChangeSpawnPosition = player.transform.parent.GetComponent<ChangeSpawnPosition>();

        InputManager._Instance.DisableInput();
        player.AcceptDamage = false;

        TransitionManager._Instance.FadeOut(delegate
        {
            playerChangeSpawnPosition.ChangePosition(teleportTo);
            TransitionManager._Instance.FadeIn(delegate
            {
                InputManager._Instance.EnableInput();
                player.AcceptDamage = true;
            }, fadeDuration);
        }, fadeDuration);
    }
}
