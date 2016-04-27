using UnityEngine;
using System.Collections;

public class RenderTextureContext : MonoBehaviour
{
    public Camera contentCamera;

    public delegate void OnSelectEvent(int contentID);
    public static event OnSelectEvent OnSelect;

    public void Attach(Texture2D img, int id) { }

    public void Attach(TextContent text, int id) { }

    void OnEnable()
    {
        _InputManager.Instance.Init();

        _InputManager.OnFocus += _InputManager_OnFocus;
        _InputManager.OnClick += _InputManager_OnClick;
    }

    private void _InputManager_OnClick(Vector3 position)
    {

        Debug.Log("Handle On Click !");
        // throw new System.NotImplementedException();
    }

    void OnDisable()
    {
        _InputManager.OnFocus -= _InputManager_OnFocus;
        _InputManager.OnClick -= _InputManager_OnClick;
    }

    private void _InputManager_OnFocus(GameObject obj, Vector2 pos)
    {
        //throw new System.NotImplementedException();
        Debug.Log(obj.name);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
