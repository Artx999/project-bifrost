using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
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
        Vector2 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        return hit.collider == this._collider;
    }
}
