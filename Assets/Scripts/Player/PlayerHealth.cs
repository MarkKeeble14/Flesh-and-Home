using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : KillableEntity
{
    [SerializeField] private ImageSliderBar playerHpBar;
    [SerializeField] private FloatStore hpStore;

    public override float CurrentHealth { get => hpStore.CurrentFloat; set => hpStore.CurrentFloat = value; }
    public override float MaxHealth { get => hpStore.MaxFloat; }

    public override void Damage(float damage, DamageSource source)
    {
        BloodSpatterSelector._Instance.CallSpatter();
        base.Damage(damage, source);
    }

    private void Start()
    {
        AddAdditionalOnEndAction(() => GameManager._Instance.OpenLoseScreen());
    }

    private void SetHPBar()
    {
        playerHpBar.Set(CurrentHealth, MaxHealth);
    }

    private void Update()
    {
        SetHPBar();
    }
}
