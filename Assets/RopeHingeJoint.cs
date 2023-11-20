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
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = this.transform;
        _ropeSegments = new List<GameObject>();
        _anchor = Instantiate(anchorPrefab, _transform);
        
        for (int i = 0; i < ropeLength; i++)
        { 
            AddRopeSegment();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            AddRopeSegment();
    }

    void AddRopeSegment()
    {
        GameObject lastSegment = _ropeSegments.Any() ? _ropeSegments.Last() : _anchor;
        
        GameObject currentSegment = Instantiate(ropeSegmentPrefab, _transform);
        _ropeSegments.Add(currentSegment);
        currentSegment.transform.localScale = new Vector3(segmentLength, segmentLength, segmentLength);
        currentSegment.GetComponent<Rigidbody2D>().mass = ropeMass;

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
}
