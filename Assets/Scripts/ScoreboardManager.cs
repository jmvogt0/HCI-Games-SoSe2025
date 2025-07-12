using UnityEngine;
using TMPro;

public class ScoreboardManager : MonoBehaviour
{
    [SerializeField] private Transform scoresParent;       // Parent für Score-Zeilen
    [SerializeField] private GameObject scoreEntryPrefab;  // Prefab für Score-Zeile

    void Start()
    {
        var scoreList = ScoreManager.Instance.LoadScores();

        // Nach Score absteigend sortieren
        scoreList.scores.Sort((a, b) => b.score.CompareTo(a.score));

        foreach (var entry in scoreList.scores)
        {
            GameObject go = Instantiate(scoreEntryPrefab, scoresParent);
            TMP_Text text = go.GetComponent<TMP_Text>();
            text.text = $"{entry.playerName} - {entry.score} - {entry.date}";
        }
    }
}