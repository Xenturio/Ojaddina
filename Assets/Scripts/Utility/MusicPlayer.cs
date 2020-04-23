using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;
    AudioSource audioSource;

    public static MusicPlayer Instance { get => instance; }

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        instance.gameObject.GetComponent<AudioSource>().volume = PlayerPrefsController.GetMasterMusicVolume();
        instance.gameObject.GetComponent<AudioSource>().Play();
    }

}
