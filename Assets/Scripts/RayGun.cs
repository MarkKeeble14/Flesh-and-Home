using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayGun : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_shootRateTimeStamp;
    [SerializeField] private float nextTimeToFire = 0f;
    [SerializeField] private float maxDistance = 1000.0f;
    [SerializeField] private float impactForce = 80f;
    [SerializeField] private float damage = 10f;
    
    private InputManager inputManager;

    [Header("References")]
    public Camera fpsCam;
    public float shootRate = 10f;
    public GameObject m_shotPrefab;
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private ParticleSystem muzzleFlash;

    private bool active;

    // RaycastHit hit;
    
    void Update()
    {
        if (inputManager.PlayerInputActions.Player.Shoot.IsPressed() && Time.time >= nextTimeToFire)
            {
                m_shootRateTimeStamp = Time.time + 1F / shootRate;
                shootRay();
            }
    }
    
    private void Start()
    {
        inputManager = InputManager._Instance;
    }

    private void shootRay()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, maxDistance))
        {
            
            EnemyTarget target = hit.transform.GetComponent<EnemyTarget>();
            if (target != null)
            {
                target.takeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            GameObject laser = Instantiate(m_shotPrefab, transform.position, transform.rotation);
            laser.GetComponent<ShotBehavior>().setTarget(hit.point);
            
            Destroy(laser, 2f);
        }
    }
    
    
}