using UnityEngine;
using System.Collections;

public class Main_V10 : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Rect rect = new Rect(0, 0, 10, 10);
        var offset = new RectOffset(1, 1, 1, 1);

        rect = offset.Remove(rect);

        Debug.Log(rect);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
