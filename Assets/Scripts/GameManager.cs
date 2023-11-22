using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("General Variable States")]
    public bool axeIsSeperated;
    [Header("Player")]
    public float playerWallFriction;
    public float playerWalkSpeed;
    [Header("Axe")]
    public float maxAxeThrowMag;
    public float minAxeThrowMag;
    public float axeSpeedAmp;
    
    // Start is called before the first frame update
    private void Start()
    {
        this.axeIsSeperated = false;
    }

    // Update is called once per frame
    private void Update()
    {
        /* Reload scene */
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
