using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.xenturio.UI
{
    public class UISoundPlayer : MonoBehaviour
    {
        public Toggle toggleSound;
        private static UISoundPlayer instance;
        public static UISoundPlayer Instance { get => instance; }
        private AudioSource[] audioSources;
        

        // Start is called before the first frame update
        void Start()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            audioSources = FindObjectsOfType<AudioSource>();
            foreach(AudioSource audioSource in audioSources)
            {
                audioSource.volume = PlayerPrefsController.GetMasterUIVolume();
            }
        }

        public void MuteSound() {
            bool mute = !toggleSound.isOn;
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.volume = mute ? 0 : 1;
                PlayerPrefsController.SetMasterUIVolume(audioSource.volume);
            }
        }
    }
}