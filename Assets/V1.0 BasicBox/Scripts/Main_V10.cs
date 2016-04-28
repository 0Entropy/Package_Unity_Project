using UnityEngine;
using System.Collections.Generic;

public class Main_V10 : MonoBehaviour {

	// Use this for initialization
	void Start () {

        /*Rect rect = new Rect(0, 0, 10, 10);
        var offset = new RectOffset(1, 1, 1, 1);

        rect = offset.Remove(rect);

        Debug.Log(rect);*/

        /*PackageData data = new PackageData();
        data.Panels = new List<PanelData>();
        data.Panels.Add(new PanelData());
        data.Creases = new List<CreaseData>();
        data.Creases.Add(new CreaseData());
        var dataString = JsonFx.Json.JsonWriter.Serialize(data);*/
        Debug.Log("zero-zero " + Mathf.Clamp((0.0f/0.000001f), 0.0F, 1.0F));

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
