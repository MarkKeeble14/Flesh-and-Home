using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTestEventInvoker : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        var trg = collision.gameObject.GetComponent<TriggerIntermediary>();
        if(trg != null)
        {
            trg.Act();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
