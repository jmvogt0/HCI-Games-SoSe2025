using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayModeStarter
{
    static PlayModeStarter()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            string startScene = "Assets/Scenes/MainMenu.unity"; // passe ggf. Pfad an

            if (SceneManager.GetActiveScene().path != startScene)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(startScene);
                }
                else
                {
                    Debug.LogWarning("Abbruch durch Benutzer – Startszene nicht geladen.");
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}