using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    Transform Player;
    public float dist;
    public float maxDist;
    public Transform head, muzzle;
    public float fireRate, nextFire;
    
    private LayerMask canHit;
    private float damage = 1.0f;
    
    public float range = 100f;
    public DamageSource damageSource = DamageSource.RIFLE;
    public LayerMask raycastLayerMask;
    public float lineDisplayTime = 0.1f; // Duration to display the line
    public Material lineMaterial; // Material for the LineRenderer

    private LineRenderer lineRenderer;

    
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("PlayerObject").transform;
        // Initialize the LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(Player.position, transform.position);
        if (dist <= maxDist)
        {
            head.LookAt(Player);
            if (Time.time >= nextFire)
            {
                nextFire = Time.time + 1f / fireRate;
                shoot();
            }
        }
    }
    
    void shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, range, raycastLayerMask))
        {
            // Display the line
            lineRenderer.SetPosition(0, muzzle.position);
            lineRenderer.SetPosition(1, hit.point);
            lineRenderer.enabled = true;
            StartCoroutine(HideLineAfterDelay(lineDisplayTime));
            
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damage, damageSource);
                Debug.Log("Damaged: " + damageable);
            }
        }
    }
    
    IEnumerator HideLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.enabled = false;
    }
    
}
