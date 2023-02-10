using UnityEngine;

public class KillableEntity : EndableEntity
{
    [Header("Damageable")]
    [SerializeField] protected float maxHealth;
    protected float currentHealth;

    [SerializeField] private bool destroyOnDeath;

    [Header("References")]
    [SerializeField] private new Rigidbody rigidbody;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onTakeDamageClip;


    private void OnEnable()
    {
        // Set current health to max health
        currentHealth = maxHealth;
    }

    public override void Damage(float damage)
    {
        currentHealth -= damage;

        // Audio
        onTakeDamageClip.PlayOneShot(source);

        if (currentHealth <= 0)
        {
            onEndAction();
        }
    }

    public override void Damage(float damage, Vector3 force)
    {
        Damage(damage);
        rigidbody.AddForce(force);
    }

    protected override void OnEnd()
    {
        base.OnEnd();

        // Destroy
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}
