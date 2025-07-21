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

            Debug.Log($"HeartRateManager initialized with Baseline: {baseline}, Max HR: {maxHR}");
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

    public void SetBaseline(int newBaseline)
    {
        baseline = newBaseline;
        Debug.Log($"Baseline aktualisiert: {baseline}");
    }
    public void SetMaxHR(int newMaxHR)
    {
        maxHR = newMaxHR;
        Debug.Log($"Max HR aktualisiert: {maxHR}");
    }

}