using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaserBayonetSettings : WeaponAttachmentController
{
    [Header("Settings")]
    [SerializeField] private float damage;
    [SerializeField] private float additionalCooldown;
    private bool onCooldown;
    private bool isInMelee;

    [Header("Visuals")]
    [SerializeField] private List<string> attackAnimationNames = new List<string>();
    private string lastAnimationCalled;

    [Header("References")]
    [SerializeField] private AudioClipContainer meleeSound;
    [SerializeField] private AudioSource source;

    [SerializeField] private Animator weapon;

    private Animator Weapon
    {
        get
        {
            return weapon;
        }
    }


    private IEnumerator Melee()
    {
        // We have started melee
        isInMelee = true;
        Weapon.gameObject.SetActive(true);

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
        Weapon.SetBool(animation, true);

        // Play sound
        meleeSound.PlayOneShot(source);

        // Wait until animator has started playing animation
        yield return new WaitUntil(() => !Weapon.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        // Then wait until animator has finished playing animation
        yield return new WaitUntil(() => Weapon.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !Weapon.IsInTransition(0));

        // Turn off bool in animator
        Weapon.SetBool(animation, false);

        // Turn off weapon gameobject so as not to show/interact with anything
        Weapon.gameObject.SetActive(false);

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

    public override void Fire(InputAction.CallbackContext ctx)
    {
        if (isInMelee || onCooldown) return;
        StartCoroutine(Melee());
    }
}
