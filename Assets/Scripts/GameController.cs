using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("Gameobject references")]
    public GameObject audioManager;
    public GameObject pauseMenu;        // VERY TEMP

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
        //Application.targetFrameRate = 60;
        this._audioManager = this.audioManager.GetComponent<AudioManager>();
        this.isGamePaused = false;
    }

    private void Update()
    {
        // Reload scene - DEVELOPMENT ONLY
        if (Input.GetKeyDown(KeyCode.R) && !this.isGamePaused)
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

    public void LoadStartMenuScene()
    {
        // Menu - Index
        SceneManager.LoadScene(0);
    }

    public void PauseGame(bool pauseGame)
    {
        this._audioManager.PlaySfx(this._audioManager.pauseAndResume);
        
        if (pauseGame)
        {
            this.isGamePaused = true;
            this._audioManager.PauseAllAudio(true);
            this.pauseMenu.SetActive(true);
            Debug.Log("Paused game");
            Time.timeScale = 0f;
            
            return;
        }
        
        this.isGamePaused = false;
        this._audioManager.PauseAllAudio(false);
        this.pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Resumed game");
    }
}