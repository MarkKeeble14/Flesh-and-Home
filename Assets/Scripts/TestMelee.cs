using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMelee : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float additionalCooldown;
    private bool onCooldown;

    [Header("Visuals")]
    [SerializeField] private List<string> attackAnimationNames = new List<string>();
    private string lastAnimationCalled;

    [Header("References")]
    [SerializeField] private AudioClipContainer meleeSound;
    [SerializeField] private AudioSource source;
    [SerializeField] private Animator weapon;
    private InputManager inputManager;

    private bool isInMelee;

    private void Start()
    {
        inputManager = InputManager._Instance;
        inputManager.PlayerInputActions.Player.Melee.started += Melee;
    }

    private void OnDestroy()
    {
        // Remove input action
        inputManager.PlayerInputActions.Player.Melee.started -= Melee;
    }

    private void Melee(InputAction.CallbackContext ctx)
    {
        if (isInMelee || onCooldown) return;
        StartCoroutine(Melee());
    }

    private IEnumerator Melee()
    {
        // We have started melee
        isInMelee = true;
        weapon.gameObject.SetActive(true);

        // Play a random animation
        string animation;
        // If there are more than one possible animations, prevent the same one from being called twice in a row
        if (attackAnimationNames.Count > 1)
        {
            bool mustReAdd = attackAnimationNames.Contains(lastAnimationCalled);
            attackAnimationNames.Remove(lastAnimationCalled);
            animation = attackAnimationNames[Random.Range(0, attackAnimationNames.Count)];
            if (mustReAdd)
                attackAnimationNames.Add(lastAnimationCalled);
            lastAnimationCalled = animation;
        }
        else
        {
            // If there is only one amimation, just play it
            animation = attackAnimationNames[0];
        }

        // Set bool in animator
        weapon.SetBool(animation, true);

        // Play sound
        meleeSound.PlayOneShot(source);

        // Wait until animator has started playing animation
        yield return new WaitUntil(() => !weapon.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        // Then wait until animator has finished playing animation
        yield return new WaitUntil(() => weapon.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !weapon.IsInTransition(0));

        // Turn off bool in animator
        weapon.SetBool(animation, false);

        // Turn off weapon gameobject so as not to show/interact with anything
        weapon.gameObject.SetActive(false);

        // Additional Cooldown
        StartCoroutine(Cooldown(additionalCooldown));

        // We have ended melee
        isInMelee = false;
        // weapon.ResetTrigger(animation);
    }

    private IEnumerator Cooldown(float cd)
    {
        onCooldown = true;
        yield return new WaitForSeconds(cd);
        onCooldown = false;
    }
}
