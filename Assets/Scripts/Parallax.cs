using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Variables
    public Camera cam;
    public Transform subject;
    public float parallaxNumber;
    Vector2 startPosition;
    float startZ;
    Vector2 travel => (Vector2)cam.transform.position - startPosition;
    float distancefromSubject => transform.position.z - subject.position.z; 
    float clippingPlane => (cam.transform.position.z +(distancefromSubject > 0 ? cam.nearClipPlane : cam.nearClipPlane));
    float parallaxFactor => (Mathf.Abs(distancefromSubject) / clippingPlane ) * parallaxNumber * -1;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = startPosition + travel; 
        Vector2 newpPos = startPosition + travel * parallaxFactor;
        transform.position = new Vector3(newpPos.x, newpPos.y, startZ);
    }
}
