using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public GameObject gameManager;
    public Player player;
    public float maxVecMagnitude;
    public float minVecMagnitude;
    public float axeSpeedAmp;
    
    private Rigidbody2D _rb;
    private Vector2 _movementVec;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Initialize variables
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            Debug.Log("Hit wall!");
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0f;
        }
    } 

    public void ApplyAxeSpeed(Vector2 inputVec)
    {
        var inputMagnitude = inputVec.magnitude;

        // If the throw vector is too short, we cancel the throw
        if (inputMagnitude < minVecMagnitude)
            return;
        
        // If a successful throw, apply gravity
        _rb.gravityScale = 1f;
        
        // Fix up the throw vector, by making a new vector with a direction and giving a capped speed
        float realSpeed = Math.Min(maxVecMagnitude, inputMagnitude);
        _movementVec = inputVec.normalized * (realSpeed * axeSpeedAmp);
        
        // Lastly, we add a force and let gravity do its thing
        _rb.AddForce(_movementVec, ForceMode2D.Impulse);
    }
}
