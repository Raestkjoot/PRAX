using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    // Singleton
    private static MusicPlayer _instance = null;
    public static MusicPlayer Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            _audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Mute()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
            else
            {
                _audioSource.Play();
            }
        }
    }
    public void SetIsPlaying(bool play) 
    {
        if(play)
            _audioSource.Play();
        else _audioSource.Pause();
    }
}