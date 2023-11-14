using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RopeHingeJoint : MonoBehaviour
{
    public int ropeLength;
    public float segmentLength;

    private Transform _transform;
    
    public GameObject anchor;
    private Rigidbody2D _anchorRigidbody;
    public GameObject ropeSegmentPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        _transform = this.transform;
        
        // Generate rope
        GameObject lastSegment = anchor;
        for (int i = 0; i < ropeLength; i++)
        {
            GameObject currentSegment = Instantiate(ropeSegmentPrefab, _transform);
            currentSegment.GetComponent<HingeJoint2D>().connectedBody = lastSegment.GetComponent<Rigidbody2D>();
            // First segment needs to be attached in the center of the anchor
            // All other segments need to be connected further down from the last
            if (i != 0)
                currentSegment.GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, -1);
            lastSegment = currentSegment;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
