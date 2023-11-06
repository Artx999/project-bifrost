using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject player;
    public GameObject axe;

    private GameManager _gameManager;
    private LineRenderer _lineRenderer;
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();
    private float _segmentLength = 0.25f;
    private int _segmentsCount = 35;
    private float _ropeWidth = 0.1f;

    // Use this for initialization
    private void Start()
    {
        _gameManager = gameManager.GetComponent<GameManager>();
        _lineRenderer = GetComponent<LineRenderer>();
        
        InitRopeSegments(player.transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        if (_gameManager.axeIsSeperated)
        {
            SimulateRope();
        }
        else
        {
            SimulateHiddenRope(player.transform.position);
        }
    }

    private void SimulateRope()
    {
        // SIMULATION
        for (var i = 0; i < _segmentsCount; i++)
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
        for (var i = 0; i < _segmentsCount - 1; i++)
        {
            var currentSegment = _ropeSegments[i];
            var nextSegment = _ropeSegments[i + 1];

            var distance = (currentSegment.posNow - nextSegment.posNow).magnitude;
            var error = distance - _segmentLength;
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

    private void DrawRope()
    {
        _lineRenderer.startWidth = _ropeWidth;
        _lineRenderer.endWidth = _ropeWidth;

        var ropePositions = new Vector3[_segmentsCount];
        for (var i = 0; i < _segmentsCount; i++)
        {
            ropePositions[i] = _ropeSegments[i].posNow;
        }

        _lineRenderer.positionCount = ropePositions.Length;
        _lineRenderer.SetPositions(ropePositions);
    }

    private void InitRopeSegments(Vector2 hookPosition)
    {
        var currentSegment = hookPosition;

        for (var i = 0; i < _segmentsCount; i++)
        {
            _ropeSegments.Add(new RopeSegment(currentSegment));
        }
    }

    private void SimulateHiddenRope(Vector2 hookPosition)
    {
        var ropeSegment = new RopeSegment(hookPosition);
        
        for(var i = 0; i < _segmentsCount; i++)
        {
            _ropeSegments[i] = ropeSegment;
        }
    }

    public Vector2 GetRopeEndPosition()
    {
        return _ropeSegments.Count <= 0 ? Vector2.zero : _ropeSegments[_segmentsCount - 1].posNow;
    }
    
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