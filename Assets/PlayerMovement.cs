using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private float _directionX;
    
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

        _rigidBody.velocity = new Vector2(_directionX * 7f, _rigidBody.velocity.y);
        
        if (Input.GetButtonDown("Jump"))
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 14f);
        }
        
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (_directionX > 0f)
        {
            _animator.SetBool("running", true);
            _spriteRenderer.flipX = false;
        }
        else if (_directionX < 0f)
        {
            _animator.SetBool("running", true);
            _spriteRenderer.flipX = true;
        }
        else
        {
            _animator.SetBool("running", false);
        }
    }
}
