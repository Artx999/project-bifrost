using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();
    private float _ropeSegmentLength = .25f;
    private int _segmentLength = 25;
    private float _lineWidth = .1f;
    
    private EdgeCollider2D _edgeCollider;

    
    // Start is called before the first frame update
    void Start()
    {
        this._lineRenderer = this.GetComponent<LineRenderer>();
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        for (int i = 0; i < _segmentLength; i++)
        {
            this._ropeSegments.Add(new RopeSegment(startPoint));
            startPoint.y -= _ropeSegmentLength;
        }
        
        _edgeCollider = this.GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        this.DrawRope();
        this.SetEdgeCollider(_lineRenderer);
    }

    private void FixedUpdate()
    {
        this.Simulate();
    }

    private void Simulate()
    {
        // Simulation
        Vector2 forceGravity = new Vector2(0f, -1.5f);
        
        for (int i = 1; i < this._segmentLength; i++)
        {
            RopeSegment firstSegment = this._ropeSegments[i];
            Vector2 velocity = firstSegment.CurrentPosition - firstSegment.OldPosition;
            firstSegment.OldPosition = firstSegment.CurrentPosition;
            firstSegment.CurrentPosition += velocity;
            firstSegment.CurrentPosition += forceGravity * Time.fixedDeltaTime;
            this._ropeSegments[i] = firstSegment;
        }
        
        // Constraints
        for (int i = 0; i < 50; i++)
        {
            this.ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        RopeSegment firstSegment = this._ropeSegments[0];
        firstSegment.CurrentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this._ropeSegments[0] = firstSegment;

        for (int i = 0; i < this._segmentLength - 1; i++)
        {
            RopeSegment firstRopeSegment = this._ropeSegments[i];
            RopeSegment secondRopeSegment = this._ropeSegments[i + 1];

            float distance = (firstRopeSegment.CurrentPosition - secondRopeSegment.CurrentPosition).magnitude;
            float error = Mathf.Abs(distance - this._ropeSegmentLength);
            Vector2 changeDirection = Vector2.zero;

            if (distance > _ropeSegmentLength)
            {
                changeDirection = (firstRopeSegment.CurrentPosition - secondRopeSegment.CurrentPosition).normalized;
            } else if (distance < _ropeSegmentLength)
            {
                changeDirection = (secondRopeSegment.CurrentPosition - firstRopeSegment.CurrentPosition).normalized;
            }

            Vector2 changeAmount = changeDirection * error;
            if (i != 0)
            {
                firstRopeSegment.CurrentPosition -= changeAmount * .5f;
                this._ropeSegments[i] = firstRopeSegment;
                secondRopeSegment.CurrentPosition += changeAmount * .5f;
                this._ropeSegments[i + 1] = secondRopeSegment;
            }
            else
            {
                secondRopeSegment.CurrentPosition += changeAmount;
                this._ropeSegments[i + 1] = secondRopeSegment;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this._lineWidth;
        _lineRenderer.startWidth = lineWidth;
        _lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[this._segmentLength];
        for (int i = 0; i < this._segmentLength; i++)
        {
            ropePositions[i] = this._ropeSegments[i].CurrentPosition;
        }

        _lineRenderer.positionCount = ropePositions.Length;
        _lineRenderer.SetPositions(ropePositions);
    }
    
    public struct RopeSegment
    {
        public Vector2 CurrentPosition;
        public Vector2 OldPosition;

        public RopeSegment(Vector2 position)
        {
            this.CurrentPosition = position;
            this.OldPosition = position;
        }
    }
    
    void SetEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for(int point = 0; point<lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        _edgeCollider.SetPoints(edges);
    }
}
