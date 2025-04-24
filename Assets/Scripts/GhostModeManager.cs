using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostModeManager : MonoBehaviour
{
    public enum GlobalMode { Scatter, Chase }
    public GlobalMode currentMode = GlobalMode.Scatter;

    public float[] phaseDurations = { 7f, 20f, 7f, 20f, 5f, 20f, 5f }; // letzte Chase läuft unendlich
    private int currentPhase = 0;

    private float modeTimer = 0f;

    private GhostMovement[] ghosts;

    void Start()
    {
        ghosts = FindObjectsOfType<GhostMovement>();
        SetMode(GlobalMode.Scatter);
    }

    void Update()
    {
        if (currentPhase >= phaseDurations.Length)
            return; // Endgültiger Chase-Modus

        modeTimer += Time.deltaTime;

        if (modeTimer >= phaseDurations[currentPhase])
        {
            currentPhase++;
            modeTimer = 0f;

            GlobalMode newMode = (currentPhase % 2 == 0) ? GlobalMode.Scatter : GlobalMode.Chase;
            SetMode(newMode);
        }
    }

    void SetMode(GlobalMode newMode)
    {
        currentMode = newMode;

        foreach (GhostMovement ghost in ghosts)
        {
            ghost.SetScatterMode(newMode == GlobalMode.Scatter);
            ghost.ReverseDirection(); // optional, für authentisches Verhalten
        }

        Debug.Log("Ghosts switched to: " + currentMode);
    }
}