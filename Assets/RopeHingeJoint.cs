using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class RopeHingeJoint : MonoBehaviour
{
    public int ropeLength;
    public float segmentLength;
    public float ropeMass;

    private Transform _transform;
    
    public GameObject anchorPrefab;
    private GameObject _anchor;
    private Rigidbody2D _anchorRigidbody;
    public GameObject ropeSegmentPrefab;

    private List<GameObject> _ropeSegments;
    
    public GameManager gameManager;
    public GameObject axe;
    public Player player;
    public bool ropeExists;
    
    // Start is called before the first frame update
    private void Start()
    {
        _transform = this.transform;
        _ropeSegments = new List<GameObject>();
        _anchor = Instantiate(anchorPrefab, _transform);
        _anchorRigidbody = _anchor.GetComponent<Rigidbody2D>();
        _anchorRigidbody.gravityScale = 0f;

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        ropeExists = false;
    }

    // Update is called once per frame
    private void Update()
    {
        ropeExists = _ropeSegments.Any();
        
        switch (player.currentState)
        {
            case Player.PlayerState.Grounded:
                DestroyRope();
                _anchorRigidbody.gravityScale = 0f;
                break;
            
            case Player.PlayerState.Fall:
                DestroyRope();
                _anchorRigidbody.gravityScale = 0f;
                break;
            
            case Player.PlayerState.GroundedAim:
                break;
            
            case Player.PlayerState.AxeThrow:
                CreateRope();
                _anchorRigidbody.gravityScale = 1f;
                break;
            
            case Player.PlayerState.AxeStuck:
                break;
            
            case Player.PlayerState.WallSlide:
                DestroyRope();
                _anchorRigidbody.gravityScale = 0f;
                break;
            
            case Player.PlayerState.WallAim:
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        _anchorRigidbody.MovePosition(axe.transform.position);
    }

    private void AddRopeSegment()
    {
        // Get the last rope segment. If rope is "empty", it will take the anchor instead
        GameObject lastSegment = _ropeSegments.Any() ? _ropeSegments.Last() : _anchor;

        var lastSegmentPosition = lastSegment.transform.position;
        var lastSegmentRotation = lastSegment.transform.rotation;
        
        GameObject currentSegment = Instantiate(ropeSegmentPrefab, lastSegmentPosition, lastSegmentRotation, _transform);
        _ropeSegments.Add(currentSegment);
        currentSegment.transform.localScale = new Vector3(segmentLength, segmentLength, segmentLength);
        currentSegment.GetComponent<Rigidbody2D>().mass = ropeMass;
        
        // Connect the hinge joints of the current segment to the last segment of the rope
        Rigidbody2D lastSegmentRigidbody2D = lastSegment.GetComponent<Rigidbody2D>();
        HingeJoint2D hingeJoint2D = currentSegment.GetComponent<HingeJoint2D>();
        DistanceJoint2D distanceJoint2D = currentSegment.GetComponent<DistanceJoint2D>();
        hingeJoint2D.connectedBody = lastSegmentRigidbody2D;
        distanceJoint2D.connectedBody = lastSegmentRigidbody2D;
        
        // First segment needs to be attached in the center of the anchor
        // All other segments need to be connected further down from the last
        if (currentSegment != _ropeSegments.First())
        {
            hingeJoint2D.connectedAnchor = new Vector2(0, -1);
            distanceJoint2D.connectedAnchor = new Vector2(0, -1);
        }

        Debug.Log("Current rope length: " + _ropeSegments.Count);
    }

    public void RemoveLastRopeSegment()
    {
        if (!ropeExists)
            return;
        
        var lastSegment = _ropeSegments.Last();
        var lastSegmentIndex = _ropeSegments.Count - 1;
        
        Destroy(lastSegment);
        _ropeSegments.RemoveAt(lastSegmentIndex);
    }

    public void CreateRope()
    {
        if (ropeExists)
            return;
        
        // Add rope segments equal to the desired rope length
        for (int i = 0; i < ropeLength; i++)
            AddRopeSegment();
        
        ropeExists = true;
    }
    
    public void DestroyRope()
    {
        if (!ropeExists)
            return;

        var ropeSegmentCount = _ropeSegments.Count;
        
        // Remove all segments
        for (var i = 0; i < ropeSegmentCount; i++)
            Destroy(_ropeSegments[i]);
        _ropeSegments.Clear();
        
        ropeExists = false;
    }

    public GameObject GetLastRopeSegment()
    {
        return !ropeExists ? null : _ropeSegments.Last();
    }

    public void FollowPlayer()
    {
        _transform.position = player.transform.position;
    }
}
