using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopupText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Set(float num, DamageSource source)
    {
        text.color = UIManager._Instance.GetDamageSourceColor(source);
        text.text = System.Math.Round(num, 2).ToString();
        if (GameManager._Instance.PlayerAimAt == null) Destroy(gameObject);
        transform.rotation = Quaternion.LookRotation(transform.position - GameManager._Instance.PlayerAimAt.position);
    }
}
