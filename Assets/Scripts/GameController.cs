using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Player adjustment variables")]
    public float playerWallFriction;
    public float playerWalkSpeed = 1f;
    
    [Header("Axe adjustment variables")]
    public float maxAxeThrowMag;
    public float minAxeThrowMag;
    public float axeSpeedAmp;
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        /* Reload scene */
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}