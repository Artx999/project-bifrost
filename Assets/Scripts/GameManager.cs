using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public float playerWallFriction;
    public float playerWalkSpeed;
    [Header("Axe")]
    public float maxAxeThrowMag;
    public float minAxeThrowMag;
    public float axeSpeedAmp;
    public bool axeIsSeperated;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /* Reload scene */
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
