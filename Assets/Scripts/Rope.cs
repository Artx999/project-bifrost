using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject player;
    public GameObject axe;

    public float segmentLength = 0.25f;
    public int segmentsCount = 35;
    public float ropeWidth = 0.1f;
    
    private GameManager _gameManager;
    private LineRenderer _lineRenderer;
    private List<RopeSegment> _ropeSegments;

    // Use this for initialization
    private void Start()
    {
        _gameManager = gameManager.GetComponent<GameManager>();
        _lineRenderer = GetComponent<LineRenderer>();
        _ropeSegments = new List<RopeSegment>();
        
        InitRopeSegments(player.transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        // TODO: Find alternate way to draw the rope when axe is not seperated
        DrawRope();
    }

    private void FixedUpdate()
    {
        // Whenever the axe is seperated, the rope should show to connect the axe and the player
        if (_gameManager.axeIsSeperated)
        {
            SimulateRope();
        }
        else // Whenever the axe is NOT seperated, the rope should not show
        {
            SimulateHiddenRope(player.transform.position);
        }
    }

    private void SimulateRope()
    {
        // SIMULATION
        for (var i = 0; i < segmentsCount; i++)
        {
            /*
            RopeSegment currentSegment = _ropeSegments[i];
             
            Vector2 velocity = currentSegment.posNow - currentSegment.posOld;
            currentSegment.posOld = currentSegment.posNow;
            currentSegment.posNow += velocity;
            currentSegment.posNow += _ropeGravity * Time.fixedDeltaTime;
            
            _ropeSegments[i] = currentSegment;
            */
            
            // Actual Verlet Integration, but the acceleration for both of these methods is very different
            var currentSegment = _ropeSegments[i];
            var tempVec = currentSegment.posNow;
            var totalAcceleration = Physics2D.gravity;
            
            currentSegment.posNow =
                2 * currentSegment.posNow - currentSegment.posOld + Time.fixedDeltaTime * Time.fixedDeltaTime * totalAcceleration;
            currentSegment.posOld = tempVec;
            _ropeSegments[i] = currentSegment;
        }

        //CONSTRAINTS
        // Times the constraint method should execute. The larger, the better rope, but more expensive
        var constraintDepth = 100;
        var inputVec1 = axe.transform.position;
        
        for (var i = 0; i < constraintDepth; i++)
        {
            ApplyConstraint(inputVec1);
        }
    }

    private void ApplyConstraint(Vector2 hookPosition)
    {
        // Create hook at start of rope
        var firstSegment = _ropeSegments[0];
        firstSegment.posNow = hookPosition;
        _ropeSegments[0] = firstSegment;

        // Keep length between points in rope constant to avoid stretching
        for (var i = 0; i < segmentsCount - 1; i++)
        {
            var currentSegment = _ropeSegments[i];
            var nextSegment = _ropeSegments[i + 1];

            var distance = (currentSegment.posNow - nextSegment.posNow).magnitude;
            var error = distance - segmentLength;
            var changeDirection = (currentSegment.posNow - nextSegment.posNow).normalized;
            var changeAmount = changeDirection * (error * 0.5f);

            if (i == 0)
            {
                nextSegment.posNow += 2 * changeAmount;
                _ropeSegments[i + 1] = nextSegment;
                
                continue;
            }
            
            currentSegment.posNow -= changeAmount;
            _ropeSegments[i] = currentSegment;
            nextSegment.posNow += changeAmount;
            _ropeSegments[i + 1] = nextSegment;
        }
    }

    // Draws rope based on the current positions of the segments, from the list
    private void DrawRope()
    {
        _lineRenderer.startWidth = ropeWidth;
        _lineRenderer.endWidth = ropeWidth;

        var ropePositions = new Vector3[segmentsCount];
        for (var i = 0; i < segmentsCount; i++)
        {
            ropePositions[i] = _ropeSegments[i].posNow;
        }

        _lineRenderer.positionCount = ropePositions.Length;
        _lineRenderer.SetPositions(ropePositions);
    }

    // Initializes the rope segment list
    private void InitRopeSegments(Vector2 hookPosition)
    {
        var currentSegment = hookPosition;

        for (var i = 0; i < segmentsCount; i++)
        {
            _ropeSegments.Add(new RopeSegment(currentSegment));
        }
    }

    // Less expensive simulation for when the rope is not visible
    // It makes sure the rope is tucked inside player, hiding it
    private void SimulateHiddenRope(Vector2 hookPosition)
    {
        var ropeSegment = new RopeSegment(hookPosition);
        
        for(var i = 0; i < segmentsCount; i++)
        {
            _ropeSegments[i] = ropeSegment;
        }
    }

    // Public method that returns the last segment
    public Vector2 GetRopeEndPosition()
    {
        return _ropeSegments.Count <= 0 ? Vector2.zero : _ropeSegments[segmentsCount - 1].posNow;
    }
    
    // Struct that stores the old and current position for Verlet Integration
    private struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
}