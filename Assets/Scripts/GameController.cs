using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    [Header("Gameobject references")]
    public GameObject audioManager;
    public GameObject pauseMenu;        // VERY TEMP
    public GameObject ratatosk;

    [Header("Game controller variables")]
    public bool isGamePaused;

    [Header("-------Game adjustment variables-------")]
    [Header("Player")]
    public float terminalVelocity;
    public float playerWalkSpeed;
    public float playerWallFriction;
    public float playerClimbSpeed;
    [Header("Axe")]
    public float maxAxeThrowMagnitude;
    public float minAxeThrowMagnitude;
    public float axeSpeedAmplitude;
    [Header("Rope")]
    public int initialRopeLength;
    
    private AudioManager _audioManager;
    private int _currentScene;

    private void Awake()
    {
        this.audioManager = GameObject.FindWithTag("AudioManager");
        this._currentScene = SceneManager.GetActiveScene().buildIndex;
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
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        switch (_currentScene)
        {
            case -1:        // This is when the player triggers the win condition
                break;
            case 0:         // Menu scene
                break;
            case 1:         // Game scene
                OnGame();
                break;
            default:
                Debug.Log("ERROR: Scene does not exists");
                break;
        }
    }

    public void LoadGameScene()
    {
        // Samplescene - Index 1
        SceneManager.LoadScene(1);
    }

    public void LoadStartMenuScene()
    {
        // Menu - Index 0
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
            Time.timeScale = 0f;
            
            return;
        }
        
        this.isGamePaused = false;
        this._audioManager.PauseAllAudio(false);
        this.pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void StartWinCondition(bool windConditionLeft, Transform playerPosition, float wallCheckRayDistance)
    {
        this._currentScene = -1;
        var distanceToRatatosk = wallCheckRayDistance - 1f;
        var ratatoskPosition = this.ratatosk.transform.position;
        var ratatoskAnimator = this.ratatosk.GetComponent<Animator>();
        this.ratatosk.GetComponent<SpriteRenderer>().enabled = true;
        
        // The player is done, only thing left is to activate Ratatosk
        if (windConditionLeft)
        {
            this.ratatosk.transform.position =
                new Vector3(playerPosition.position.x - distanceToRatatosk, ratatoskPosition.y, ratatoskPosition.z);
            ratatoskAnimator.SetTrigger("spawnToLeft");
            return;
        }

        this.ratatosk.transform.position =
            new Vector3(playerPosition.position.x + distanceToRatatosk, ratatoskPosition.y, ratatoskPosition.z);
        ratatoskAnimator.SetTrigger("spawnToRight");
    }
    
    private void OnGame()
    {
        // Pause game
        if(Input.GetKeyDown(KeyCode.Escape))
            this.PauseGame(!this.isGamePaused);
    }
}