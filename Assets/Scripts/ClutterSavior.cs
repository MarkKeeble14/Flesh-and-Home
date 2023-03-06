using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutterSavior : MonoBehaviour
{
    public static ClutterSavior _Instance;

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _Instance = this;
        }
    }
}
