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
    private bool _ropeIsCreated;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = this.transform;
        _ropeSegments = new List<GameObject>();
        _anchor = Instantiate(anchorPrefab, _transform);
        _anchorRigidbody = _anchor.GetComponent<Rigidbody2D>();
        _anchorRigidbody.gravityScale = 0f;

        _ropeIsCreated = false;
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetMouseButtonDown(1))
            AddRopeSegment();

        if (Input.GetKeyDown(KeyCode.Space))
            DeleteAllRopeSegments(); */

        if(!_ropeIsCreated)
            this.transform.position = axe.transform.position;
        
        if (gameManager.axeIsSeperated)
        {
            if (!_ropeIsCreated)
            {
                CreateRope();
                _anchorRigidbody.gravityScale = 1f;
            }
            _anchorRigidbody.MovePosition(axe.transform.position);
        }
        else
        {
            if (_ropeIsCreated)
            {
                DestroyRope();
                _anchorRigidbody.gravityScale = 0f;
            }
            _anchorRigidbody.MovePosition(axe.transform.position);
        }
    }

    void AddRopeSegment()
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

        Debug.Log("Current rope length" + _ropeSegments.Count);
    }

    public void CreateRope()
    {
        // Add rope segments equal to the desired rope length
        for (int i = 0; i < ropeLength; i++)
            AddRopeSegment();
        _ropeIsCreated = true;
    }
    
    public void DestroyRope()
    {
        // Remove all segments
        for (int i = 0; i < ropeLength; i++)
            Destroy(_ropeSegments[i]);
        _ropeSegments.Clear();
        _ropeIsCreated = false;
    }

    public GameObject GetLastRopeSegment()
    {
        return _ropeSegments.Last();
    }
}
