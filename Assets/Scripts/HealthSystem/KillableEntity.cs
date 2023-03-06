using UnityEngine;
using UnityEngine.AI;

public class KillableEntity : EndableEntity
{
    [Header("Damageable")]
    private float currentHealth;
    public virtual float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    [SerializeField] private float maxHealth;
    public virtual float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

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

    public virtual bool IsDead => currentHealth <= 0;

    [SerializeField] private bool acceptKnockback;

    [Header("What Happens After Death?")]
    [SerializeField] private bool destroyOnDeath;

    [Header("References")]
    [SerializeField] protected new Rigidbody rigidbody;
    public Rigidbody Rigidbody => rigidbody;
    [SerializeField] private Rigidbody[] rigidbodiesToEnableOnDeath;
    [SerializeField] protected new Renderer renderer;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onTakeDamageClip;

    private void OnEnable()
    {
        // Set current health to max health
        CurrentHealth = maxHealth;
    }

    public override void Damage(float damage)
    {
        if (!acceptDamage) return;
        CurrentHealth -= damage;

        // Audio
        Instantiate(tempSource, transform.position, Quaternion.identity).Play(onTakeDamageClip);

        if (CurrentHealth <= 0)
        {
            onEndAction();
        }
    }

    public override void Damage(float damage, Vector3 force)
    {
        Damage(damage);

        if (acceptKnockback)
            rigidbody.AddForce(force);
    }

    protected override void OnEnd()
    {
        // Debug.Log(name + ": Killable - On End Called");
        base.OnEnd();

        acceptDamage = false;

        // Deparent objects & enable rigidbodies
        foreach (Rigidbody rb in rigidbodiesToEnableOnDeath)
        {
            rb.transform.parent = null;
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        // Destroy
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }

        // Stop basic enemy functions such as attacking
        if (TryGetComponent(out AttackingEnemy basicEnemy))
        {
            basicEnemy.NotifyOfDeath();
        }

        // Stop enemy from moving
        if (TryGetComponent(out EnemyMovement movement))
        {
            movement.SetMove(false);
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }
}
