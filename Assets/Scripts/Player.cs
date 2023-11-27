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
        WallSlide,
        WallAim
    }

    public PlayerState currentState;
    public GameObject gameManager;
    public GameObject axe;
    public GameObject sight;
    public GameObject rope;

    private GameManager _gameManager;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private Axe _axeThrow;
    private Camera _camera;
    private Rope _rope;
    private GameObject _lastRopeSegment;
    private Animator _animator;

    private float _aimingVectorX = 0f;
    private float _directionX = 0f;
    public float movementSpeed = 1f;

    private void Start()
    {
        // Initialize variables
        this.currentState = PlayerState.Grounded;
        _gameManager = gameManager.GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _axeThrow = axe.GetComponent<Axe>();
        _camera = Camera.main;
        _rope = rope.GetComponent<Rope>();
        _animator = GetComponent<Animator>();
        sight.SetActive(false);
    }

    private void Update()
    {
        _animator.SetInteger("currentState", (int)currentState);
        
        var rigidBodyVelocityX = _rigidbody.velocity.x;
        if (Mathf.Abs(rigidBodyVelocityX) > _gameManager.minAxeThrowMag)
        {
            _animator.SetFloat("movementX", rigidBodyVelocityX);
        }
        
        switch (this.currentState)
        {
            case PlayerState.Grounded:
                _animator.SetBool("isWalking", _rigidbody.velocity.magnitude > 0.1f);
                OnGrounded();
                break;
            
            case PlayerState.Fall:
                OnFall();
                break;
            
            case PlayerState.GroundedAim:
                var aimingVector = Vector2.zero;
                OnGroundAim(ref aimingVector);
                if (currentState == PlayerState.GroundedAim)
                {
                    _aimingVectorX = aimingVector.x;
                    _animator.SetFloat("aimingX", _aimingVectorX);
                    _animator.SetBool("aimCancel", 
                        Mathf.Abs(aimingVector.magnitude) < _gameManager.minAxeThrowMag);
                    _animator.SetFloat("movementX", _aimingVectorX);
                }
                break;
            
            case PlayerState.AxeThrow:
                OnAxeThrow();
                break;
            
            case PlayerState.AxeStuck:
                OnAxeStuck();
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
    }

    private void FixedUpdate()
    {
        if (this.currentState == PlayerState.Grounded)
        {
            _directionX = Input.GetAxisRaw("Horizontal");
            _rigidbody.velocity = new Vector2(_directionX * movementSpeed, _rigidbody.velocity.y);
        }
    }

    /* STATE METHODS */
    private void OnGrounded()
    {
        // Movement done in FixedUpdate()
        
        // Aim
        if (IsAiming())
        {
            _rigidbody.velocity = Vector2.zero;
            this.currentState = PlayerState.GroundedAim;
        }
        
        // Step off ledge
        if (!IsGrounded())
        {
            this._rigidbody.velocity = Vector2.zero;
            this.currentState = PlayerState.Fall;
        }
    }

    private void OnFall()
    {
        // Land
        if (IsGrounded())
        {
            this.currentState = PlayerState.Grounded;
        }
        else if (IsWalled())
        {
            this.currentState = PlayerState.WallSlide;
        }
    }

    private void OnGroundAim(ref Vector2 animatorAimVector)
    {
        // AIMING
        // While the player is holding down the mouse button (ie. aiming), we show a simple sight
        if (Input.GetMouseButton(0))
        {
            // Gets the player position, mouse position and calculates the throw vector with these two points
            // This new vector is sent to the show sight method
            Vector2 playerPosition = transform.position;
            var currentMousePosition = GetMousePosition();
            var currentThrowVector = GetThrowVector(playerPosition, currentMousePosition);
            animatorAimVector = currentThrowVector;
            
            ShowSight(currentThrowVector);
        }
        
        // The moment the player releases the mouse button the axe is thrown (if the throw is strong enough)
        else
        {
            sight.SetActive(false);

            Vector2 playerPosition = transform.position;
            var newMousePosition = GetMousePosition();
            var throwVector = GetThrowVector(playerPosition, newMousePosition);
            
            // Reference the axe and apply the speed based on the aim vector
            if (throwVector.magnitude < _gameManager.minAxeThrowMag)
            {
                animatorAimVector = throwVector;
                this.currentState = PlayerState.Grounded;
                return;
            }

            this.currentState = PlayerState.AxeThrow;
            _axeThrow.ApplyAxeSpeed(throwVector);
        }
    }

    private void OnAxeThrow()
    {
        // Initialize rope
        _rope.CreateRope();
        _lastRopeSegment = _rope.GetLastRopeSegment();
        
        EnablePlayerPhysics(false);
        this._rigidbody.MovePosition(_lastRopeSegment.transform.position);
        
        // Axe collide
        if(_axeThrow.currentAxePosition != Axe.AxePosition.Null)
            this.currentState = PlayerState.AxeStuck;
    }

    private void OnAxeStuck()
    {
        if(_rope.RopeExists)
        {
            // While the rope still exists we can climb the rope
            _lastRopeSegment = _rope.GetLastRopeSegment();
            this._rigidbody.MovePosition(_lastRopeSegment.transform.position);
            
            // Climb rope
            // TODO: Better climbing mechanic
            if (Input.GetKeyDown(KeyCode.W))
            {
                _rope.RemoveLastRopeSegment();
                return;
            }
            
            // Release rope
            if (Input.GetMouseButtonDown(1))
            {
                _rope.DestroyRope();
                this._axeThrow.currentAxePosition = Axe.AxePosition.Null;
                this.EnablePlayerPhysics(true);
                this._rigidbody.velocity = _lastRopeSegment.GetComponent<Rigidbody2D>().velocity;
                
                this.currentState = PlayerState.Fall;
            }
            
            return;
        }
        
        // Reached axe
        this.EnablePlayerPhysics(true);
        switch (_axeThrow.currentAxePosition)
        {
            case Axe.AxePosition.Null:
                break;
            
            // Climb to axe (roof)
            case Axe.AxePosition.Roof:
                _axeThrow.currentAxePosition = Axe.AxePosition.Null;
                this.currentState = PlayerState.Fall;
                break;
            
            // Climb to axe (wall)
            case Axe.AxePosition.Wall:
                _axeThrow.currentAxePosition = Axe.AxePosition.Null;
                this.currentState = PlayerState.WallSlide;
                break;
            
            // Climb to axe (floor)
            case Axe.AxePosition.Floor:
                _axeThrow.currentAxePosition = Axe.AxePosition.Null;
                this.currentState = PlayerState.Grounded;
                break;
                
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnWallSlide()
    {
        // Wall sliding check is done in OnCollisionStay2D()

        // Slide to ground
        if (IsGrounded())
        {
            this.currentState = PlayerState.Grounded;
        }

        // Slide off wall
        else if (!IsWalled())
        {
            this.currentState = PlayerState.Fall;
        }

        // Slide and aim
        else if (IsAiming())
        {
            this.currentState = PlayerState.WallAim;
        }
    }

    private void OnWallAim()
    {
        // If we slide to the ground/off the wall while aiming, we change state
        if (IsGrounded())
        {
            if (Input.GetMouseButton(0))
            {
                this.currentState = PlayerState.GroundedAim;
            }
            else
            {
                sight.SetActive(false);
                this.currentState = PlayerState.Grounded;
            }
            
            return;
        }

        // Slide off wall
        if (!IsWalled())
        {
            sight.SetActive(false);
            this.currentState = PlayerState.Fall;
            return;
        }
        
        // TODO: Make aiming possible while sliding down wall
        // AIMING
        // While the player is holding down the mouse button (ie. aiming), we show a simple sight
        if (Input.GetMouseButton(0))
        {
            // Gets the player position, mouse position and calculates the throw vector with these two points
            // This new vector is sent to the show sight method
            Vector2 playerPosition = transform.position;
            var currentMousePosition = GetMousePosition();
            var currentThrowVector = GetThrowVector(playerPosition, currentMousePosition);
            
            ShowSight(currentThrowVector);
        }
        
        // The moment the player releases the mouse button the axe is thrown (if the throw is strong enough)
        else
        {
            sight.SetActive(false);

            Vector2 playerPosition = transform.position;
            var newMousePosition = GetMousePosition();
            var throwVector = GetThrowVector(playerPosition, newMousePosition);
            
            // Reference the axe and apply the speed based on the aim vector
            if (throwVector.magnitude < _gameManager.minAxeThrowMag)
            {
                this.currentState = PlayerState.WallSlide;
                return;
            }

            this.currentState = PlayerState.AxeThrow;
            _axeThrow.ApplyAxeSpeed(throwVector);
        }
        
        // Slide off wall
        //this.currentState = PlayerState.Fall;
    }
    
    /* PRIVATE METHODS */
    private void EnablePlayerPhysics(bool activate)
    {
        if (activate)
        {
            this._boxCollider.enabled = true;
            this._rigidbody.gravityScale = 1f;

            return;
        }

        this._boxCollider.enabled = false;
        this._rigidbody.gravityScale = 0f;
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
    }
    
    private Vector2 GetMousePosition()
    {
        if(_camera != null)
            return _camera.ScreenToWorldPoint(Input.mousePosition);
        
        return Vector2.zero;
    }

    private Vector2 GetThrowVector(Vector2 startPosition, Vector2 endPosition)
    {
        // Since vectors are lines from origin to a specified point, we need to move
        // the "wrong " vector (from player to mouse position) to have origin in "origin"
        var aimVector = endPosition - startPosition;
        Debug.DrawLine(transform.position, endPosition, Color.green, 3f);

        // We are aiming in the opposite direction of the throw, so we flip the result vector
        return -aimVector;
    }
    
    private bool IsGrounded()
    {
        LayerMask desiredMask = LayerMask.GetMask("Surface");
        var boxColliderBounds = _boxCollider.bounds;
        
        return Physics2D.BoxCast(
            boxColliderBounds.center, boxColliderBounds.size, 
            0f, Vector2.down, .1f, desiredMask);
    }

    private bool IsWalled()
    {
        if (IsGrounded())
            return false;
        
        LayerMask desiredMask = LayerMask.GetMask("Surface");
        var boxColliderBounds = _boxCollider.bounds;
        
        var leftBoxCast = Physics2D.BoxCast(
            boxColliderBounds.center, boxColliderBounds.size, 
            0f, Vector2.left, .1f, desiredMask);
        
        var rightBoxCast = Physics2D.BoxCast(
            boxColliderBounds.center, boxColliderBounds.size, 
            0f, Vector2.right, .1f, desiredMask);
        
        return leftBoxCast || rightBoxCast;
    }

    private bool IsAiming()
    {
        if (!Input.GetMouseButton(0)) return false;
        
        // Detect if mouse is hitting 
        var hit = Physics2D.Raycast(GetMousePosition(), Vector2.zero);
    
        // Method will only return true if left mouse is clicking on the player, else everything is false
        return hit.collider && hit.collider.CompareTag("Player");
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
}
