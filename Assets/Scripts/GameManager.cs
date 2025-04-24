using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerLives = 3;
    public int score = 0;
    public Text scoreText;

    void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void LoseLife()
    {
        playerLives--;

        if (playerLives <= 0)
        {
            GameOver();
        } 
        else
        {
            // Respawn-Logik oder Level neu laden
            Debug.Log("Player respawned. Lives left: " + playerLives);
        }
    }

    void GameOver()
    {
        // Game Over-Logik
        Debug.Log("Game Over");

        // Pacman stoppen
        GameObject pacman = GameObject.FindWithTag("Player");
        if (pacman != null)
        {
            pacman.GetComponent<PlayerGridMovement>().StopMovement();
        }

        //Ghosts stoppen
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostMovement>().StopMovement();
        }

    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}