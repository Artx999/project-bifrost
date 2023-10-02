using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO:
// 1. Have a max speed and a minimum speed
// 2. Apply gravity
// 3. Add colliders for the axe to hit
// 4. Add sight
// 5. 
//


public class Player : MonoBehaviour
{
    public GameObject axe;
    
    private AxeThrow _axeThrow;

    private bool _mouseHeldDown = false;
    private Vector2 _initialAxePos;
    
    void Start()
    {
        _axeThrow = axe.GetComponent<AxeThrow>();

        _initialAxePos = transform.position;
    }

    void Update()
    {
        /* Reload scene (should be in a Game Manager, this for test) */
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        /* Axe throw */
        // If the left button is pressed start the throw mechanic here
        if (Input.GetMouseButton(0) && !_mouseHeldDown)
        {
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
        else if (Input.GetMouseButton(0))
        {
            //Debug.Log("Mouse is being held down.");
        }
        // If the code has been through the above two blocks (ie. left button has been pressed and held down)
        // and been released we start the actual axe throw
        else if(_mouseHeldDown)
        {
            //Debug.Log("Mouse released.");
            _mouseHeldDown = false;

            Vector2 newMousePos = GetMousePosition();
            
            _axeThrow.GiveAxeSpeed(DrawVector2(_initialAxePos, newMousePos));
        }
    }

    private Vector2 GetMousePosition()
    {
        if(Camera.main != null)
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        return Vector2.zero;
    }

    private Vector2 DrawVector2(Vector2 startPos, Vector2 endPos)
    {
        // Since vectors are lines from origin to a specified point, we need to move the "wrong " vector (from
        // player to mouse position) to 
        Vector2 result = endPos - startPos;
        Debug.DrawLine(_initialAxePos, endPos, Color.green, 5f);

        return -result;
    }
}
