using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region singleton
    public static AudioManager Instance;
    #endregion
    [SerializeField] private AudioSource foundAudioSource;
    [SerializeField] private AudioSource finishAudioSource;
    [SerializeField] private AudioClip select;
    [SerializeField] private AudioClip highlight;
    [SerializeField] private AudioClip complete;
    [SerializeField] private AudioClip finish;
    private AudioSource _audioSource;
    public float foundCounter = 1f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        GameplayController.FoundWord += FoundWord;
        GameplayController.Finish += PlayFinish;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public AudioClip GetHighlight()
    {
        return highlight;
    }
    public AudioClip GetSelect()
    {
        return select;
    }
    public void PlayFinish()
    {
        finishAudioSource.clip = finish;
        finishAudioSource.Play();
    }

    public void FoundWord(Transform one, Transform two)
    {
        foundAudioSource.clip = complete;
        foundAudioSource.pitch = foundCounter;
        foundAudioSource.Play();
        foundCounter += .1f;
    }

    public void PlaySound(AudioClip clip, float pitch)
    {
        _audioSource.clip = clip;
        _audioSource.pitch = pitch;
        _audioSource.Play();
    }

    private void OnDestroy()
    {
        GameplayController.FoundWord -= FoundWord;
        GameplayController.Finish -= PlayFinish;
    }
}
