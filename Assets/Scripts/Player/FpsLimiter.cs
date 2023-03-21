using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsLimiter : MonoBehaviour
{

    [SerializeField]
    private int frameRate = 60;
    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = frameRate;
    }


}
