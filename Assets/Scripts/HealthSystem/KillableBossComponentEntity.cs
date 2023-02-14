using UnityEngine;

public class KillableBossComponentEntity : KillableEntity
{
    public ImageSliderBar HPBar { get; private set; }

    public override void Damage(float damage)
    {
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
