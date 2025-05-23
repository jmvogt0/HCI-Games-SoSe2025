using UnityEngine;

public class CAVEManager : MonoBehaviour
{
    public Camera leftCam;
    public Camera centerCam;
    public Camera rightCam;

    void Start()
    {
        // Auflösung für 3 Monitore (z. B. Full HD je Monitor)
        Screen.SetResolution(5760, 1080, false); // 1920 * 3

        // Linke Kamera
        if (leftCam != null)
        {
            leftCam.rect = new Rect(0f, 0f, 1f / 3f, 1f);
            //leftCam.transform.rotation = Quaternion.Euler(0, -45, 0); // Optional: leicht nach innen
        }

        // Zentrale Kamera
        if (centerCam != null)
        {
            centerCam.rect = new Rect(1f / 3f, 0f, 1f / 3f, 1f);
            centerCam.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        // Rechte Kamera
        if (rightCam != null)
        {
            rightCam.rect = new Rect(2f / 3f, 0f, 1f / 3f, 1f);
            //rightCam.transform.rotation = Quaternion.Euler(0, 45, 0); // Optional: leicht nach innen
        }
    }
}