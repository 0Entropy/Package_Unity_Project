using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TextMesh))]
public class TextMaterialHandler : MonoBehaviour
{
    
    public Texture fontTexture
    {
        get
        {
            return GetComponent<TextMesh>().font.material.mainTexture;
            
        }
    }
    
    public void SetShaderByURL(string url)
    {

        /*mat.mainTexture = fontTexture;*/
        var shader = Shader.Find(url);
        GetComponent<MeshRenderer>().material.shader = Shader.Find(url);
        //GetComponent<MeshRenderer>().material.mainTexture = fontTexture;

    }

    public void SetMaterial(Material mat)
    {

        mat.mainTexture = fontTexture;
        GetComponent<MeshRenderer>().material = mat;

    }

}
