using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestFlamethrower : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private ParticleSystem flamethrowerParticleSystem;
    [Header("Fuel")]
    [SerializeField] private FuelStore fuelStore;
    [SerializeField] private float fuelConsumptionRate;
    [Header("References")]
    [SerializeField] private AudioSource flamethrowerSource;
    [SerializeField] private AudioClip flamethrowerStartSound;
    [SerializeField] private AudioSource source;

    private void Start()
    {
        inputManager = InputManager._Instance;
        // inputManager.PlayerInputActions.Player.FireAttachment.started += Fire;
    }

    private void OnDestroy()
    {
        // Remove input action
        // inputManager.PlayerInputActions.Player.FireAttachment.started -= Fire;
    }

    private void Fire(InputAction.CallbackContext ctx)
    {
        if (fuelStore.CurrentFuel > 0)
            StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        // Tell Flamethrower to play
        flamethrowerParticleSystem.Play();
        flamethrowerSource.enabled = true;
        source.PlayOneShot(flamethrowerStartSound);

        while (inputManager.PlayerInputActions.Player.FireAttachment.IsPressed() && fuelStore.CurrentFuel > 0)
        {
            fuelStore.CurrentFuel -= Time.deltaTime * fuelConsumptionRate;
            yield return null;
        }

        // Stop playing
        flamethrowerParticleSystem.Stop();
        flamethrowerSource.enabled = false;
    }
}

