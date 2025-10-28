using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager: MonoBehaviour
{
    private AudioSource bgm;
    [SerializeField] public BGMConfig cfg;

    void Awake()
    {
        bgm = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (cfg.enabledBGM)
        {
            bgm.Play();
        }
        else
        {
            bgm.Stop();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
