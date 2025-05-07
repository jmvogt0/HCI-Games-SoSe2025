using UnityEngine;

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
        float hrr = maxHR - baseline;
        if (hrr <= 0) return 0;
        return Mathf.Clamp01((currentHR - baseline) / (float)hrr);
    }
}