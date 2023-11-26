using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private Camera _mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody.MovePosition(_mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }
}
