using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public AnimationClip buttonDefault;
    public AnimationClip buttonHover;

    private Camera _camera;
    private Animator _animator;
    private PolygonCollider2D _collider;
    
    // Start is called before the first frame update
    void Start()
    {
        this._camera = Camera.main;
        this._animator = GetComponent<Animator>();
        this._collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var test = OnMouseHover();
        this._animator.SetBool("mouseHover", test);
    }

    private bool OnMouseHover()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = _camera.nearClipPlane;
        var worldPosition = _camera.ScreenToWorldPoint(mousePosition);
        Debug.Log(worldPosition);
        
        var hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        return hit.collider == this._collider;
    }
}
