using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooting : MonoBehaviour
{
    private InputManager inputManager;
    
    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;
    
    private void Start()
    {
        inputManager = InputManager._Instance;
        // inputManager.PlayerInputActions.Player.FireAttachment.started += Fire;
    }

    private IEnumerator Fire()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
            {
             Debug.Log(hit.transform.name);   
            }
        }
    }
}
