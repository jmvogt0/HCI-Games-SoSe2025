using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Globalization;

public class ScoreboardManager : MonoBehaviour
{
    [SerializeField] private Transform scoresParent;       // Parent für Score-Zeilen
    [SerializeField] private GameObject scoreEntryPrefab;  // Prefab für Score-Zeile

    void Start()
    {
        Debug.Log("ScoreboardManager gestartet.");
        var scoreList = ScoreManager.Instance.LoadScores();

        // Nach Score absteigend sortieren
        scoreList.scores.Sort((a, b) => b.score.CompareTo(a.score));

        Debug.Log($"Loaded {scoreList.scores.Count} Scores");

        // Finde das neueste gültige Datum
        DateTime latestDate = DateTime.MinValue;
        foreach (var entry in scoreList.scores)
        {
            if (DateTime.TryParse(entry.date, out DateTime parsed))
            {
                if (parsed > latestDate)
                    latestDate = parsed;
            }
        }

        foreach (var entry in scoreList.scores)
        {
            GameObject go = Instantiate(scoreEntryPrefab, scoresParent);
            if (go == null)
            {
                Debug.LogError("FEHLER: Instanziierung fehlgeschlagen!");
                continue;
            }

            var ui = go.GetComponent<ScoreEntryUI>();
            if (ui == null)
            {
                Debug.LogError("FEHLER: ScoreEntryUI-Script NICHT am Prefab!");
                continue;
            }

            bool isLatest = DateTime.TryParse(entry.date, out DateTime parsedDate) && parsedDate == latestDate;
            ui.SetData(entry.playerName, entry.score, entry.date, isLatest);
        }
    }
    public void RetryGame()
    {
        SceneManager.LoadScene("AgeInputScene");
    }

    public void QuitGame()
    {
        //Application.Quit();
        SceneManager.LoadScene("MainMenuScene");
        Debug.Log("Spiel wurde beendet.");
    }
}