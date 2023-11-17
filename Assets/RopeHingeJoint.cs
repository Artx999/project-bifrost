using System;
using System.Collections;
using System.Collections.Generic;
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
    private Rigidbody2D _anchorRigidbody;
    public GameObject ropeSegmentPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = this.transform;
        
        // Generate rope
        GameObject lastSegment = Instantiate(anchorPrefab, _transform);;
        for (int i = 0; i < ropeLength; i++)
        {
            GameObject currentSegment = Instantiate(ropeSegmentPrefab, _transform);
            currentSegment.transform.localScale = new Vector3(segmentLength, segmentLength, segmentLength);
            currentSegment.GetComponent<Rigidbody2D>().mass = ropeMass;

            Rigidbody2D lastSegmentRigidbody2D = lastSegment.GetComponent<Rigidbody2D>();
            
            HingeJoint2D hingeJoint2D = currentSegment.GetComponent<HingeJoint2D>();
            DistanceJoint2D distanceJoint2D = currentSegment.GetComponent<DistanceJoint2D>();
            hingeJoint2D.connectedBody = lastSegmentRigidbody2D;
            distanceJoint2D.connectedBody = lastSegmentRigidbody2D;
            // First segment needs to be attached in the center of the anchor
            // All other segments need to be connected further down from the last
            if (i != 0)
            {
                hingeJoint2D.connectedAnchor = new Vector2(0, -1);
                distanceJoint2D.connectedAnchor = new Vector2(0, -1);
            }

            lastSegment = currentSegment;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
