using UnityEngine;
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
}