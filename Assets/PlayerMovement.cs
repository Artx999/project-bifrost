using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float directionX = Input.GetAxisRaw("Horizontal");

        _rigidBody.velocity = new Vector2(directionX * 7f, _rigidBody.velocity.y);
        
        if (Input.GetButtonDown("Jump"))
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 14f);
        }
    }
}
