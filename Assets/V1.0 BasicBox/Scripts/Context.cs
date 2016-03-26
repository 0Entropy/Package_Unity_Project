using UnityEngine;
using System.Collections;

public class Context {

    Package2D mPackage;

	public Context()
    {

    }

    

    /// <summary>
    /// Initialize the package.
    /// 
    /// </summary>
    void OnInit() { }

    public void OnUpdateSize(float l = 1, float w = 1, float d = 1, float t = 0) { }

    public void OnUpdateLength(float l) { }

    public void OnUpdateWidth(float w) { }

    public void OnUpdateDepth(float d) { }



    public void OnUpdateType() { }


    public void OnShow2D() { }

    public void OnShow3D() { }
}
