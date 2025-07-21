using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreEntryUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text scoreText;
    public TMP_Text dateText;

    public void SetData(string playerName, int score, string date, bool isLatest = false)
    {
        nameText.text = playerName;
        scoreText.text = score.ToString();
        if (DateTime.TryParse(date, out DateTime parsedDate))
        {
            // Deutsches Datumsformat
            dateText.text = parsedDate.ToString("dd.MM.yyyy");
        }
        else
        {
            // Fallback bei ungültigem Datum
            dateText.text = date;
        }
        if (isLatest)
        {
            // Beispiel: Text gelb einfärben
            nameText.color = Color.yellow;
            scoreText.color = Color.yellow;
            dateText.color = Color.yellow;
        }
    }
}