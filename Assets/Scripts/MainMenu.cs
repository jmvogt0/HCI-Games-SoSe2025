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
        Application.Quit();
        Debug.Log("Spiel wurde beendet.");
    }
}