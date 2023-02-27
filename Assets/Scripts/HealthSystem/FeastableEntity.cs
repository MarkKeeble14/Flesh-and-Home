using UnityEngine;

public class FeastableEntity : KillableEntity
{
    [SerializeField] private int feastableLayer = 19;
    [SerializeField] private float corpseHP;
    private bool isCorpse;
    [SerializeField] private Color corpseColor;

    private bool targeted;
    public bool Targeted
    {
        get
        {
            return targeted;
        }
        set
        {
            targeted = value;
        }
    }

    protected override void OnEnd()
    {
        base.OnEnd();

        if (!isCorpse)
        {
            gameObject.layer = feastableLayer;
            maxHealth = corpseHP;
            currentHealth = corpseHP;

            Renderer renderer = GetComponent<Renderer>();
            renderer.material.DisableKeyword("_EMISSION");
            renderer.material.color = corpseColor;

            acceptDamage = true;
            isCorpse = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void FeastOn()
    {
        // Debug.Log(name + " Was Feasted On");
        Destroy(gameObject);
    }
}
