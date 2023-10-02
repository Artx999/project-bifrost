using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // We can reference the axe like this, because of Singleton
    // Singleton = One class can have only one instance
    public GameManager gameManager;
    public GameObject axe;

    private GameManager _gm;
    private Rigidbody2D _rb;
    private Axe _axeThrow;
    private Rigidbody2D _axeRb;
    private Vector2 _initialAxePos;
    private bool _mouseHeldDown;
    
    private void Start()
    {
        // Initialize variables
        _gm = gameManager.GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        _axeThrow = axe.GetComponent<Axe>();
        _axeRb = axe.GetComponent<Rigidbody2D>();
        _initialAxePos = transform.position;
        _mouseHeldDown = _gm.axeThrown = false;
    }

    private void Update()
    {
        /* Axe throw */
        
        // If the axe is thrown, we dont want to repeat the throw mechanic below
        if (_gm.axeThrown)
            return;
        
        // If the left button is pressed start the throw mechanic here
        if (Input.GetMouseButton(0) && !_mouseHeldDown)
        {
            // Detect if mouse is hitting 
            RaycastHit2D hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);
            
            if (!hit.collider || !hit.collider.CompareTag("Player"))
            {
                //Debug.Log("ERROR: Player not registered.");
                return;
            } 
            //Debug.Log("Success: Player hit.");

            _mouseHeldDown = true;
        }
        
        // Does nothing, as we want nothing to happen while the button is being held down
        else if (Input.GetMouseButton(0)) {}
        
        // If the code has been through the above two blocks (ie. left button has been pressed and held down)
        // and been released we start the actual axe throw
        else if(_mouseHeldDown)
        {
            _mouseHeldDown = false;
            _gm.axeThrown = true;

            Vector2 newMousePos = GetMousePosition();
            
            // Reference the axe and apply the speed based on the aim vector
            _axeThrow.ApplyAxeSpeed(GetThrowVector(_initialAxePos, newMousePos));
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
        Debug.DrawLine(_initialAxePos, endPos, Color.green, 5f);

        // We are aiming in the opposite direction of the throw, so we flip the result vector
        return -aimVec;
    }
}
