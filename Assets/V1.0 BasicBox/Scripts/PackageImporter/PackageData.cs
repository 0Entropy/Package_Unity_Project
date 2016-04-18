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

}

[Serializable]
public class PanelData
{
    public float Left { set; get; }
    public float Right { set; get; }
    public float Top { set; get; }
    public float Bottom { set; get; }

    public List<SerializableVector3> Vertices { set; get; }

    public List<SerializableVector2> Alphas { set; get; }
    
}

[Serializable]
public class CreaseData
{

    //public List<int> Indices { set; get; }
    public List<SerializableVector3> Vertices { set; get; }
    public List<List<int>> Neighbors { set; get; }
    public float FoldAngle { set; get; }
    

}