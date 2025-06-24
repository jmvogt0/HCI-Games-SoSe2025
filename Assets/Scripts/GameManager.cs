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

    [Header("UI – Herz")]
    public TextMeshProUGUI heartRateText;         // für BPM-Anzeige
    public RectTransform heartIconTransform;      // fürs Pulsieren
    public float pulseAmplitude = 0.1f;           // Max-Skalierungs-Abweichung

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

    void Update()
    {
        // Herzfrequenz-UI aktualisieren
        UpdateHeartRateUI();

        // Herz-Icon pulsieren lassen
        AnimateHeartIcon();
    }

    public void AddScore(int amount)
    {
        score += amount;
        // Score-UI updaten
        UpdateUI();

                // nach jedem gesammelten Dot prüfen, ob keine mehr da sind:
        CheckForWin();
    }

    // 1. Prüft, ob noch Objekte mit Tag "Dot" existieren
    private void CheckForWin()
    {
        // FindGameObjects… liefert alle GameObjects mit Tag
        if (GameObject.FindGameObjectsWithTag("Dot").Length == 0)
        {
            OnAllDotsCollected();
        }
    }

    // 2. Gewinnzustand behandeln
    private void OnAllDotsCollected()
    {
        Debug.Log("Gewonnen! Alle Dots gesammelt.");
        // SceneManager.LoadScene("WinScene");
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
            //pacman.transform.position = playerSpawnPoint.position; // Spieler zurück zum Spawnpunkt teleportieren
            //pacman.transform.rotation = playerSpawnPoint.rotation; // Spieler zurück zur Startrotation teleportieren
            pacman.GetComponent<FirstPersonGridMovement>().ResetAfterTeleport(new Vector3(13f, 0.6f, -23f), Quaternion.Euler(0, 90f, 0));
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
            pacman.GetComponent<FirstPersonGridMovement>().StopMovement();
        }

        //Ghosts stoppen
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostMovement>().StopMovement();
        }

    }

    private void UpdateHeartRateUI()
    {
        if (HeartRateManager.Instance == null || heartRateText == null)
            return;

        int hr = HeartRateManager.Instance.currentHR;

        heartRateText.text = $"{hr:F0}";
    }

    private void AnimateHeartIcon()
    {
        if (heartIconTransform == null || HeartRateManager.Instance == null)
            return;

        int hr = HeartRateManager.Instance.currentHR;

        float pulseFreq = hr / 60f; // Pulse pro Sekunde
        float scaleOffset = Mathf.Sin(Time.time * pulseFreq * 2f * Mathf.PI) * pulseAmplitude;
        float scale = 1f + scaleOffset;
        heartIconTransform.localScale = new Vector3(scale, scale, 1f);
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