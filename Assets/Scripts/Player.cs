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
        WallAim,
        GroundStun
    }

    public PlayerState currentState;
    public GameObject gameManager;
    public GameObject axe;
    public GameObject sight;
    public GameObject rope;

    private GameManager _gameManager;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _boxCollider;
    private HingeJoint2D _hingeJoint;
    
    private Axe _axeThrow;
    private Camera _camera;
    private Rope _rope;
    private GameObject _lastRopeSegment;
    private Animator _animator;

    private float _aimingVectorX = 0f;
    private float _animatorLeftWallCheck = 0f;
    private float _directionX = 0f;
    private const float GroundStunTimeNormal = 0.34f;
    private const float GroundStunTimeBack = 1.67f;
    private const float VerticalSpeedLimit = 10f; // REMEMBER TO CHANGE IN ANIMATOR TRANSITIONS AS WELL
    private bool _isBufferedGroundStun = false;
    private bool _isStunCoroutineStarted = false;
    public float movementSpeed = 1f;
    public float climbSpeed = 1f;
    
    private void Start()
    {
        // Initialize variables
        this.currentState = PlayerState.Grounded;
        _gameManager = gameManager.GetComponent<GameManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _hingeJoint = GetComponent<HingeJoint2D>();
        _axeThrow = axe.GetComponent<Axe>();
        _camera = Camera.main;
        _rope = rope.GetComponent<Rope>();
        _animator = GetComponent<Animator>();
        sight.SetActive(false);
    }

    private void Update()
    {
        var aimingVector = Vector2.zero;
        var rigidBodyVelocity = this._rigidbody.velocity;
        
        this._animator.SetInteger("currentState", (int)this.currentState);
        this._animator.SetFloat("speedY", rigidBodyVelocity.y);
        this._animator.SetFloat("speedX", rigidBodyVelocity.x);
        if (Mathf.Abs(rigidBodyVelocity.x) > 0.1f)
        {
            this._animator.SetFloat("lateSpeedX", rigidBodyVelocity.x);
        }
        
        switch (this.currentState)
        {
            case PlayerState.Grounded:
                this.OnGrounded();
                this._animator.SetBool("isWalking", Mathf.Abs(this._rigidbody.velocity.x) > 0.1f);
                break;
            
            case PlayerState.Fall:
                this.OnFall();
                break;
            
            case PlayerState.GroundedAim:
                this.OnGroundAim(ref aimingVector);
                if (this.currentState == PlayerState.GroundedAim)
                {
                    this._aimingVectorX = aimingVector.x;
                    this._animator.SetFloat("aimingX", this._aimingVectorX);
                    this._animator.SetBool("aimCancel", 
                        Mathf.Abs(aimingVector.magnitude) < this._gameManager.minAxeThrowMag);
                    this._animator.SetFloat("lateSpeedX", this._aimingVectorX);
                }
                break;
            
            case PlayerState.AxeThrow:
                this.OnAxeThrow();
                break;
            
            case PlayerState.AxeStuck:
                this.OnAxeStuck();
                break;
            
            case PlayerState.WallSlide:
                this.OnWallSlide();
                this._animator.SetFloat("wallSlideLeft", _animatorLeftWallCheck);
                break;
            
            case PlayerState.WallAim:
                this.OnWallAim(ref aimingVector);
                if (this.currentState == PlayerState.WallAim)
                {
                    this._aimingVectorX = aimingVector.x;
                    this._animator.SetFloat("aimingX", this._aimingVectorX);
                    this._animator.SetBool("aimCancel", 
                        Mathf.Abs(aimingVector.magnitude) < this._gameManager.minAxeThrowMag);
                }
                break;
            
            case PlayerState.GroundStun:
                if(!this._isStunCoroutineStarted)
                    StartCoroutine(this.OnGroundStun());
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        if (this.currentState == PlayerState.Grounded)
        {
            this._directionX = Input.GetAxisRaw("Horizontal");
            this._rigidbody.velocity = new Vector2(this._directionX * this.movementSpeed, this._rigidbody.velocity.y);
        }
    }

    /* STATE METHODS */
    private void OnGrounded()
    {
        // Movement done in FixedUpdate()
        
        // Aim
        if (IsAiming())
        {
            this._rigidbody.velocity = Vector2.zero;
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
        if(!this._isBufferedGroundStun)
            this._isBufferedGroundStun = this._rigidbody.velocity.y < -Player.VerticalSpeedLimit;
        
        // Land
        if (IsGrounded())
        {
            this.currentState = PlayerState.GroundStun;
        }
        else if (IsWalled())
        {
            this._isBufferedGroundStun = false;
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
            Vector2 playerPosition = this.transform.position;
            var currentMousePosition = this.GetMousePosition();
            var currentThrowVector = this.GetThrowVector(playerPosition, currentMousePosition);
            animatorAimVector = currentThrowVector;
            
            this.ShowSight(currentThrowVector);
        }
        
        // The moment the player releases the mouse button the axe is thrown (if the throw is strong enough)
        else
        {
            this.sight.SetActive(false);

            Vector2 playerPosition = this.transform.position;
            var newMousePosition = this.GetMousePosition();
            var throwVector = this.GetThrowVector(playerPosition, newMousePosition);
            
            // Reference the axe and apply the speed based on the aim vector
            if (throwVector.magnitude < this._gameManager.minAxeThrowMag)
            {
                animatorAimVector = throwVector;
                this.currentState = PlayerState.Grounded;
                return;
            }

            this.currentState = PlayerState.AxeThrow;
            this._axeThrow.currentState = Axe.AxeState.Air;
            this._axeThrow.ApplyAxeSpeed(throwVector);
        }
    }

    private void OnAxeThrow()
    {
        // Initialize rope and axe
        this._rope.CreateRope();
        this._lastRopeSegment = this._rope.GetLastRopeSegment();
        
        var playerSegment = this._rope.GetLastRopeSegmentIndex();
        var ropeHangDirection = this._rope.GetRopeSegmentDirection(playerSegment, 3);
        var xValue = ropeHangDirection.normalized.x;
        var yValue = ropeHangDirection.normalized.y;
        this._animator.SetFloat("ropeHangX", xValue);
        this._animator.SetFloat("ropeHangY", yValue);
        
        ConnectToRope(_lastRopeSegment);
        
        // Axe collide
        if(this._axeThrow.currentState != Axe.AxeState.Air)
            this.currentState = PlayerState.AxeStuck;
    }

    private void OnAxeStuck()
    {
        if(this._rope.RopeExists)
        {
            this._lastRopeSegment = this._rope.GetLastRopeSegment();
            
            var playerSegment = this._rope.GetLastRopeSegmentIndex();
            var ropeHangDirection = this._rope.GetRopeSegmentDirection(playerSegment, 3);
            var xValue = ropeHangDirection.normalized.x;
            var yValue = ropeHangDirection.normalized.y;
            this._animator.SetFloat("ropeHangX", xValue);
            this._animator.SetFloat("ropeHangY", yValue);
            
            // Climb rope
            if (Input.GetKey(KeyCode.W))
            {
                this.ClimbRope();
                return;
            }
            
            // Release rope
            if (Input.GetMouseButtonDown(1))
            {
                this._rope.DestroyRope();
                this._axeThrow.currentState = Axe.AxeState.Player;
                //this.EnablePlayerPhysics(true);
                this._rigidbody.velocity = this._lastRopeSegment.GetComponent<Rigidbody2D>().velocity;
                
                if (IsGrounded())
                {
                    this.currentState = PlayerState.Grounded;
                }
                else if (IsWalled())
                {
                    this.currentState = PlayerState.WallSlide;
                }
                else
                {
                    this.currentState = PlayerState.Fall;
                }
            }
            return;
        }
        
        // Reached axe
        //this.EnablePlayerPhysics(true);
        switch (this._axeThrow.currentState)
        {
            case Axe.AxeState.Player:
                // Do nothing
                break;
            
            case Axe.AxeState.Air:
                // Do nothing
                break;
            
            // Climb to axe (floor)
            case Axe.AxeState.Floor:
                this._axeThrow.currentState = Axe.AxeState.Player;
                this.currentState = PlayerState.Grounded;
                break;
            
            // Climb to axe (left wall)
            case Axe.AxeState.LeftWall:
                this._axeThrow.currentState = Axe.AxeState.Player;
                this.currentState = PlayerState.WallSlide;
                break;
            
            // Climb to axe (right wall)
            case Axe.AxeState.RightWall:
                this._axeThrow.currentState = Axe.AxeState.Player;
                this.currentState = PlayerState.WallSlide;
                break;
            
            // Climb to axe (roof)
            case Axe.AxeState.Roof:
                this._axeThrow.currentState = Axe.AxeState.Player;
                this.currentState = PlayerState.Fall;
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

    private void OnWallAim(ref Vector2 animatorAimVector)
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
            this.sight.SetActive(false);
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
            Vector2 playerPosition = this.transform.position;
            var currentMousePosition = this.GetMousePosition();
            var currentThrowVector = this.GetThrowVector(playerPosition, currentMousePosition);
            animatorAimVector = currentThrowVector;
            
            this.ShowSight(currentThrowVector);
        }
        
        // The moment the player releases the mouse button the axe is thrown (if the throw is strong enough)
        else
        {
            this.sight.SetActive(false);

            Vector2 playerPosition = this.transform.position;
            var newMousePosition = this.GetMousePosition();
            var throwVector = this.GetThrowVector(playerPosition, newMousePosition);
            
            // Reference the axe and apply the speed based on the aim vector
            if (throwVector.magnitude < this._gameManager.minAxeThrowMag)
            {
                animatorAimVector = throwVector;
                this.currentState = PlayerState.WallSlide;
                return;
            }

            this.currentState = PlayerState.AxeThrow;
            this._axeThrow.currentState = Axe.AxeState.Air;
            this._axeThrow.ApplyAxeSpeed(throwVector);
        }
        
        // Slide off wall
        //this.currentState = PlayerState.Fall;
    }
    private IEnumerator OnGroundStun()
    {
        this._rigidbody.velocity = Vector2.zero;
        this._isStunCoroutineStarted = true;
        
        if (this._isBufferedGroundStun)
            yield return new WaitForSeconds(GroundStunTimeBack);
        else
            yield return new WaitForSeconds(GroundStunTimeNormal);

        this._isStunCoroutineStarted = false;
        this._isBufferedGroundStun = false;
        this.currentState = PlayerState.Grounded;
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
    
    private Vector2 GetMousePosition()
    {
        if(this._camera != null)
            return this._camera.ScreenToWorldPoint(Input.mousePosition);
        
        return Vector2.zero;
    }

    private Vector2 GetThrowVector(Vector2 startPosition, Vector2 endPosition)
    {
        // Since vectors are lines from origin to a specified point, we need to move
        // the "wrong " vector (from player to mouse position) to have origin in "origin"
        var aimVector = endPosition - startPosition;
        Debug.DrawLine(this.transform.position, endPosition, Color.green, 3f);

        // We are aiming in the opposite direction of the throw, so we flip the result vector
        return -aimVector;
    }
    
    private bool IsGrounded()
    {
        LayerMask desiredMask = LayerMask.GetMask("Surface");
        var boxColliderBounds = this._boxCollider.bounds;
        
        return Physics2D.BoxCast(
            boxColliderBounds.center, boxColliderBounds.size, 
            0f, Vector2.down, .1f, desiredMask);
    }

    private bool IsWalled()
    {
        if (IsGrounded())
            return false;
        
        var desiredMask = LayerMask.GetMask("Surface");
        var boxColliderBounds = this._boxCollider.bounds;
        
        var leftBoxCast = Physics2D.BoxCast(
            boxColliderBounds.center, boxColliderBounds.size, 
            0f, Vector2.left, .1f, desiredMask);
        
        var rightBoxCast = Physics2D.BoxCast(
            boxColliderBounds.center, boxColliderBounds.size, 
            0f, Vector2.right, .1f, desiredMask);

        this._animatorLeftWallCheck = leftBoxCast ? 1f : 0f;
        
        return leftBoxCast || rightBoxCast;
    }

    private bool IsAiming()
    {
        if (!Input.GetMouseButtonDown(0)) return false;
        
        // Detect if mouse is hitting
        var desiredMask = LayerMask.GetMask("Player");
        var hit = Physics2D.Raycast(this.GetMousePosition(), Vector2.zero , Mathf.Infinity, desiredMask);
    
        // Method will only return true if left mouse is clicking on the player, else everything is false
        return hit.collider && hit.collider.CompareTag("Player");
    }

    private void ShowSight(Vector2 inputVector)
    {
        // If the aim vector is too short for a throw, dont show the dot
        var inputVectorMagnitude = inputVector.magnitude;

        if (inputVectorMagnitude < this._gameManager.minAxeThrowMag)
        {
            this.sight.SetActive(false);
            return;
        }
        
        // Since the throw has a max strength, the sight should have a max length, and we do this by limiting
        // the vector magnitude based on the defined max magnitude from GM
        var newMagnitude = Math.Min(this._gameManager.maxAxeThrowMag, inputVectorMagnitude);
        inputVector = inputVector.normalized * newMagnitude;
        
        Vector2 playerPosition = this.transform.position;
        
        // Calculate the position with player position and throw vector and activate the sight
        // I am unsure why we multiply bu 0.1f^2, but it works
        this.sight.transform.position = playerPosition + inputVector + Physics2D.gravity * ((float)Math.Pow(0.1f, 2));
        this.sight.SetActive(true);
    }

    private void ConnectToRope(GameObject ropeSegment)
    {
        _hingeJoint.enabled = true;
        _hingeJoint.connectedBody = ropeSegment.GetComponent<Rigidbody2D>();
        _hingeJoint.connectedAnchor = new Vector2(0, -.5f);
    }

    private void ClimbRope()
    {
        float playerPositionOnRopeSegment = this._hingeJoint.connectedAnchor.y;
        if (playerPositionOnRopeSegment <= .5f)
            this._hingeJoint.connectedAnchor = new Vector2(0, playerPositionOnRopeSegment + this.climbSpeed * .01f);
        else
        {
            _rope.RemoveLastRopeSegment();
            this.ConnectToRope(this._rope.LastRopeSegment);
        }
    }
}
