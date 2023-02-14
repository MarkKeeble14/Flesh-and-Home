using UnityEngine;

public class KillableEntity : EndableEntity
{
    [Header("Damageable")]
    [SerializeField] protected float maxHealth;
    public float MaxHealth => maxHealth;
    protected float currentHealth;
    [SerializeField] protected bool acceptDamage = true;
    public bool AcceptDamage
    {
        get
        {
            return acceptDamage;
        }
        set
        {
            acceptDamage = value;
        }
    }

    [SerializeField] private bool destroyOnDeath;

    [Header("References")]
    [SerializeField] protected new Rigidbody rigidbody;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onTakeDamageClip;

    private void OnEnable()
    {
        // Set current health to max health
        currentHealth = maxHealth;
    }

    public override void Damage(float damage)
    {
        if (!acceptDamage) return;
        currentHealth -= damage;

        // Audio
        Instantiate(tempSource, transform.position, Quaternion.identity).Play(onTakeDamageClip);

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
