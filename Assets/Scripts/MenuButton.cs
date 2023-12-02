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
    
    private Camera _camera;
    private Animator _animator;
    private PolygonCollider2D _collider;
    
    // Start is called before the first frame update
    private void Start()
    {
        this._camera = Camera.main;
        this._animator = GetComponent<Animator>();
        this._collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        var mouseHover = OnMouseHover();
        this._animator.SetBool("mouseHover", mouseHover);
        
        if (mouseHover && Input.GetMouseButtonDown(0))
        {
            var objectName = this.name;
            
            switch (objectName)
            {
                case "Play":
                    Debug.Log("Play");
                    SceneManager.LoadScene(1);
                    break;
                
                case "HowToPlay":
                    Debug.Log("How to play");
                    this.LoadMenu(false);
                    break;
                
                case "Exit":
                    Debug.Log("Exit");
                    Application.Quit();
                    break;
                
                case "Back":
                    Debug.Log("Back");
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
}
