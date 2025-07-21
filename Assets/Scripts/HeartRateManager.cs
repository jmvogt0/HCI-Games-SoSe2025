using UnityEngine;
using TMPro;

public class HeartRateManager : MonoBehaviour
{
    public static HeartRateManager Instance;

    public int baseline = 60;
    public int currentHR = 60;
    public int maxHR = 190;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            baseline = PlayerPrefs.GetInt("HR_Baseline", 60);
            maxHR = PlayerPrefs.GetInt("HR_Max", 190);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHR(int bpm)
    {
        currentHR = bpm;
    }

    public float GetHRRPercent()
    {
        //Debug.Log($"Current HR: {currentHR}, Baseline: {baseline}, Max HR: {maxHR}");
        float hrr = maxHR - baseline;
        if (hrr <= 0) return 0;
        return Mathf.Clamp01((currentHR - baseline) / (float)hrr);
    }
}