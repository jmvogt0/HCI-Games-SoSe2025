using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Application.persistentDataPath + "/scores.json";
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
        public string date;
    }

    [System.Serializable]
    public class ScoreList
    {
        public List<ScoreEntry> scores = new List<ScoreEntry>();
    }

    public void SaveScore(string name, int score, string date)
    {
        ScoreList scoreList = new ScoreList();

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            scoreList = JsonUtility.FromJson<ScoreList>(json);
        }

        ScoreEntry newEntry = new ScoreEntry { playerName = name, score = score, date = date };
        scoreList.scores.Add(newEntry);

        string newJson = JsonUtility.ToJson(scoreList, true);
        File.WriteAllText(savePath, newJson);
    }

    public ScoreList LoadScores()
    {
        if (!File.Exists(savePath))
            return new ScoreList();

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<ScoreList>(json);
    }
}