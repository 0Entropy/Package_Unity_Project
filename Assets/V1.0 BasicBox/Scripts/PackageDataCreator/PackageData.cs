using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PackageData
{
    
    public float Length { set; get; }
    public float Width { set; get; }
    public float Depth { set; get; }

    public float Thickness { set; get; }

    public List<PanelData> Panels { set; get; }

    public List<CreaseData> Creases { set; get; }

    public override string ToString()
    {
        return string.Format("Dimension : [L:{0}, W:{1}, D:{2}]; Panels:{3}; Creases:{4}",
            Length, Width, Depth, Panels.Count, Creases.Count);
    }

}

[Serializable]
public class PanelData
{
    public int Row { set; get; }
    public int Col { set; get; }

    public float Left { set; get; }
    public float Right { set; get; }
    public float Top { set; get; }
    public float Bottom { set; get; }

    public float VarRight { set; get; }
    public float VarLeft { set; get; }
    public float VarTop { set; get; }
    public float VarBottom { set; get; }

    public List<SerializableVector2> Vertices { set; get; }

    public List<SerializableVector2> Alphas { set; get; }
    
}

[Serializable]
public class CreaseData
{
    
    public List<SerializableVector3> Vertices { set; get; }
    public List<List<int>> Neighbors { set; get; }
    public float FoldAngle { set; get; }
    
}