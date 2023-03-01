using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : KillableEntity
{
    [SerializeField] private ImageSliderBar playerHpBar;
    [SerializeField] private FloatStore hpStore;

    public override float CurrentHealth { get => hpStore.CurrentFloat; set => hpStore.CurrentFloat = value; }
    public override float MaxHealth { get => hpStore.MaxFloat; }

    private void Start()
    {
        SetHPBar();
        AddAdditionalOnEndAction(() => GameManager._Instance.OpenLoseScreen());
    }

    public override void Damage(float damage)
    {
        base.Damage(damage);
        SetHPBar();
    }

    private void SetHPBar()
    {
        playerHpBar.Set(CurrentHealth, MaxHealth);
    }
}
