using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : TextPromptKeyTrigger
{
    [SerializeField] private List<UnlockType> requiredUnlocks = new List<UnlockType>();
    [SerializeField] private SpawnPosition teleportTo;
    private ChangeSpawnPosition playerChangeSpawnPosition;

    protected override string Suffix => GetTeleportLocationString(teleportTo);

    private new void Update()
    {
        Active = ProgressionManager._Instance.HasAllUnlocks(requiredUnlocks);
        base.Update();
    }

    private new void Awake()
    {
        onActivate += delegate
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
                }, 1f);
            }, 1f);

        };
        base.Awake();
    }

    private string GetTeleportLocationString(SpawnPosition position)
    {
        switch (position)
        {
            case SpawnPosition.BOSS_ROOM:
                return "Boss Room";
            case SpawnPosition.HUB:
                return "Hub";
            case SpawnPosition.OVERWORLD:
                return "Overworld Spawn";
            case SpawnPosition.RUIN_START:
                return "Ruins Start";
            case SpawnPosition.UNLOCK1:
                return "First Unlock";
            case SpawnPosition.UNLOCK2:
                return "Second Unlock";
            case SpawnPosition.UNLOCK3:
                return "Third Unlock";
            default:
                throw new UnhandledSwitchCaseException();
        }
    }
}
