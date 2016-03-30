using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Imported Die-line Data
/// </summary>
[Serializable]
public class PackageData
{
    
    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }
    public float Thickness { set; get; }

    public List<SerializableVector3> Vertices { set; get; }

    public List<PanelData> Panels { set; get; }

    public List<CreaseData> Creases { set; get; }

}

[Serializable]
public class PanelData
{
    /// <summary>
    /// Grid_Y Value
    /// </summary>
    public int Row { set; get; }

    /// <summary>
    /// Grid_x Value
    /// </summary>
    public int Col { set; get; }

    //public RectOffset Border { set; get; }
    public float Left { set; get; }
    public float Right { set; get; }
    public float Top { set; get; }
    public float Bottom { set; get; }

    public List<SerializableVector3> Vertices { set; get; }
    
}

[Serializable]
public class CreaseData
{

    public List<int> Indices { set; get; }
    public float FoldAngle { set; get; }

}