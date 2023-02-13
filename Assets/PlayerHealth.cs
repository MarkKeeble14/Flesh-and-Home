using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : KillableEntity
{
    [SerializeField] private ImageSliderBar playerHpBar;

    private void Start()
    {
        SetHPBar();
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);
        SetHPBar();
    }

    private void SetHPBar()
    {
        playerHpBar.Set(currentHealth, maxHealth);
    }
}
