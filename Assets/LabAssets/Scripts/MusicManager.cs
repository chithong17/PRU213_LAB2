using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    const string RootName = "__MusicManager";

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip endGameMusic;

    static MusicManager instance;

    AudioSource source;
    AudioClip activeClip;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Ensure().PlayForScene(SceneManager.GetActiveScene().name);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Ensure().PlayForScene(scene.name);
    }

    static MusicManager Ensure()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject existing = GameObject.Find(RootName);
        if (existing != null && existing.TryGetComponent(out MusicManager existingManager))
        {
            instance = existingManager;
            return instance;
        }

        GameObject managerObject = new GameObject(RootName);
        instance = managerObject.AddComponent<MusicManager>();
        DontDestroyOnLoad(managerObject);
        return instance;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        source = GetComponent<AudioSource>();
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.38f;
    }

    void PlayForScene(string sceneName)
    {
        AudioClip targetClip = GetMusicClip(sceneName);

        if (targetClip == null)
        {
            StopMusic();
            return;
        }

        if (activeClip == targetClip && source != null && source.isPlaying)
        {
            source.volume = GetVolume(sceneName);
            return;
        }

        activeClip = targetClip;
        source.clip = targetClip;
        source.volume = GetVolume(sceneName);
        source.Play();
    }

    AudioClip GetMusicClip(string sceneName)
    {
        if (sceneName == SceneFlow.MenuSceneName)
        {
            return menuMusic;
        }

        if (sceneName == SceneFlow.EndGameSceneName)
        {
            return endGameMusic;
        }

        return SceneFlow.IsGameScene(sceneName) ? gameplayMusic : null;
    }

    static float GetVolume(string sceneName)
    {
        if (SceneFlow.IsGameScene(sceneName))
        {
            return 0.34f;
        }

        return sceneName == SceneFlow.EndGameSceneName ? 0.4f : 0.36f;
    }

    void StopMusic()
    {
        activeClip = null;
        if (source != null)
        {
            source.Stop();
            source.clip = null;
        }
    }
}