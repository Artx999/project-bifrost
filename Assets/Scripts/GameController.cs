using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Gameobject references")]
    public GameObject audioManager;

    [Header("Game controller variables")]
    public bool isPauseEnabled;
    public bool isGamePaused;
    
    [Header("Player adjustment variables")]
    public float playerWalkSpeed;
    public float playerWallFriction;
    public float playerClimbSpeed;
    
    [Header("Axe adjustment variables")]
    public float maxAxeThrowMagnitude;
    public float minAxeThrowMagnitude;
    public float axeSpeedAmplitude;
    
    private AudioManager _audioManager;

    private void Awake()
    {
        this.audioManager = GameObject.FindWithTag("AudioManager");
    }

    private void Start()
    {
        this._audioManager = this.audioManager.GetComponent<AudioManager>();
        this.isGamePaused = false;
    }

    private void Update()
    {
        // Reload scene
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        // Pause game
        if(Input.GetKeyDown(KeyCode.Escape) && this.isPauseEnabled)
           this.PauseGame(!this.isGamePaused);
    }

    public void LoadGameScene()
    {
        // Samplescene - Index 1
        SceneManager.LoadScene(1);
    }

    private void PauseGame(bool pauseGame)
    {
        if (pauseGame)
        {
            Time.timeScale = 0f;
            this.isGamePaused = true;
            this._audioManager.PauseAllAudio(true);
            Debug.Log("Paused game");
            return;
        }

        this.isGamePaused = false;
        Time.timeScale = 1f;
        this._audioManager.PauseAllAudio(false);
        Debug.Log("Resumed game");
    }
}