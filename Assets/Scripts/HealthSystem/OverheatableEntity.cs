using UnityEngine;

public class OverheatableEntity : EndableEntity
{
    [Header("Overheatable")]
    [SerializeField] protected OverheatSettings overheatSettings;
    protected float currentHeat;
    private float dissapateAfterTimer;

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

    [SerializeField] private bool setDefaultMaterialColorAsDefaultVisualColor = true;

    [Header("Color")]
    [SerializeField] private LaserVisuals visuals;
    private bool emissionEnabled;

    [Header("References")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Rigidbody rigidbody;
    private Material material;
    private Color currentColor;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onHeatClip;

    [SerializeField] private bool spawnDamageText;
    [SerializeField] private DamagePopupText damagePopupText;

    [SerializeField] private GameEvent[] onTakeDamageEvents;
    private bool hasPlayedOnTakeDamageEvents;

    private new void Awake()
    {
        // Get a reference to the instantiated material
        material = renderer.material;

        // Create copy of scriptable object for this entity
        visuals = Instantiate(visuals);

        if (setDefaultMaterialColorAsDefaultVisualColor)
            visuals.SetDefaultColor(material.color);

        base.Awake();
    }

    private void Update()
    {
        // Heat is reduced when above 0
        if (dissapateAfterTimer <= 0)
        {
            // Reduce Heat
            currentHeat = Mathf.Lerp(currentHeat, 0, Time.deltaTime * overheatSettings.heatDissapationRate);
        }

        // Handle dissapate after time
        if (dissapateAfterTimer > 0)
            dissapateAfterTimer -= Time.deltaTime;

        // Interpolate Color based on how close we are to overheating
        float percent = currentHeat / overheatSettings.overheatAfter;
        if (percent > 0 && !emissionEnabled)
        {
            // Enable emission
            emissionEnabled = true;
        }
        else if (percent <= 0 && emissionEnabled)
        {
            // Disable emission
            emissionEnabled = false;
        }
        currentColor = visuals.GetLerpedColor(percent);
        material.color = currentColor;

        if (emissionEnabled)
            material.SetColor("_EmissionColor", visuals.GetEmmissiveColor(currentColor, percent));
        else
            material.SetColor("_EmissionColor", Color.black);
    }

    public override void Damage(float damage, DamageSource damageSource)
    {
        if (!acceptDamage) return;

        if (!hasPlayedOnTakeDamageEvents)
        {
            hasPlayedOnTakeDamageEvents = true;
            foreach (GameEvent gameEvent in onTakeDamageEvents)
            {
                if (gameEvent == null) continue;
                gameEvent.Call();
            }
        }

        // Add heat
        currentHeat += damage;

        if (spawnDamageText)
            Instantiate(damagePopupText, transform.position + (Vector3.up * transform.localScale.y / 2), Quaternion.identity).Set(damage, damageSource);

        // Audio
        onHeatClip.PlayOneShot(source);

        // Check if exeeding heat threshold
        if (currentHeat > overheatSettings.overheatAfter)
        {
            // Call on End
            onEndAction();
        }

        // Set dissapate after timer
        dissapateAfterTimer = overheatSettings.dissapateAfter;
    }

    public override void Damage(float damage, Vector3 force, DamageSource source)
    {
        Damage(damage, source);
        rigidbody.AddForce(force);
    }

    protected override void OnEnd()
    {
        // Debug.Log("On End From Overheatable Entity");

        base.OnEnd();

        // Destroy
        Destroy(gameObject);
    }
}
