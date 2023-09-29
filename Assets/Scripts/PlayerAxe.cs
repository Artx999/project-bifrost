using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAxe : MonoBehaviour
{
    public GameObject axe;
    private AxeThrow _axeThrow;
    
    void Start()
    {
        _axeThrow = axe.GetComponent<AxeThrow>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {/*
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                //Debug.Log(hit.collider.name);
            } */
            
            _axeThrow.GiveAxeSpeed(GetMouseCLick());
        }
    }

    private Vector2 GetMouseCLick()
    {
        return Vector2.left * 10f;
    }
}
