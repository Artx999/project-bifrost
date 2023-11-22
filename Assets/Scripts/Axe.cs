using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public enum AxePosition
    {
        Null,
        Floor,
        Wall,
        Roof
    }
    
    public GameObject gameManager;
    public Player player;
    public AxePosition currentAxePosition;
    
    private Rigidbody2D _rigidbody;
    private GameManager _gameManager;
    private Vector2 _movementVector;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Initialize variables
        _rigidbody = GetComponent<Rigidbody2D>();
        _gameManager = gameManager.GetComponent<GameManager>();
        currentAxePosition = AxePosition.Null;
        
        _rigidbody.gravityScale = 0f;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private void Update()
    {
        // If the axe is on the player, the axe should follow the player around
        // the moment the axe is thrown, the collider is activated
        if (!_gameManager.axeIsSeperated)
            transform.position = player.transform.position;
        else
            GetComponent<BoxCollider2D>().enabled = true;
    }

    public void ApplyAxeSpeed(Vector2 inputVec)
    {
        // If the throw vector is too short, we cancel the throw
        var inputVecMag = inputVec.magnitude;
        
        // If a successful throw, apply gravity
        _rigidbody.gravityScale = 1f;
        
        // Fix up the throw vector, by making a new vector with a direction and giving a capped speed
        float realSpeed = Math.Min(_gameManager.maxAxeThrowMag, inputVecMag);
        _movementVector = inputVec.normalized * (realSpeed * _gameManager.axeSpeedAmp);
        
        // Lastly, we add a force and let gravity do its thing
        _rigidbody.AddForce(_movementVector, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var collisionHitNormal = other.GetContact(0).normal;
        
        if (collisionHitNormal == Vector2.right || collisionHitNormal == Vector2.left)
            this.currentAxePosition = AxePosition.Wall;
        else if (collisionHitNormal == Vector2.up)
            this.currentAxePosition = AxePosition.Floor;
        else if (collisionHitNormal == Vector2.down)
            this.currentAxePosition = AxePosition.Roof;
        
        Debug.Log("Hit surface!");
        this._rigidbody.velocity = Vector2.zero;
        this._rigidbody.gravityScale = 0f;
    }
}
