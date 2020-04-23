using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = PlayerPrefsController.GetMasterMusicVolume();
    }

    public void OnVolumeSliderChange() {
        PlayerPrefsController.SetMasterMusicVolume(volumeSlider.value);
    }
}
