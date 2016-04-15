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

    Package mPackage;
    Crease mCrease;
    float Length = 1.2f;
    float Width = 1.0f;
    float Depth = 2.0f;

    float destLength = 1.2f;
    float destWidth = 1.0f;
    float destDepth = 2.0f;

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

        Length = EditorGUILayout.FloatField("Length", Length);///.Dimension.x);
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

        destLength = EditorGUILayout.FloatField("Length", destLength);///.Dimension.x);
        destWidth = EditorGUILayout.FloatField("Width", destWidth);//.Dimension.z);
        destDepth = EditorGUILayout.FloatField("Depth", destDepth);//.Dimension.y);

        if (GUILayout.Button("Resize"))
        {
            OnResize(destLength, destWidth, destDepth);
        }

        if (GUILayout.Button("Save"))
        {
            string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
            if (path.Length != 0)
            {

            }
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

        mPackage = obj.AddComponent<Package>();
        mPackage.OnInit(Length, Width, Depth);

        mCrease = obj.AddComponent<Crease>();
        mCrease.OnInit();
    }

    void OnResize(float Length, float Width, float Depth)
    {
        /*if (l == Length && w == Width && d == Depth)
            throw new System.Exception("THE DIMENSION IS SAME AS INITIAL!");*/
        mPackage.OnResize(Length, Width, Depth);

    }

    static bool initinalSettings
    {
        get { return EditorPrefs.GetBool("PackageEditor_initinalSettings", false); }
        set { EditorPrefs.SetBool("PackageEditor_initinalSettings", value); }
    }
}
