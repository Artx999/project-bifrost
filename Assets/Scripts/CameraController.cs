using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform followObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var transform1 = transform;
        var followObjectPosition = followObject.position;
        transform1.position = new Vector3(followObjectPosition.x, followObjectPosition.y, transform1.position.z);
    }
}
