using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    void Start()
    {
        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
            Debug.Log("Display " + (i + 1) + " aktiviert.");
        }
    }
}