using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AgeInput : MonoBehaviour
{
    public TMP_InputField ageInputField; // im Inspector zuweisen

    public void ConfirmAge()
    {
        if (int.TryParse(ageInputField.text, out int age))
        {
            int maxHR = CalculateMaxHeartRate(age);

            // HR-Wert im HeartRateManager speichern (muss in der Scene aktiv sein)
            if (HeartRateManager.Instance != null)
            {
                HeartRateManager.Instance.SetMaxHR(maxHR);
            }

            // Optional: Persistenz (z. B. wenn HeartRateManager in späterer Scene geladen wird)
            PlayerPrefs.SetInt("HR_Max", maxHR);
            PlayerPrefs.Save();

            SceneManager.LoadScene("HeartRateBaseline");
        }
        else
        {
            Debug.LogWarning("Ungültige Eingabe für Alter.");
            // Optional: Fehlermeldung im UI anzeigen
        }
    }

    private int CalculateMaxHeartRate(int age)
    {
        return 220 - age;
    }
}