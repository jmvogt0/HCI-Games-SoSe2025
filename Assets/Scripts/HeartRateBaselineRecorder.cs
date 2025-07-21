using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections;

public class HeartRateBaselineRecorder : MonoBehaviour
{
    public TMP_Text hrText;

    public RectTransform heartIconTransform;      // fürs Pulsieren
    public float pulseAmplitude = 0.1f;           // Max-Skalierungs-Abweichung
    [SerializeField] private AudioSource heartbeatAudio;
    private float previousSin = 0f;
    public TMP_Text timerText;
    public float recordDuration = 60f;

    private List<int> heartRates = new List<int>();
    private float timeRemaining;
    private bool isRecording = false;

    private int currentBPM = 0;


    public GameObject NormalUI;
    public GameObject ErrorUI;

    void Start()
    {
        timeRemaining = recordDuration;
        isRecording = true;
        StartCoroutine(Countdown());
    }

    void Update()
    {
        if (currentBPM > 0)
        {
            AnimateHeartIcon(currentBPM);
        }
    }

    IEnumerator Countdown()
    {
        while (timeRemaining > 0)
        {
            timerText.text = $"Messung endet in: {Mathf.CeilToInt(timeRemaining)}s";
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        isRecording = false;
        FinishMeasurement();
    }

    public void OnHeartRateReceived(int bpm)
    {
        DisplayHeartRate(bpm);
        currentBPM = bpm;


        if (isRecording)
        {
            heartRates.Add(bpm);
        }
    }

    public void DisplayHeartRate(int bpm)
    {
        hrText.text = bpm.ToString();
    }

    private void AnimateHeartIcon(int bpm)
    {
        Debug.Log("AnimateHeartIcon called with BPM: " + bpm);
        if (heartIconTransform == null || bpm == null)
            return;
        Debug.Log("HeartIconTransform and BPM are valid.");

        float pulseFreq = bpm / 60f; // Pulse pro Sekunde

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

    void FinishMeasurement()
    {
        if (heartRates.Count == 0)
        {
            Debug.LogWarning("Keine Herzfrequenzdaten gesammelt.");
            NormalUI.SetActive(false);
            ErrorUI.SetActive(true);
            return;
        }

        heartRates.Sort();
        int median = heartRates[heartRates.Count / 2];

        Debug.Log("Herzfrequenz-Baseline (Median): " + median);
        PlayerPrefs.SetInt("HR_Baseline", median); // für späteren Zugriff im Spiel

        SceneManager.LoadScene("3DGameScene"); // oder beliebige Szene
    }
    
    public void RestartMeasurement()
    {
        NormalUI.SetActive(true);
        ErrorUI.SetActive(false);
        heartRates.Clear();
        timeRemaining = recordDuration;
        isRecording = true;
        StartCoroutine(Countdown());
    }
}