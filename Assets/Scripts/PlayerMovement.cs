using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private float _directionX;

    [SerializeField] private float _movementSpeed = 7f;
    [SerializeField] private float _jumpForce = 14f;

    private enum MovementState
    {
        Idle,
        Running,
        Jumping,
        Falling
    }

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _directionX = Input.GetAxisRaw("Horizontal");

        _rigidBody.velocity = new Vector2(_directionX * _movementSpeed, _rigidBody.velocity.y);

        if (Input.GetButtonDown("Jump"))
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, _jumpForce);
        }

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState movementState;
        
        if (_directionX > 0f)
        {
            movementState = MovementState.Running;
            _spriteRenderer.flipX = false;
        }
        else if (_directionX < 0f)
        {
            movementState = MovementState.Running;
            _spriteRenderer.flipX = true;
        }
        else
        {
            movementState = MovementState.Idle;
        }

        if (_rigidBody.velocity.y > .1f)
        {
            movementState = MovementState.Jumping;
        }
        else if (_rigidBody.velocity.y < -.1f)
        {
            movementState = MovementState.Falling;
        }
        {
            
        }
        
        _animator.SetInteger("MovementState", (int) movementState);
    }
}