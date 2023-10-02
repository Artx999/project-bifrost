using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 1f;
    private float _directionX;
    private Rigidbody2D _rigidBody;
    
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _directionX = Input.GetAxisRaw("Horizontal");
        _rigidBody.velocity = new Vector2(_directionX * movementSpeed, _rigidBody.velocity.y);
    }
}
