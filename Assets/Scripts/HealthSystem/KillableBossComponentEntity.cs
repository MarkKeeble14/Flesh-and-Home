using UnityEngine;

public class KillableBossComponentEntity : KillableEntity
{
    public bool AcceptDamage { get; set; }
    public ImageSliderBar HPBar { get; private set; }

    public override void Damage(float damage)
    {
        if (!AcceptDamage) return;
        if (HPBar != null)
            HPBar.Set(currentHealth, maxHealth);
        base.Damage(damage);
    }
    public void SetHPBar(ImageSliderBar hpBar)
    {
        HPBar = hpBar;
        HPBar.Set(currentHealth, maxHealth);
    }
}
