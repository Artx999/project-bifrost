using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // We can reference like this, because of Singleton
    // Singleton = One class can have only one instance
    public GameManager gameManager;
    public GameObject axe;
    public GameObject sight;

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
        sight.SetActive(false);
    }

    private void Update()
    {
        /* Walk */
        Debug.Log(IsGrounded());
        if (!_gm.axeIsSeperated && IsGrounded() && !_mouseHeldDown)
        {
            float xMovement = Input.GetAxisRaw("Horizontal");
            _rb.velocity = new Vector2(xMovement, 0) * _gm.playerWalkSpeed;
        }
        
        /* Move to axe */
        // Temporary: Right click to move there, in future this will be a rope mechanic
        if(_axeRb.velocity.magnitude <= 0.1f && Input.GetMouseButtonDown(1))
        {
            transform.position = axe.transform.position;
            _rb.velocity = Vector2.zero;
            _gm.axeIsSeperated = false;
        }
        
        /* Axe throw */
        // If the axe is thrown, we dont want to repeat the throw mechanic below
        if (_gm.axeIsSeperated)
            return;
        
        // If the left button is pressed start the throw mechanic here
        if (IsAiming() && IsGrounded() && !_mouseHeldDown)
        {
            _mouseHeldDown = true;
            
            // Stop the players movement completely while aiming
            _rb.velocity = Vector2.zero;
        }

        // While the player is holding down the mouse button (ie. aiming), we show a simple sight in form
        // of a white dot
        else if (Input.GetMouseButton(0) && _mouseHeldDown)
        {
            // Gets the player position, mouse position and calculates the throw vector with these two points
            // This new vector is sent to the show sight method
            Vector2 playerPos = transform.position;
            Vector2 currentMousePos = GetMousePosition();
            Vector2 currentThrowVec = GetThrowVector(playerPos, currentMousePos);
            
            ShowSight(currentThrowVec);
        }
        
        // If the code has been through the above two blocks (ie. left button has been pressed and held down)
        // and been released we start the actual axe throw
        else if(_mouseHeldDown)
        {
            _mouseHeldDown = false;
            _gm.axeIsSeperated = true;
            sight.SetActive(false);

            Vector2 playerPos = transform.position;
            Vector2 newMousePos = GetMousePosition();
            Vector2 throwVec = GetThrowVector(playerPos, newMousePos);
            
            // Reference the axe and apply the speed based on the aim vector
            _axeThrow.ApplyAxeSpeed(throwVec);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            _rb.AddForce(Vector2.up * _gm.playerWallFriction, ForceMode2D.Force);
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
        Debug.DrawLine(transform.position, endPos, Color.green, 3f);

        // We are aiming in the opposite direction of the throw, so we flip the result vector
        return -aimVec;
    }
    
    private bool IsGrounded()
    {
        LayerMask desiredMask = LayerMask.GetMask("Surface");

        Vector2 playerPos = transform.position;
        
        Debug.DrawLine(playerPos, playerPos + Vector2.down * _cc.radius, Color.red);
        
        return Physics2D.Raycast(playerPos, Vector2.down, _cc.radius, desiredMask);
    }

    private bool IsAiming()
    {
        if (Input.GetMouseButton(0))
        {
            // Detect if mouse is hitting 
            RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);

            if (!hit.collider)
                return false;

            // Method will only return true if left mouse is clicking on the player, else everything is false
            if (hit.collider.CompareTag("Player"))
                return true;
        }
        return false;
    }

    private void ShowSight(Vector2 inputVec)
    {
        // If the aim vector is too short for a throw, dont show the dot
        var inputVecMag = inputVec.magnitude;

        if (inputVecMag < _gm.minAxeThrowMag)
        {
            sight.SetActive(false);
            return;
        }
        
        // Since the throw has a max strength, the sight should have a max length, and we do this by limiting
        // the vector magnitude based on the defined max magnitude from GM
        var newMag = Math.Min(_gm.maxAxeThrowMag, inputVecMag);
        inputVec = inputVec.normalized * newMag;
        
        Vector2 playerPos = transform.position;
        
        // Calculate the position with player position and throw vector and activate the sight
        // I am unsure why we multiply bu 0.1f^2, but it works
        sight.transform.position = playerPos + inputVec + Physics2D.gravity * ((float)Math.Pow(0.1f, 2));
        sight.SetActive(true);
    }

    public void CancelThrow()
    {
        _gm.axeIsSeperated = false;
    }
}
