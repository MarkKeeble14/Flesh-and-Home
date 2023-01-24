using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestFlamethrower : MonoBehaviour
{
    private InputManager inputManager;
    [SerializeField] private ParticleSystem flamethrowerParticleSystem;
    private ParticleSystemRenderer flamethrowerParticleSystemRenderer;
    [SerializeField] private Material[] possibleMaterials;
    private void Awake()
    {
        // Set references
        flamethrowerParticleSystemRenderer = flamethrowerParticleSystem.GetComponent<ParticleSystemRenderer>();
    }

    private void Start()
    {
        inputManager = InputManager._Instance;
    }


    private void Update()
    {
        if (!flamethrowerParticleSystem.isEmitting && inputManager.PlayerInputActions.Player.FireAttachment.IsPressed())
        {
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        // Tell Flamethrower to play
        flamethrowerParticleSystem.Play();

        while (inputManager.PlayerInputActions.Player.FireAttachment.IsPressed())
        {
            yield return null;
        }

        // Stop playing
        flamethrowerParticleSystem.Stop();
    }
}

