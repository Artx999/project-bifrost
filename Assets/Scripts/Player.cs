using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Grounded,
        Fall,
        GroundedAim,
        AxeThrow,
        AxeStuck,
        RopeClimb,
        WallSlide,
        WallAim
    }

    public PlayerState _currentState;
    
    // We can reference like this, because of Singleton
    // Singleton = One class can have only one instance
    public GameManager gameManager;
    public GameObject axe;
    public GameObject sight;
    public GameObject rope;

    private GameManager _gameManager;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private Axe _axeThrow;
    private Rigidbody2D _axeRigidbody;
    private RopeHingeJoint _ropeHingeJoint;
    private bool _mouseHeldDown;
    
    private float _directionX;
    public float movementSpeed = 1f;
    
    private void Start()
    {
        this._currentState = PlayerState.Grounded;
        
        // Initialize variables
        _gameManager = gameManager.GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _axeThrow = axe.GetComponent<Axe>();
        _axeRigidbody = axe.GetComponent<Rigidbody2D>();
        _ropeHingeJoint = rope.GetComponent<RopeHingeJoint>();
        _mouseHeldDown = _gameManager.axeIsSeperated = false;
        sight.SetActive(false);
    }

    private void Update()
    {
        switch (this._currentState)
        {
            case PlayerState.Grounded:
                OnGrounded();
                break;
            
            case PlayerState.Fall:
                OnFall();
                break;
            
            case PlayerState.GroundedAim:
                OnGroundAim();
                break;
            
            case PlayerState.AxeThrow:
                OnAxeThrow();
                break;
            
            case PlayerState.AxeStuck:
                OnAxeStuck();
                break;
            
            case PlayerState.RopeClimb:
                OnRopeClimb();
                break;
            
            case PlayerState.WallSlide:
                OnWallSlide();
                break;
            
            case PlayerState.WallAim:
                OnWallAim();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        /* Axe throw */
        // If the axe is thrown, we dont want to repeat the throw mechanic below
        if (_gameManager.axeIsSeperated)
        {
            // Start following the rope
            var lastRopeSegment = _ropeHingeJoint.GetLastRopeSegment();
            this._boxCollider.enabled = false;
            this._rigidbody.gravityScale = 0f;
            this._rigidbody.MovePosition(lastRopeSegment.transform.position);
            
            return;
        }

        this._boxCollider.enabled = true;
        this._rigidbody.gravityScale = 1f;
    }

    private void FixedUpdate()
    {
        switch (this._currentState)
        {
           case PlayerState.Grounded:
                _directionX = Input.GetAxisRaw("Horizontal");
                _rigidbody.velocity = new Vector2(_directionX * movementSpeed, _rigidbody.velocity.y);
                break;
            
            case PlayerState.Fall:
                break;
            
            case PlayerState.GroundedAim:
                break;
            
            case PlayerState.AxeThrow:
                break;
            
            case PlayerState.AxeStuck:
                break;
            
            case PlayerState.RopeClimb:
                break;
            
            case PlayerState.WallSlide:
                break;
            
            case PlayerState.WallAim:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void TeleportToAxe()
    {
        var oldVal = transform.position;

        var axeDiff = axe.transform.position - oldVal;
        if (axeDiff.x > 0f)
        {
            transform.position = axe.transform.position - new Vector3(_boxCollider.size.x/2, 0);
        }
        else if (axeDiff.x < 0f)
        {
            transform.position = axe.transform.position + new Vector3(_boxCollider.size.x/2, 0);
        }

        if (axeDiff.y + _boxCollider.size.y/2 > .1f)
        {
            transform.position = axe.transform.position + new Vector3(0, _boxCollider.size.y/2);
        }

        Debug.DrawLine(oldVal, transform.position, Color.red, 3f);

        _rigidbody.velocity = Vector2.zero;
        _gameManager.axeIsSeperated = false;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Surface"))
        {
            _rigidbody.AddForce(Vector2.up * _gameManager.playerWallFriction, ForceMode2D.Force);
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
        var aimVec = endPos - startPos;
        Debug.DrawLine(transform.position, endPos, Color.green, 3f);

        // We are aiming in the opposite direction of the throw, so we flip the result vector
        return -aimVec;
    }
    
    private bool IsGrounded()
    {
        LayerMask desiredMask = LayerMask.GetMask("Surface");
        return Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, .1f, desiredMask);
    }

    private bool IsAiming()
    {
        if (!Input.GetMouseButton(0)) return false;
        
        // Detect if mouse is hitting 
        var hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);

        if (!hit.collider)
            return false;

        // Method will only return true if left mouse is clicking on the player, else everything is false
        if (hit.collider.CompareTag("Player"))
            return true;
        
        return false;
    }

    private void ShowSight(Vector2 inputVec)
    {
        // If the aim vector is too short for a throw, dont show the dot
        var inputVecMag = inputVec.magnitude;

        if (inputVecMag < _gameManager.minAxeThrowMag)
        {
            sight.SetActive(false);
            return;
        }
        
        // Since the throw has a max strength, the sight should have a max length, and we do this by limiting
        // the vector magnitude based on the defined max magnitude from GM
        var newMag = Math.Min(_gameManager.maxAxeThrowMag, inputVecMag);
        inputVec = inputVec.normalized * newMag;
        
        Vector2 playerPos = transform.position;
        
        // Calculate the position with player position and throw vector and activate the sight
        // I am unsure why we multiply bu 0.1f^2, but it works
        sight.transform.position = playerPos + inputVec + Physics2D.gravity * ((float)Math.Pow(0.1f, 2));
        sight.SetActive(true);
    }

    /*if (!_gameManager.axeIsSeperated && IsGrounded() && !_mouseHeldDown)
        {
            //this._currentState = PlayerState.Walking;
        }*/

    private void OnGrounded()
    {
        // Movement done in FixedUpdate()
        
        // Aim
        if (IsAiming())
        {
            this._currentState = PlayerState.GroundedAim;
        }
        
        // Step off ledge
        if (!IsGrounded())
        {
            this._currentState = PlayerState.Fall;
        }
    }

    private void OnFall()
    {
        // Land
        if (IsGrounded())
        {
            this._currentState = PlayerState.Grounded;
        }
    }

    private void OnGroundAim()
    {
        // AIMING
        if (!_mouseHeldDown)
        {
            _mouseHeldDown = true;

            // Stop the players movement completely while aiming
            _rigidbody.velocity = Vector2.zero;
        }

        // While the player is holding down the mouse button (ie. aiming), we show a simple sight in form
        // of a white dot
        else if (Input.GetMouseButton(0) && _mouseHeldDown)
        {
            // Gets the player position, mouse position and calculates the throw vector with these two points
            // This new vector is sent to the show sight method
            Vector2 playerPosition = transform.position;
            var currentMousePosition = GetMousePosition();
            var currentThrowVector = GetThrowVector(playerPosition, currentMousePosition);
            
            ShowSight(currentThrowVector);
        }
        
        // If the code has been through the above two blocks (ie. left button has been pressed and held down)
        // and been released we start the actual axe throw
        else if(_mouseHeldDown)
        {
            this._currentState = PlayerState.AxeThrow;
            
            _mouseHeldDown = false;
            _gameManager.axeIsSeperated = true;
            sight.SetActive(false);

            Vector2 playerPosition = transform.position;
            var newMousePosition = GetMousePosition();
            var throwVector = GetThrowVector(playerPosition, newMousePosition);
            
            // Reference the axe and apply the speed based on the aim vector
            if (throwVector.magnitude < _gameManager.minAxeThrowMag)
            {
                _gameManager.axeIsSeperated = false;
                this._currentState = PlayerState.Grounded;
            
                return;
            }

            this._currentState = PlayerState.AxeThrow;
            _axeThrow.ApplyAxeSpeed(throwVector);
        }
    }

    private void OnAxeThrow()
    {
        // Generate rope and hook rope to axe and player to rope
        
        // Axe collide
        if(_axeThrow.currentAxePosition != Axe.AxePosition.Null)
            this._currentState = PlayerState.AxeStuck;
    }

    private void OnAxeStuck()
    {
        // Climb rope
        // Temporary: Climb with right click
        if (Input.GetMouseButtonDown(1))
        {
            TeleportToAxe();
            this._currentState = PlayerState.RopeClimb;
        }
        
        // Release rope (to fall)
        //this._currentState = PlayerState.Fall;
        
        // Release rope (to slide)
        //this._currentState = PlayerState.WallSlide;
        
        // Release rope (to ground)
        //this._currentState = PlayerState.Grounded;
    }

    private void OnRopeClimb()
    {
        // Climb up rope
        
        // Stop climb
        //this._currentState = PlayerState.AxeStuck;
        
        // Release rope (to fall)
        //this._currentState = PlayerState.Fall;
        
        // Release rope (to slide)
        //this._currentState = PlayerState.WallSlide;
        
        // Release rope (to ground)
        //this._currentState = PlayerState.Grounded;

        // Climb to axe
        switch (_axeThrow.currentAxePosition)
        {
            // Climb to axe (roof)
            case Axe.AxePosition.Roof:
                this._currentState = PlayerState.Fall;
                break;
            
            // Climb to axe (wall)
            case Axe.AxePosition.Wall:
                this._currentState = PlayerState.WallSlide;
                break;
            
            // Climb to axe (floor)
            case Axe.AxePosition.Floor:
                this._currentState = PlayerState.Grounded;
                break;
        }

        _axeThrow.currentAxePosition = Axe.AxePosition.Null;
    }

    private void OnWallSlide()
    {
        // Slide player down the wall
        
        // Slide off wall
        this._currentState = PlayerState.Fall;
        
        // Aim
        //this._currentState = PlayerState.WallAim;
    }

    private void OnWallAim()
    {
        // Slide player, show sight and track mouse
        
        // Cancel throw
        //this._currentState = PlayerState.WallSlide;
        
        // Throw
        //this._currentState = PlayerState.AxeThrow;
        
        // Slide off wall
        //this._currentState = PlayerState.Fall;
    }
}
