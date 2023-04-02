﻿using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlamethrowerSettings : WeaponAttachmentController
{
    [System.Serializable]
    public struct TriggerSettings
    {
        public float damage;
        public float tickRate;
        public LayerMask canHit;
        public float crosshairSpread;
    }

    [SerializeField] private TriggerSettings triggerSettings;

    [Header("Fuel")]
    [SerializeField] private FloatStore fuelStore;
    [SerializeField] private float fuelConsumptionRate;
    [Header("References")]
    [SerializeField] private ParticleSystem flamethrowerParticleSystem;
    [SerializeField] private FlamethrowerTrigger trigger;
    [SerializeField] private AudioSource flamethrowerSource;
    [SerializeField] private AudioClip flamethrowerStartSound;
    [SerializeField] private AudioSource source;

    public override void Fire(InputAction.CallbackContext ctx)
    {
        if (fuelStore.CurrentFloat > 0)
        {
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        // Tell Flamethrower to play
        trigger.Activate(triggerSettings);
        flamethrowerParticleSystem.Play();
        flamethrowerSource.enabled = true;
        source.PlayOneShot(flamethrowerStartSound);

        while (InputManager._Instance.PlayerInputActions.Player.FireAttachment.IsPressed() && fuelStore.CurrentFloat > 0)
        {
            Firing = true;

            // Spread Crosshair
            CrosshairManager._Instance.Spread(CrosshairType.FLAMETHROWER, triggerSettings.crosshairSpread);

            // Use fuel
            fuelStore.AlterFloat(-Time.deltaTime * fuelConsumptionRate);

            yield return null;
        }

        Firing = false;

        // Stop playing
        flamethrowerParticleSystem.Stop();
        flamethrowerSource.enabled = false;
        trigger.Deactivate();
    }
}

