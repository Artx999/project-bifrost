using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public GameObject startMenu = null;
    public GameObject controlsMenu = null;
    public GameObject audioManager;
    
    private Camera _camera;
    private Animator _animator;
    private PolygonCollider2D _collider;
    private AudioManager _audioManager;
    private bool _hoverSoundFlag = true;

    private void Awake()
    {
        this.audioManager = GameObject.FindGameObjectWithTag("AudioManager");
    }

    // Start is called before the first frame update
    private void Start()
    {
        this._camera = Camera.main;
        this._animator = GetComponent<Animator>();
        this._collider = GetComponent<PolygonCollider2D>();
        this._audioManager = audioManager.GetComponent<AudioManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        var mouseHover = OnMouseHover();
        this._animator.SetBool("mouseHover", mouseHover);
        PlaySfxOnceOnHover(mouseHover);

        if (mouseHover && Input.GetMouseButtonDown(0))
        {
            var objectName = this.name;
            
            switch (objectName)
            {
                case "Play":
                    Debug.Log("Play");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    SceneManager.LoadScene(1);
                    break;
                
                case "HowToPlay":
                    Debug.Log("How to play");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    this.LoadMenu(false);
                    break;
                
                case "Exit":
                    Debug.Log("Exit");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    Application.Quit();
                    break;
                
                case "Back":
                    Debug.Log("Back");
                    this._audioManager.PlaySfx(this._audioManager.buttonClick);
                    this.LoadMenu(true);
                    break;
                
                default:
                    Debug.Log("ERROR: Check gameobject names");
                    break;
            }
        }
    }

    private bool OnMouseHover()
    {
        var mousePosition = this.GetMousePosition();

        var hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        return hit.collider == this._collider;
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
        this.controlsMenu.SetActive(!isToStartMenu);
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
