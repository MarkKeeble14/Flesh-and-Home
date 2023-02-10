using UnityEngine;

public class OverheatableEntity : EndableEntity
{
    [Header("Overheatable")]
    [SerializeField] protected float currentHeat;
    [SerializeField] protected float maxWithStandableHeat;
    [SerializeField] private float dissapationRate;
    [SerializeField] private float dissapateAfter;
    private float dissapateAfterTimer;
    [SerializeField] private float minHeat = .1f;

    [Header("Color")]
    private Color coolColor;
    [SerializeField] private Color hotColor;

    [Header("Emission")]
    [SerializeField] private Vector2 minMaxEmissionIntensity;

    [Header("References")]
    [SerializeField] private new Renderer renderer;
    [SerializeField] private new Rigidbody rigidbody;
    private Material material;
    private Color currentColor;

    [Header("Audio")]
    [SerializeField] private AudioClipContainer onHeatClip;

    private new void Awake()
    {
        // Get a reference to the instantiated material
        material = renderer.material;
        coolColor = material.color;

        base.Awake();
    }

    private void Update()
    {
        // Handle heat
        if (currentHeat <= minHeat)
        {
            currentHeat = 0;
        }

        // Heat is reduced when above 0
        if (dissapateAfterTimer <= 0)
        {
            // Reduce Heat
            currentHeat = Mathf.Lerp(currentHeat, 0, Time.deltaTime * dissapationRate);
        }

        // Handle dissapate after time
        if (dissapateAfterTimer > 0)
            dissapateAfterTimer -= Time.deltaTime;

        // Interpolate Color based on how close we are to overheating
        float percent = currentHeat / maxWithStandableHeat;
        currentColor = Color.Lerp(coolColor, hotColor, percent);
        material.color = currentColor;
        material.SetColor("_EmissionColor", hotColor * Mathf.SmoothStep(minMaxEmissionIntensity.x, minMaxEmissionIntensity.y, percent));
    }

    public override void Damage(float damage)
    {
        // Add heat
        currentHeat += damage;

        // Audio
        onHeatClip.PlayOneShot(source);

        // Check if exeeding heat threshold
        if (currentHeat > maxWithStandableHeat)
        {
            // Call on End
            onEndAction();
        }

        // Set dissapate after timer
        dissapateAfterTimer = dissapateAfter;
    }

    public override void Damage(float damage, Vector3 force)
    {
        Damage(damage);
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
