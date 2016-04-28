using UnityEngine;
using System.Collections;

public static class _TransformExtensions
{

    public static TransformData SaveLocal(this Transform aTransform)
    {
        return new TransformData()
        {
            position = aTransform.localPosition,
            rotation = aTransform.localRotation,
            localScale = aTransform.localScale
        };
    }
    public static TransformData SaveWorld(this Transform aTransform)
    {
        return new TransformData()
        {
            position = aTransform.position,
            rotation = aTransform.rotation,
            localScale = aTransform.localScale
        };
    }

    public static void LoadLocal(this Transform aTransform, TransformData aData)
    {
        aTransform.localPosition = aData.position;
        aTransform.localRotation = aData.rotation;
        aTransform.localScale = aData.localScale;
    }

    public static void LoadWorld(this Transform aTransform, TransformData aData)
    {
        aTransform.position = aData.position;
        aTransform.rotation = aData.rotation;
        aTransform.localScale = aData.localScale;
    }

    public static void CopyFrom(this Transform aTransform, Transform bTransform)
    {
        if (bTransform.parent)
        {

            aTransform.SetParent(bTransform.parent);
            aTransform.localPosition = bTransform.localPosition;
            aTransform.localRotation = bTransform.localRotation;
            aTransform.localScale = bTransform.localScale;
        }
        else
        {

            aTransform.position = bTransform.position;
            aTransform.rotation = bTransform.rotation;
            aTransform.localScale = bTransform.localScale;
        }
    }

    public static void CopyFrom(this Transform aTransform, GameObject Obj)
    {
        aTransform.CopyFrom(Obj.transform);
    }

}

public class TransformData
{
    public Vector3 position { set; get; }
    public Quaternion rotation { set; get; }
    public Vector3 localScale { set; get; }
}
