using UnityEngine;
using UnityEngine.InputSystem;

public class PulseGrenadeLauncherSettings : WeaponAttachmentController
{
    [Header("Pulse Grenade Launcher")]
    [SerializeField] private PulseGrenadeSettings settings;

    [SerializeField] private Transform grenadeSpawnPos;
    [SerializeField] private PulseGrenade pulseGrenadePrefab;

    private float fireRateTimer;
    [SerializeField] private float fireRate;

    public override void Fire(InputAction.CallbackContext ctx)
    {
        if (fireRateTimer > 0)
        {
            return;
        }

        PulseGrenade spawned = Instantiate(pulseGrenadePrefab, grenadeSpawnPos.position, Quaternion.identity);
        spawned.Set(settings, Camera.main.transform.forward);

        CrosshairManager._Instance.Spread(CrosshairType.PULSE_GRENADE_LAUNCHER, settings.crosshairSpread);

        fireRateTimer = 1.0f / fireRate;
    }

    private void Update()
    {
        if (fireRateTimer > 0)
        {
            fireRateTimer -= Time.deltaTime;
        }
    }
}
