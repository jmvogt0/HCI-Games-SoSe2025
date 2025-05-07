using UnityEngine;
using TMPro;

public class HeartRateDisplay : MonoBehaviour
{
    public TMP_Text heartRateText;

    void Update()
    {
        if (HeartRateManager.Instance == null)
        {
            Debug.LogWarning("HeartRateManager.Instance is null");
            return;
        }

        Debug.Log("HR: " + HeartRateManager.Instance.currentHR);
        Debug.Log("Test");

        int bpm = HeartRateManager.Instance.currentHR;
        int baseline = HeartRateManager.Instance.baseline;
        int maxHR = HeartRateManager.Instance.maxHR;

        float hrrPercent = HeartRateManager.Instance.GetHRRPercent() * 100f;

        heartRateText.text = $"HR: {bpm} BPM\nHRR: {hrrPercent:F0}%\n(Baseline: {baseline}, Max: {maxHR})";
    }
}