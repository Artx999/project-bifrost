using UnityEngine;

public class Parallax : MonoBehaviour
{
    //Variables
    public Camera cam;
    public Transform subject;
    public float parallaxNumber;
    
    private Vector3 _startPosition;
    private float _startZ;
    private Vector2 Travel => (Vector2)cam.transform.position - (Vector2)_startPosition;
    private float DistanceFromSubject => transform.position.z - subject.position.z; 
    private float ClippingPlane => (cam.transform.position.z +(DistanceFromSubject > 0 ? cam.nearClipPlane : cam.nearClipPlane));
    private float ParallaxFactor => (Mathf.Abs(DistanceFromSubject) / ClippingPlane ) * parallaxNumber * -1;
    
    private void Start()
    {
        _startPosition = transform.position;
        _startZ = _startPosition.z;
    }

    private void FixedUpdate()
    {
        //transform.position = startPosition + travel; 
        var newpPos = (Vector2)_startPosition + Travel * ParallaxFactor;
        transform.position = new Vector3(newpPos.x, newpPos.y, _startZ);
    }
}
