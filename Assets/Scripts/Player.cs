using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // We can reference the axe like this, because of Singleton
    // Singleton = One class can have only one instance
    public GameManager gameManager;
    public GameObject axe;
    public float friction;
    public float walkSpeed;

    private GameManager _gm;
    private Rigidbody2D _rb;
    private CircleCollider2D _cc;
    private Axe _axeThrow;
    private Rigidbody2D _axeRb;
    private bool _mouseHeldDown;
    
    private void Start()
    {
        // Initialize variables
        _gm = gameManager.GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        _cc = GetComponent<CircleCollider2D>();
        _axeThrow = axe.GetComponent<Axe>();
        _axeRb = axe.GetComponent<Rigidbody2D>();
        _mouseHeldDown = _gm.axeIsSeperated = false;
    }

    private void Update()
    {
        /* Walk */
        if (!_gm.axeIsSeperated && IsGrounded() && !_mouseHeldDown)
        {
            float xMovement = Input.GetAxisRaw("Horizontal");
            _rb.velocity = new Vector2(xMovement, 0) * walkSpeed;
        }
        
        /* Move to axe */
        if(_axeRb.velocity.magnitude <= 0.1f && Input.GetMouseButtonDown(1))
        {
            transform.position = axe.transform.position;
            _gm.axeIsSeperated = false;
        }
        
        /* Axe throw */
        // If the axe is thrown, we dont want to repeat the throw mechanic below
        if (_gm.axeIsSeperated)
            return;
        
        // If the left button is pressed start the throw mechanic here
        if (IsAiming() && !_mouseHeldDown)
        {
            _mouseHeldDown = true;
            
            // Stop the players movement completely while aiming
            _rb.velocity = Vector2.zero;
        }

        // Does nothing, as we want nothing to happen while the button is being held down
        else if (Input.GetMouseButton(0)) {}
        
        // If the code has been through the above two blocks (ie. left button has been pressed and held down)
        // and been released we start the actual axe throw
        else if(_mouseHeldDown)
        {
            _mouseHeldDown = false;
            _gm.axeIsSeperated = true;

            Vector2 newMousePos = GetMousePosition();
            
            // Reference the axe and apply the speed based on the aim vector
            _axeThrow.ApplyAxeSpeed(GetThrowVector(transform.position, newMousePos));
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            _rb.AddForce(Vector2.up * friction, ForceMode2D.Force);
        }
    }

    private Vector2 GetMousePosition()
    {
        if(Camera.main != null)
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        return Vector2.zero;
    }

    private Vector2 GetThrowVector(Vector2 startPos, Vector2 endPos)
    {
        // Since vectors are lines from origin to a specified point, we need to move
        // the "wrong " vector (from player to mouse position) to have origin in "origin"
        Vector2 aimVec = endPos - startPos;
        Debug.DrawLine(transform.position, endPos, Color.green, 5f);

        // We are aiming in the opposite direction of the throw, so we flip the result vector
        return -aimVec;
    }
    
    private bool IsGrounded()
    {
        LayerMask mask = LayerMask.GetMask("Surface");
        
        return Physics2D.Raycast(transform.position, Vector2.down, _cc.radius, mask);
    }

    private bool IsAiming()
    {
        if (Input.GetMouseButton(0))
        {
            // Detect if mouse is hitting 
            RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);

            if (!hit.collider)
                return false;

            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }
}
