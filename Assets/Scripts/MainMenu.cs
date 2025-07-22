using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Spiel gestartet.");
        //SceneManager.LoadScene("ScoreboardScene");
        SceneManager.LoadScene("AgeInputScene");
        Debug.Log(File.ReadAllText(Application.persistentDataPath + "/scores.json"));
        //SceneManager.LoadScene("3DGameScene");
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}