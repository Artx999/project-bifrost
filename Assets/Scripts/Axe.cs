using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
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
    }

    private void Update()
    {
        switch (player.currentState)
        {
            case Player.PlayerState.Grounded:
                FollowPlayer();
                break;
            
            case Player.PlayerState.Fall:
                FollowPlayer();
                break;
            
            case Player.PlayerState.GroundedAim:
                FollowPlayer();
                break;
            
            case Player.PlayerState.AxeThrow:
                break;
            
            case Player.PlayerState.AxeStuck:
                break;
            
            case Player.PlayerState.WallSlide:
                FollowPlayer();
                break;
            
            case Player.PlayerState.WallAim:
                FollowPlayer();
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
            this.currentAxePosition = AxePosition.Wall;
        else if ((collisionHitNormal - Vector2.left).magnitude < 0.1f)
            this.currentAxePosition = AxePosition.Wall;
        else if ((collisionHitNormal - Vector2.up).magnitude < 0.1f)
            this.currentAxePosition = AxePosition.Floor;
        else if ((collisionHitNormal - Vector2.down).magnitude < 0.1f)
            this.currentAxePosition = AxePosition.Roof;
        
        // We hit a surface successfully and can stop the axe movement
        this._rigidbody.velocity = Vector2.zero;
        this._rigidbody.gravityScale = 0f;
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
    }
}
