using UnityEngine;
using UnityEditor;
using System.Collections;

public class PackageImportWindow : EditorWindow
{
    string packageID = string.Empty;
    string packageName = string.Empty;
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    GameObject srcMeshObj;

    PackageImporter mPackage;
    float Length = 1.2f;
    float Width = 1.0f;
    float Depth = 2.0f;

    float newLength, newWidth, newDepth;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Package Import Window")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PackageImportWindow window = (PackageImportWindow)EditorWindow.GetWindow(typeof(PackageImportWindow));
        window.Show();
    }

    void OnGUI()
    {

        GUILayout.Label("Initial Settings", EditorStyles.boldLabel);

        srcMeshObj = (GameObject)EditorGUILayout.ObjectField("Mesh Prefab ", srcMeshObj, typeof(GameObject));

        if (!srcMeshObj)
            return;

        Length = EditorGUILayout.FloatField ("Length", Length);///.Dimension.x);
        Width = EditorGUILayout.FloatField("Width", Width);//.Dimension.z);
        Depth = EditorGUILayout.FloatField("Depth", Depth);//.Dimension.y);

        if (GUILayout.Button("Initial"))
        {
            //             ImPackage.InitDim = initDim;
            //             ImPackage.InitMesh = meshObj;
            //             ImPackage.OnInit();
            OnInit();
        }

        if (!mPackage)
            return;

        

        GUILayout.Space(8);
        GUILayout.Label("Resize Settings", EditorStyles.boldLabel);

        newLength = EditorGUILayout.FloatField("Length", newLength);///.Dimension.x);
        newWidth = EditorGUILayout.FloatField("Width", newWidth);//.Dimension.z);
        newDepth = EditorGUILayout.FloatField("Depth", newDepth);//.Dimension.y);

        if (GUILayout.Button("Resize"))
        {
            OnResize(newLength, newWidth, newDepth);
        }
        packageID = EditorGUILayout.TextField("Package ID", srcMeshObj.name);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }

    void OnInit()
    {
        if (!srcMeshObj)
            return;
        if (mPackage)
            DestroyImmediate(mPackage.gameObject);
        var obj = Instantiate(srcMeshObj);

        mPackage = obj.AddComponent<PackageImporter>();
        mPackage.OnInit(Length, Width, Depth);
    }

    void OnResize(float l, float w, float d)
    {
        if (l == Length && w == Width && d == Depth)
            throw new System.Exception("THE DIMENSION IS SAME AS INITIAL!");
        mPackage.OnResize(l, w, d);

    }

    static bool initinalSettings
    {
        get { return EditorPrefs.GetBool("PackageEditor_initinalSettings", false); }
        set { EditorPrefs.SetBool("PackageEditor_initinalSettings", value); }
    }
}
