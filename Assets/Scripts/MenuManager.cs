using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    // PUBLIC FIELDS
    [Header("Manager references")]
    public GameObject gameController;
    public GameObject audioManager;

    [Header("Menus references")]
    public GameObject startMenu;
    public GameObject tutorialMenu;
    
    [Header("Menu buttons references")]
    public GameObject playButton;
    public GameObject tutorialButton;
    public GameObject exitButton;
    public GameObject backButton;
    
    // PRIVATE FIELDS
    private Camera _camera;
    
    private AudioManager _audioManager;
    private GameController _gameController;
    private Animator _playAnimator;
    private Animator _tutorialAnimator;
    private Animator _exitAnimator;
    private Animator _backAnimator;

    [CanBeNull] private GameObject _currentButton;
    private bool _hoverSoundFlag = true;

    private void Awake()
    {
        this.gameController = GameObject.FindWithTag("GameController");
        this.audioManager = GameObject.FindGameObjectWithTag("AudioManager");
    }

    // Start is called before the first frame update
    private void Start()
    {
        this._camera = Camera.main;
        this._audioManager = audioManager.GetComponent<AudioManager>();
        this._gameController = gameController.GetComponent<GameController>();
        this._playAnimator = playButton.GetComponent<Animator>();
        this._tutorialAnimator = tutorialButton.GetComponent<Animator>();
        this._exitAnimator = exitButton.GetComponent<Animator>();
        this._backAnimator = backButton.GetComponent<Animator>();
        this._currentButton = null;
    }

    // Update is called once per frame
    private void Update()
    {
        var mouseHover = OnMouseHover();
        
        if (mouseHover)
        {
            PlaySfxOnceOnHover(true);
            
            if (this._currentButton == this.playButton)
            {
                this._playAnimator.SetBool("mouseHover", true);
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Play");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    _gameController.LoadGameScene();
                }
            }
            else if (this._currentButton == this.tutorialButton)
            {
                this._tutorialAnimator.SetBool("mouseHover", true);
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Tutorial");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    this.LoadMenu(false);
                }
            }
            else if (this._currentButton == this.exitButton)
            {
                this._exitAnimator.SetBool("mouseHover", true);
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Exit");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    Application.Quit();
                }
            }
            else if (this._currentButton == this.backButton)
            {
                this._backAnimator.SetBool("mouseHover", true);
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Back");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    this.LoadMenu(true);
                }
            }
        }
        else
        {
            PlaySfxOnceOnHover(false);
            if (this.startMenu.activeSelf)
            {
                this._playAnimator.SetBool("mouseHover", false);
                this._tutorialAnimator.SetBool("mouseHover", false);
                this._exitAnimator.SetBool("mouseHover", false);
            }
            else if(this.tutorialMenu)
                this._backAnimator.SetBool("mouseHover", false);
        }
    }
    
    private bool OnMouseHover()
    {
        var desiredMask = LayerMask.GetMask("UI");
        var mousePosition = this.GetMousePosition();

        var hit = Physics2D.Raycast(mousePosition, Vector2.zero, desiredMask);
        if (hit)
        {
            this._currentButton = hit.collider.gameObject;
            return true;
        }
        else
        {
            this._currentButton = null;
            return false;
        }
    }

    private Vector2 GetMousePosition()
    {
        if(this._camera != null)
            return this._camera.ScreenToWorldPoint(Input.mousePosition);
        
        return Vector2.zero;
    }

    private void LoadMenu(bool isToStartMenu)
    {
        this.startMenu.SetActive(isToStartMenu);
        this.tutorialMenu.SetActive(!isToStartMenu);
    }

    private void PlaySfxOnceOnHover(bool mouseHover)
    {
        switch (mouseHover)
        {
            case true when _hoverSoundFlag:
                this._hoverSoundFlag = false;
                this._audioManager.PlaySfx(this._audioManager.buttonHover);
                break;
            case false:
                this._hoverSoundFlag = true;
                break;
        }
    }
}
