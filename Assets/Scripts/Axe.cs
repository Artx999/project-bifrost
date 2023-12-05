using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Axe : MonoBehaviour
{
    // Axe-state enum
    public enum AxeState
    {
        Player,
        Air,
        Floor,
        LeftWall,
        RightWall,
        Roof
    }
    
    // PUBLIC FIELDS
    public AxeState currentState;
    
    [Header("Gameobject references")]
    public GameObject gameController;
    public GameObject player;
    
    // PRIVATE FIELDS
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    
    private GameController _gameController;
    
    private Vector2 _movementVector;
    private float _speedX = 0f;

    private void Awake()
    {
        this.gameController = GameObject.FindWithTag("GameController");
        this.player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        // Initialize variables
        this.currentState = AxeState.Player;
        this._rigidbody = GetComponent<Rigidbody2D>();
        this._spriteRenderer = GetComponent<SpriteRenderer>();
        this._boxCollider = GetComponent<BoxCollider2D>();
        this._animator = GetComponent<Animator>();
        this._gameController = this.gameController.GetComponent<GameController>();

        this.DisableAxe(true);
    }

    private void Update()
    {
        if (this._gameController.isGamePaused)
            return;
        
        this._speedX = this._rigidbody.velocity.x;
        this._animator.SetInteger("currentState", (int)this.currentState);
        
        if (Mathf.Abs(this._speedX) > 0.1f)
        {
            this._animator.SetFloat("speedX", this._speedX);
        }
        
        switch(this.currentState)
        {
            case AxeState.Player:
                OnPlayer();
                break;
            
            case AxeState.Air:
                OnAir();
                break;
            
            case AxeState.Floor:
                break;
            
            case AxeState.LeftWall:
                break;
            
            case AxeState.RightWall:
                break;
            
            case AxeState.Roof:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void FixedUpdate()
    {
        if (this.currentState == AxeState.Player)
        {
            FollowPlayer();
        }
        
        // Check for terminal velocity
        if (this._rigidbody.velocity.y <=
            -this._gameController.terminalVelocity)
        {
            this._rigidbody.velocity =
                new Vector2(this._rigidbody.velocity.x, -this._gameController.terminalVelocity);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Surface"))
            return;
        
        // Detect collision and see what type of surface was hit, based on the enum
        var collisionHitNormal = other.GetContact(0).normal;
        
        if ((collisionHitNormal - Vector2.right).magnitude < 0.1f)
            this.currentState = AxeState.LeftWall;
        else if ((collisionHitNormal - Vector2.left).magnitude < 0.1f)
            this.currentState = AxeState.RightWall;
        else if ((collisionHitNormal - Vector2.up).magnitude < 0.1f)
            this.currentState = AxeState.Floor;
        else if ((collisionHitNormal - Vector2.down).magnitude < 0.1f)
            this.currentState = AxeState.Roof;
        
        // We hit a surface successfully and can stop the axe movement
        this._rigidbody.velocity = Vector2.zero;
        this._rigidbody.gravityScale = 0f;
    }

    private void OnPlayer()
    {
        this.DisableAxe(true);
    }

    private void OnAir()
    {
        this.DisableAxe(false);
    }
    
    public void ApplyAxeSpeed(Vector2 inputVector)
    {
        var inputVectorMagnitude = inputVector.magnitude;
        this._rigidbody.gravityScale = 1f;
        
        // Fix up the throw vector, by making a new vector with a direction and giving a capped speed
        var realSpeed = Math.Min(this._gameController.maxAxeThrowMagnitude, inputVectorMagnitude);
        this._movementVector = inputVector.normalized * (realSpeed * this._gameController.axeSpeedAmplitude);
        
        // Lastly, we add a force and let gravity do its thing
        this._rigidbody.AddForce(this._movementVector, ForceMode2D.Impulse);
    }

    private void FollowPlayer()
    {
        this._rigidbody.position = this.player.transform.position;
        this.DisableAxe(true);
    }

    private void DisableAxe(bool isDisabled)
    {
        this._spriteRenderer.enabled = !isDisabled;
        this._boxCollider.enabled = !isDisabled;
        this._rigidbody.gravityScale = isDisabled ? 0f : 1f;
    }
}
