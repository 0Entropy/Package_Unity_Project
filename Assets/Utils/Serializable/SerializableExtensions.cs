using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class SerializableExtensions
{
    public static SerializableVector2[] ToSerializableVector2Array(this List<Vector2> listVector2)
    {
        return listVector2.Select(p => (SerializableVector2)p).ToArray();
    }

    public static SerializableVector2[] ToSerializableVector2Array(this Vector2[] listVector2)
    {
        return listVector2.Select(p => (SerializableVector2)p).ToArray();
    }

    public static SerializableVector3[] ToSerializableVector3Array(this Vector3[] listVector3)
    {
        
        return listVector3.Select(p => (SerializableVector3)p).ToArray();
    }

    public static Vector3[] ToVector3Array(this SerializableVector3[] src)

    {
        return src.Select(p => (Vector3)p).ToArray();
    }

    public static Vector2[] ToVector2Array(this SerializableVector2[] src)

    {
        return src.Select(p => (Vector2)p).ToArray();
    }
}
