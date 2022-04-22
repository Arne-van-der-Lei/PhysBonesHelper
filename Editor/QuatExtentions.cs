using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionExt
{
    public static Quaternion GetNormalized(this Quaternion q)
    {
        float f = 1f / Mathf.Sqrt(q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w);
        return new Quaternion(q.x * f, q.y * f, q.z * f, q.w * f);
    }
}