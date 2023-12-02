using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Imlemented 
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Function for PlayButton
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    //Function for QuitButton
    public void QuitGame()
    {
        //For testing purposes
        Debug.Log("QUIT");

        //To actually quit when not in Unity
        Application.Quit();
    }
}
