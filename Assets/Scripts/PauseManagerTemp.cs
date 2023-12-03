using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManagerTemp : MonoBehaviour
{
    public GameObject targetToFollow;
    // PUBLIC FIELDS
    [Header("Manager references")]
    public GameObject gameController;
    public GameObject audioManager;
    
    [Header("Menu buttons references")]
    public GameObject mainMenuButton;
    
    // PRIVATE FIELDS
    private Camera _camera;
    private AudioManager _audioManager;
    private GameController _gameController;
    private Animator _mainMenuAnimator;
    
    private bool _hoverSoundFlag = true;

    private void OnEnable()
    {
        this.transform.position = (Vector2)this.targetToFollow.transform.position;
    }

    // Start is called before the first frame update
    private void Start()
    {
        this._audioManager = audioManager.GetComponent<AudioManager>();
        this._gameController = gameController.GetComponent<GameController>();
        this._mainMenuAnimator = mainMenuButton.GetComponent<Animator>();
        this._camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        var mouseHover = OnMouseHover();
        this._mainMenuAnimator.SetBool("mouseHover", mouseHover);
        PlaySfxOnceOnHover(mouseHover);
        
        if (mouseHover && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Main Menu");
            this._audioManager.PlaySfx(this._audioManager.buttonClick);
            this._gameController.PauseGame(false);
            this._gameController.LoadStartMenuScene();
        }
    }
    
    private bool OnMouseHover()
    {
        var desiredMask = LayerMask.GetMask("UI");
        var mousePosition = this.GetMousePosition();

        var hit = Physics2D.Raycast(mousePosition, Vector2.zero, desiredMask);
        return hit;
    }

    private Vector2 GetMousePosition()
    {
        if(this._camera != null)
            return this._camera.ScreenToWorldPoint(Input.mousePosition);
        
        return Vector2.zero;
    }

    private void LoadMenu()
    {
        this._gameController.LoadStartMenuScene();
    }

    private void PlaySfxOnceOnHover(bool mouseHover)
    {
        switch (mouseHover)
        {
            case true when _hoverSoundFlag:
                _hoverSoundFlag = false;
                this._audioManager.PlaySfx(this._audioManager.buttonHover);
                break;
            case false:
                _hoverSoundFlag = true;
                break;
        }
    }
}