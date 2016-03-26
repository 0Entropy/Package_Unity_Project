using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class SerializableExtensions
{
    public static SerializableVector2[] ToSerializableVector2Array(this List<Vector2> listVector2)
    {
        List<SerializableVector2> result = new List<SerializableVector2>();
        foreach (var pt in listVector2.Select(p => new SerializableVector2(p.x, p.y)))
        {
            result.Add(pt);
        }
        return result.ToArray();
    }

    public static SerializableVector2[] ToSerializableVector2Array(this Vector2[] listVector2)
    {
        List<SerializableVector2> result = new List<SerializableVector2>();
        foreach (var pt in listVector2.Select(p => new SerializableVector2(p.x, p.y)))
        {
            result.Add(pt);
        }
        return result.ToArray();
    }

    public static SerializableVector3[] ToSerializableVector3Array(this Vector3[] listVector2)
    {
        List<SerializableVector3> result = new List<SerializableVector3>();
        foreach (var pt in listVector2.Select(p => new SerializableVector3(p.x, p.y, p.z)))
        {
            result.Add(pt);
        }
        return result.ToArray();
    }

    public static Vector3[] ToVector3Array(this SerializableVector3[] src)

    {
        Vector3[] result = new Vector3[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            result[i] = (Vector3)src[i];
        }
        return result;
    }

    public static Vector2[] ToVector2Array(this SerializableVector2[] src)

    {
        Vector2[] result = new Vector2[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            result[i] = (Vector2)src[i];
        }
        return result;
    }
}
