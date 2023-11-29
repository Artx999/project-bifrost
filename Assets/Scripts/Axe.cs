using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public enum AxeState
    {
        Player,
        Air,
        Floor,
        LeftWall,
        RightWall,
        Roof
    }

    public AxeState currentState;
    public GameObject gameManager;
    public Player player;
    
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    private GameManager _gameManager;
    private Vector2 _movementVector;
    private float _speedX = 0f;
    
    private void Start()
    {
        // Initialize variables
        this.currentState = AxeState.Player;
        this._rigidbody = GetComponent<Rigidbody2D>();
        this._spriteRenderer = GetComponent<SpriteRenderer>();
        this._boxCollider = GetComponent<BoxCollider2D>();
        this._animator = GetComponent<Animator>();
        this._gameManager = gameManager.GetComponent<GameManager>();

        this._rigidbody.gravityScale = 0f;
    }

    private void Update()
    {
        this._speedX = _rigidbody.velocity.x;
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
                OnFloor();
                break;
            
            case AxeState.LeftWall:
                OnLeftWall();
                break;
            
            case AxeState.RightWall:
                OnRightWall();
                break;
            
            case AxeState.Roof:
                OnRoof();
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
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
        FollowPlayer();
        DisableAxe(true);
    }

    private void OnAir()
    {
        DisableAxe(false);
    }

    private void OnFloor()
    {
        
    }

    private void OnLeftWall()
    {
        
    }

    private void OnRightWall()
    {
        
    }

    private void OnRoof()
    {
        
    }
    
    public void ApplyAxeSpeed(Vector2 inputVector)
    {
        var inputVecMag = inputVector.magnitude;
        _rigidbody.gravityScale = 1f;
        
        // Fix up the throw vector, by making a new vector with a direction and giving a capped speed
        var realSpeed = Math.Min(_gameManager.maxAxeThrowMag, inputVecMag);
        _movementVector = inputVector.normalized * (realSpeed * _gameManager.axeSpeedAmp);
        
        // Lastly, we add a force and let gravity do its thing
        _rigidbody.AddForce(_movementVector, ForceMode2D.Impulse);
    }

    private void FollowPlayer()
    {
        this.transform.position = player.transform.position;
        DisableAxe(true);
    }

    private void DisableAxe(bool isDisabled)
    {
        this._spriteRenderer.enabled = !isDisabled;
        this._boxCollider.enabled = !isDisabled;
    }
}
