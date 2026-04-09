using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Music")]
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 0.5f;

    private AudioSource _source;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _source = gameObject.AddComponent<AudioSource>();
        _source.clip = clip;
        _source.loop = true;
        _source.volume = volume;
        _source.playOnAwake = false;
        _source.Play();
    }
}
