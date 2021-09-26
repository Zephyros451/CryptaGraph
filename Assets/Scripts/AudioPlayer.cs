using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource background;
    [SerializeField] private AudioSource win;
    [SerializeField] private AudioSource lose;

    public static AudioPlayer instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<AudioPlayer>();
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnEnable()
    {
        UnPauseBackground();
    }

    public void PlayWin()
    {
        background.Pause();
        win.Play();
    }

    public void PlayLose()
    {
        background.Pause();
        lose.Play();
    }

    public void UnPauseBackground()
    {
        win.Stop();
        lose.Stop();
        background.Play();
    }
}
