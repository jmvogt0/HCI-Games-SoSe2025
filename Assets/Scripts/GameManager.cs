using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int playerLives = 3;
    public TextMeshProUGUI countdownText; // UI-Element für den Countdown
    public Image[] lifeHearts; // UI für Leben

    [Header("Score")]
    public int score = 0;
    public TextMeshProUGUI scoreText;
    [SerializeField] private AudioSource scoreAudio;
    [SerializeField] private AudioSource gameOverAudio;

    [Header("UI – Herz - BPM")]
    public TextMeshProUGUI heartRateText;         // für BPM-Anzeige
    public RectTransform heartIconTransform;      // fürs Pulsieren
    public float pulseAmplitude = 0.1f;           // Max-Skalierungs-Abweichung
    [SerializeField] private AudioSource heartbeatAudio;
    private float previousSin = 0f;

    [Header("Name Input")]

    [SerializeField] private GameObject nameInputPanel; // Panel für Nameingabe
    [SerializeField] private TMP_InputField nameInputField; // InputField für Namen

    [Header("User Battery")]

    [SerializeField] private Image batteryImage;
    [SerializeField] private float batteryChargeRate = 0.2f;   // Aufladung je nach HRR
    [SerializeField] private float batteryCapacity = 1f;       // Maximale Batterieladung (0–1 normalisieren)
    [SerializeField] private TextMeshProUGUI batteryText;
    private float batteryLevel = 0f;                           // Aktueller Ladezustand (0–batteryCapacity)

    public float GetBatteryLevelNormalized() => Mathf.Clamp01(batteryLevel / batteryCapacity);

    

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

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        // Herzfrequenz-UI aktualisieren
        UpdateHeartRateUI();

        // Herz-Icon pulsieren lassen
        AnimateHeartIcon();

        ChargeBattery();
    }

    private void ChargeBattery()
    {
        if (HeartRateManager.Instance == null)
            return;

        float hrr = HeartRateManager.Instance.GetHRRPercent(); // 0–1
        float chargeAmount = hrr * batteryChargeRate * Time.deltaTime;

        batteryLevel += chargeAmount;
        batteryLevel = Mathf.Clamp(batteryLevel, 0f, batteryCapacity);

        if (batteryImage != null)
            batteryImage.fillAmount = GetBatteryLevelNormalized();
            
        if (batteryText != null)
        {
            batteryText.text = $"Battery: {(batteryLevel / batteryCapacity * 100f):F0}%";
        }
    }

    public void AddScore(int amount)
    {
        score += amount;

        scoreAudio?.Play();

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
            pacman.GetComponent<FirstPersonGridMovement>().StopMovement();
            // Nun noch Ghosts zurücksetzen
            UpdateUI();
            GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
            foreach (GameObject ghost in ghosts)
            {
                ghost.GetComponent<GhostMovement>().ResetAfterTeleport(new Vector3(13f, 0.8f, -13.5f), Quaternion.Euler(0, 0, 0), Vector3.right);
                ghost.GetComponent<GhostMovement>().StopMovement();
            }
             StartCoroutine(StartCountdown());
        }
    }

    void GameOver()
    {
        // Game Over-Logik
        Debug.Log("Game Over");

        gameOverAudio?.Play();

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

        nameInputPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void OnSubmitName()
    {
        string playerName = nameInputField.text;
        string date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        int playerScore = score;

        // Score speichern
        ScoreManager.Instance.SaveScore(playerName, playerScore, date);

         // GhostModeManager stoppen, bevor Szene gewechselt wird
        GhostModeManager gmm = FindObjectOfType<GhostModeManager>();
        if (gmm != null)
        {
            Destroy(gmm.gameObject); // oder: gmm.enabled = false;
        }

        // Scoreboard-Szene laden
        UnityEngine.SceneManagement.SceneManager.LoadScene("ScoreboardScene");
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

        float sinValue = Mathf.Sin(Time.time * pulseFreq * 2f * Mathf.PI);
        float scaleOffset = sinValue * pulseAmplitude;
        float scale = 1f + scaleOffset;
        heartIconTransform.localScale = new Vector3(scale, scale, 1f);

        // ➕ Sound bei positiver Nullüberschreitung
        if (previousSin < 0f && sinValue >= 0f)
        {
            heartbeatAudio.Play();
        }

        previousSin = sinValue;
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
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

    IEnumerator StartCountdown()
    {
        countdownText.gameObject.SetActive(true);

        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);

        StartGameplay();
    }

    void StartGameplay()
    {
        // Hier Gameplay starten oder Bewegungen aktivieren
        Debug.Log("Gameplay gestartet!");
        GameObject pacman = GameObject.FindWithTag("Player");
        if (pacman != null)
            pacman.GetComponent<FirstPersonGridMovement>().EnableMovement();

        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in ghosts)
        {
            ghost.GetComponent<GhostMovement>().StartDelayedMovement();
        }
    }
}