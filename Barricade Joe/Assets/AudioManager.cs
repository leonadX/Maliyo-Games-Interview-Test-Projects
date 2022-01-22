using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioClip ButtonPress;
    public static AudioManager instance;
    AudioSource audio;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }
    public void ButtonSound()
    {
        PlayAudio(ButtonPress);
    }
    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }
    public void PlayAudio(AudioClip clip, float volume = 1)
    {
        audio.PlayOneShot(clip, volume);
    }
    public void PlayAudio(Audio a)
    {
        audio.PlayOneShot(a.clip, a.volume);
    }
}
[System.Serializable]
public struct Audio
{
    public AudioClip clip;
    public float volume;
}