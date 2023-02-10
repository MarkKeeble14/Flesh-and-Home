using UnityEngine;

public class TransformHelper
{
    // Shakes the passed in transform
    public static void ShakeTransform(Transform transform, float xStart, float zStart, float shakeSpeed, float shakeStrength)
    {
        float xVal = Mathf.Sin(xStart + Time.time * shakeSpeed) * shakeStrength;
        float zVal = Mathf.Sin(zStart + Time.time * shakeSpeed) * shakeStrength;
        transform.position = new Vector3(
            transform.position.x + xVal,
            transform.position.y,
            transform.position.z + zVal);
    }
}
