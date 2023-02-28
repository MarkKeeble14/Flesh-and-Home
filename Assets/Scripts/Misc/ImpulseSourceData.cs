using UnityEngine;
using Cinemachine;

[System.Serializable]
public struct ImpulseSourceData
{
    public CinemachineImpulseSource collideWithPlayerImpulseSource;
    public Vector2 verticalMinMaxImpulse;
    public Vector2 horizontalMinMaxImpulse;
    public float impulseMultiplier;
}
