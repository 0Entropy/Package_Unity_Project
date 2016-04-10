﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class _Extensions {
    public static Vector2[] Vector2Array(this Rect rect)
    {
        return new Vector2[]{
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMin, rect.yMax),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMax,  rect.yMin)
            };
    }

    public static Vector3[] Vector3Array(this Rect rect)
    {
        return new Vector3[]{
                    new Vector2(rect.xMin, rect.yMin),
                    new Vector2(rect.xMin, rect.yMax),
                    new Vector2(rect.xMax, rect.yMax),
                    new Vector2(rect.xMax,  rect.yMin)
            };
    }

    public static Rect BorderRect(this List<Vector2> vertices)
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);

        return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
    }

    public static Rect BorderRect(this List<Vector3> vertices)
    {
        var min_x = vertices.Min(p => p.x);
        var max_x = vertices.Max(p => p.x);
        var min_y = vertices.Min(p => p.y);
        var max_y = vertices.Max(p => p.y);

        return Rect.MinMaxRect(min_x, min_y, max_x, max_y);
    }
}
