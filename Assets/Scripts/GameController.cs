using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Player adjustment variables")]
    public float playerWalkSpeed;
    public float playerWallFriction;
    public float playerClimbSpeed;
    
    [Header("Axe adjustment variables")]
    public float maxAxeThrowMagnitude;
    public float minAxeThrowMagnitude;
    public float axeSpeedAmplitude;
    
    private void Update()
    {
        /* Reload scene */
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadGameScene()
    {
        // Samplescene - Index 1
        SceneManager.LoadScene(1);
    }
}