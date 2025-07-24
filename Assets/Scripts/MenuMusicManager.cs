using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager Instance;

    public AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // andere Instanzen vermeiden
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            if (!musicSource.isPlaying)
                musicSource.Play();

            musicSource.Stop();
            musicSource.Play();
            musicSource.volume = 0.15f;
            
        }
    }
}