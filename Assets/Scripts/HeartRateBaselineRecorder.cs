using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class HeartRateBaselineRecorder : MonoBehaviour
{
    public TMP_Text hrText;
    public TMP_Text timerText;
    public float recordDuration = 60f;

    private List<int> heartRates = new List<int>();
    private float timeRemaining;
    private bool isRecording = false;

    void Start()
    {
        timeRemaining = recordDuration;
        isRecording = true;
        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        while (timeRemaining > 0)
        {
            timerText.text = "Verbleibende Zeit: " + Mathf.CeilToInt(timeRemaining);
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        isRecording = false;
        FinishMeasurement();
    }

    public void OnHeartRateReceived(int bpm)
    {
        hrText.text = "Derzeitige HR: " + bpm;

        if (isRecording)
        {
            heartRates.Add(bpm);
        }
    }

    void FinishMeasurement()
    {
        if (heartRates.Count == 0)
        {
            Debug.LogWarning("Keine Herzfrequenzdaten gesammelt.");
            return;
        }

        heartRates.Sort();
        int median = heartRates[heartRates.Count / 2];

        Debug.Log("Herzfrequenz-Baseline (Median): " + median);
        PlayerPrefs.SetInt("HR_Baseline", median); // für späteren Zugriff im Spiel

        SceneManager.LoadScene("3DGameScene"); // oder beliebige Szene
    }
}