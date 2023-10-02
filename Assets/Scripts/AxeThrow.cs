using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    public float maxInitAxeSpeed = 6f;
    public float minInitAxeSpeed = 1f;
    public float axeSpeedAmp;
    
    private Rigidbody2D _rb;
    private Vector2 _movementVec;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    public void GiveAxeSpeed(Vector2 playerDrag)
    {
        var inputMagnitude = playerDrag.magnitude;

        // If the throw vector is too short, we cancel the throw
        if (inputMagnitude < minInitAxeSpeed)
            return;
        
        // If a successful throw, apply gravity
        _rb.gravityScale = 1f;
        
        float realSpeed = Math.Min(maxInitAxeSpeed, inputMagnitude);
        Debug.Log("Current axe speed:" + realSpeed);
        
        _movementVec = playerDrag.normalized * (realSpeed * axeSpeedAmp);
        
        // Since projectile, we add a force and let gravity do its thing
        _rb.AddForce(_movementVec, ForceMode2D.Impulse);
    }
}
