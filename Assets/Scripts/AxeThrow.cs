using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    public float maxInitAxeSpeed = 6f;
    public float minInitAxeSpeed = 1f;
    public float axeSpeedAmp = 3f;
    
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
        // Throw movement
        _rb.velocity = _movementVec;
    }

    public void GiveAxeSpeed(Vector2 playerDrag)
    {
        float inputMagnitude = playerDrag.magnitude;

        // To throw or not
        if (inputMagnitude < minInitAxeSpeed)
            return;
        
        float realSpeed = Math.Min(maxInitAxeSpeed, inputMagnitude);
        Debug.Log("Current axe speed:" + realSpeed);
        
        _movementVec = playerDrag.normalized * (realSpeed * axeSpeedAmp);
    }
}
