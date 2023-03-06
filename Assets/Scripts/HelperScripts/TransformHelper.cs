using System;
using System.Collections.Generic;
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

    public static Transform GetClosestTransformToTransform(Transform transform, Collider[] cols)
    {
        if (cols.Length == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                < Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }

    public static float FindGroundPoint(Transform transform)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground"));
        return hit.point.y;
    }

    public static Transform GetFurthestTransformToTransform(Transform transform, Collider[] cols)
    {
        if (cols.Length == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                > Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }

    public static Transform GetClosestTransformToTransform(Transform transform, List<Collider> cols)
    {
        if (cols.Count == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                < Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }

    public static Transform GetFurthestTransformToTransform(Transform transform, List<Collider> cols)
    {
        if (cols.Count == 0) return null;

        Transform toReturn = null;
        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i] == null)
            {
                continue;
            }
            if (toReturn == null)
            {
                toReturn = cols[i].transform;
                continue;
            }
            if (Vector3.Distance(transform.position, cols[i].transform.position)
                > Vector3.Distance(transform.position, toReturn.position))
            {
                toReturn = cols[i].transform;
            }
        }
        return toReturn;
    }
}
