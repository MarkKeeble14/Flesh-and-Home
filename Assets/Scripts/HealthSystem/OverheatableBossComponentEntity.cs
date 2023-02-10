using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatableBossComponentEntity : OverheatableEntity
{
    public bool AcceptDamage { get; set; }
    public ImageSliderBar HPBar { get; private set; }

    public override void Damage(float damage)
    {
        if (!AcceptDamage) return;
        if (HPBar != null)
            HPBar.Set(currentHeat, maxWithStandableHeat);
        base.Damage(damage);
    }

    public virtual void SetHPBar(ImageSliderBar hpBar)
    {
        HPBar = hpBar;
        HPBar.Set(currentHeat, maxWithStandableHeat);
    }
}
