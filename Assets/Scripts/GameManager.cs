using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int playerLives = 3;
    public Image[] lifeHearts; // UI für Leben
    public int score = 0;
    public TextMeshProUGUI scoreText;

    public Transform playerSpawnPoint; // Spawnpunkt für den Spieler

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
        // Score-UI updaten
        UpdateUI();
    }

    public void LoseLife()
    {
        playerLives--;
        GameObject pacman = GameObject.FindWithTag("Player");

        if (playerLives <= 0)
        {
            GameOver();
        }
        else
        {
            // Respawn-Logik oder Level neu laden
            Debug.Log("Player respawned. Lives left: " + playerLives);
            pacman.transform.position = playerSpawnPoint.position; // Spieler zurück zum Spawnpunkt teleportieren
            UpdateUI();
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
        if (lifeHearts != null && lifeHearts.Length > 0)
        {
            for (int i = 0; i < lifeHearts.Length; i++)
            {
                if (i < playerLives)
                {
                    lifeHearts[i].enabled = true; // Herz anzeigen
                }
                else
                {
                    lifeHearts[i].enabled = false; // Herz ausblenden
                }
            }
        }
    }
}