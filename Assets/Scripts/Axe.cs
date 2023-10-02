using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
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
    private GameManager _gm;
    private Vector2 _movementVec;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Initialize variables
        _rb = GetComponent<Rigidbody2D>();
        _gm = gameManager.GetComponent<GameManager>();
        
        _rb.gravityScale = 0f;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update()
    {
        // If the axe is at the player, the axe should follow the player around
        // the moment the axe is thrown, the collider is activated
        if (!_gm.axeIsSeperated)
            transform.position = player.transform.position;
        else
            GetComponent<BoxCollider2D>().enabled = true;
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
