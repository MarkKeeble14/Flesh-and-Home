using UnityEngine;

public class KillableBossComponentEntity : KillableEntity
{
    public ImageSliderBar HPBar { get; private set; }

    public override void Damage(float damage, DamageSource source)
    {
        if (HPBar != null)
            HPBar.Set(CurrentHealth, MaxHealth);
        base.Damage(damage, source);
    }
    public void SetHPBar(ImageSliderBar hpBar)
    {
        HPBar = hpBar;
        HPBar.Set(CurrentHealth, MaxHealth);
    }
}
