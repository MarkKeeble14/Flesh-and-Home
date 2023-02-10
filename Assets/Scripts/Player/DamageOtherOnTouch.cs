using UnityEngine;

public class DamageOtherOnTouch : MonoBehaviour, IDamageOthers
{
    [SerializeField] private float damage;
    [SerializeField] private float tickRate;
    private TimerDictionary<IDamageable> hasDamagedRecently = new TimerDictionary<IDamageable>();
    [SerializeField] private LayerMask dealDamageTo;

    public void DealDamage(IDamageable damageable)
    {
        damageable.Damage(damage);
        hasDamagedRecently.Add(damageable, tickRate);
    }

    private void Update()
    {
        hasDamagedRecently.Update();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!LayerMaskHelper.IsInLayerMask(other.gameObject, dealDamageTo)) return;
        if (other.TryGetComponent(out IDamageable damageable)
            && !hasDamagedRecently.ContainsKey(damageable))
        {
            DealDamage(damageable);
        }
    }
}
