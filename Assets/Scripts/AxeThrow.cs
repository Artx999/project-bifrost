using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    private Rigidbody2D _rb;

    private Vector2 _movementVec;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _movementVec * Time.deltaTime);
    }

    public void GiveAxeSpeed(Vector2 playerDrag)
    {
        _movementVec = playerDrag;
    }
}
