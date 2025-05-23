using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Spiel gestartet.");
        //SceneManager.LoadScene("HeartRateBaseline");
        SceneManager.LoadScene("3DGameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Spiel wurde beendet.");
    }
}