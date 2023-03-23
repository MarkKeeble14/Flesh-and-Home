using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheatableBossComponentEntity : OverheatableEntity
{
    public ImageSliderBar HPBar { get; private set; }

    public override void Damage(float damage, DamageSource source)
    {
        if (HPBar != null)
            HPBar.Set(currentHeat, overheatSettings.overheatAfter);
        base.Damage(damage, source);
    }

    public virtual void SetHPBar(ImageSliderBar hpBar)
    {
        HPBar = hpBar;
        HPBar.Set(currentHeat, overheatSettings.overheatAfter);
    }

    public void CoolOff(float rate)
    {
        StartCoroutine(Cool(rate));
    }

    private IEnumerator Cool(float rate)
    {
        while (currentHeat > 0)
        {
            currentHeat = Mathf.MoveTowards(currentHeat, 0, Time.deltaTime * overheatSettings.heatDissapationRate * rate);
            yield return null;
        }
    }
}
