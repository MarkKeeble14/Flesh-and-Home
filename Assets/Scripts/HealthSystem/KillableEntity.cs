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

    [SerializeField] private bool spawnHealthTrigger;
    [SerializeField] private EventManager healthTrigger;
    [SerializeField] private Vector2 chanceToSpawnHealthTrigger = new Vector2(1, 6);
    [SerializeField] private bool spawnFuelTrigger;
    [SerializeField] private EventManager fuelTrigger;
    [SerializeField] private Vector2 chanceToSpawnFuelTrigger = new Vector2(1, 6);

    public virtual bool IsDead => currentHealth <= 0;
    public bool IsFull => currentHealth == maxHealth;

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

    [SerializeField] private bool spawnDamageText;
    [SerializeField] private DamagePopupText damagePopupText;
    protected DamageSource lastHitBy;

    [SerializeField] private GameObject[] particlesOnHit;
    [SerializeField] private GameObject[] particlesOnDeath;

    private void OnEnable()
    {
        // Set current health to max health
        CurrentHealth = maxHealth;
    }

    public override void Damage(float damage, DamageSource source)
    {
        if (!acceptDamage) return;
        CurrentHealth -= damage;

        // Spawn particles
        foreach (GameObject obj in particlesOnHit)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }

        lastHitBy = source;
        if (spawnDamageText)
        {
            Instantiate(damagePopupText, transform.position + (Vector3.up * transform.localScale.y / 2), Quaternion.identity).Set(damage, source);
        }

        // Audio
        Instantiate(tempSource, transform.position, Quaternion.identity).Play(onTakeDamageClip);

        if (CurrentHealth <= 0)
        {
            onEndAction();
        }
    }

    public override void Damage(float damage, Vector3 force, DamageSource source)
    {
        Damage(damage, source);

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
            rb.AddForce(Random.onUnitSphere * RandomHelper.RandomFloat(100, 200));
        }

        if (spawnHealthTrigger && !GameManager._Instance.PlayerHealth.IsFull)
        {
            if (RandomHelper.EvaluateChanceTo(chanceToSpawnHealthTrigger))
            {
                Instantiate(healthTrigger, transform.position, Quaternion.identity);
            }
        }

        if (spawnFuelTrigger && GameManager._Instance.PlayerUseFuel)
        {
            if (RandomHelper.EvaluateChanceTo(chanceToSpawnFuelTrigger))
            {
                Instantiate(fuelTrigger, transform.position, Quaternion.identity);
            }
        }

        // Destroy
        if (destroyOnDeath)
        {
            Die();
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

    protected void Die()
    {
        foreach (GameObject obj in particlesOnDeath)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void ResetHealth()
    {
        CurrentHealth = MaxHealth;
    }
}
