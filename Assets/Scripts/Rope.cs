using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();
    private float _segmentLength = 0.25f;
    private int _segmentsCount = 35;
    private float _ropeWidth = 0.1f;

    // private Vector2 _ropeGravity = new Vector2(0f, -10f);

    // Use this for initialization
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        InitRopeSegments(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        SimulateRope();
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
            var optimizedGravity = Physics2D.gravity * 5f;
            currentSegment.posNow =
                2 * currentSegment.posNow - currentSegment.posOld + Time.fixedDeltaTime * Time.fixedDeltaTime * optimizedGravity;
            currentSegment.posOld = tempVec;
            _ropeSegments[i] = currentSegment;
            
        }

        //CONSTRAINTS
        var constraintDepth = 100;
        var inputVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        for (var i = 0; i < constraintDepth; i++)
        {
            ApplyConstraint(inputVec);
        }
    }

    private void ApplyConstraint(Vector2 hookPosition)
    {
        //Constraint to hookPosition
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
            currentSegment.y -= _segmentLength;
        }
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