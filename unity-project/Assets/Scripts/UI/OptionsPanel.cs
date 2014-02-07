using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OptionsPanel : MonoBehaviour {

    public UISlider musicVolSlider;
    public UISlider soundVolSlider;
    public UIPopupList resList;
    public UIPopupList qualityList;

    public AudioClip soundTestSound;

    private AudioSource soundTestSource;

    void OnEnable() {
        musicVolSlider.value = GameManager.Instance.musicVol;
        musicVolSlider.onChange.Add(new EventDelegate(OnMusicVolumeChanged));

        soundVolSlider.value = GameManager.Instance.soundVol;
        soundVolSlider.onChange.Add(new EventDelegate(OnSoundVolumeChanged));
        soundTestSource = gameObject.AddComponent<AudioSource>();

        qualityList.items = new List<string>();
        string[] qualityNames = QualitySettings.names;
        for (int i=0; i<qualityNames.Length; i++) {
            qualityList.items.Add(qualityNames[i]);
        }
        qualityList.value = qualityList.items[QualitySettings.GetQualityLevel()];

        Resolution[] availableResolutions = Screen.resolutions;
        Resolution currentResolution = Screen.currentResolution;
        List<string> resNames = new List<string>(availableResolutions.Length);
        foreach (Resolution res in availableResolutions) {
            resNames.Add(res.width + "x" + res.height);
        }
        resList.items = resNames;
        resList.value = currentResolution.width + "x" + currentResolution.height;
    }

    public void OnMusicVolumeChanged() {
        GameManager.Instance.musicVol = musicVolSlider.value;

        if (Application.loadedLevelName == "mainmenu") {
            Camera.main.GetComponent<AudioSource>().volume = musicVolSlider.value;
        }
    }

    public void OnSoundVolumeChanged() {
        GameManager.Instance.soundVol = soundVolSlider.value;

        soundTestSource.volume = soundVolSlider.value;
        soundTestSource.PlayOneShot(soundTestSound);
    }

    public void OnResolutionValueChange() {
        Resolution currentResolution = Screen.currentResolution;
        string[] dimensions = resList.value.Split(new char[] {'x'});
        int newWidth = int.Parse(dimensions[0]);
        int newHeight = int.Parse(dimensions[1]);

        if (currentResolution.height != newHeight && currentResolution.width != newWidth) {
            Screen.SetResolution(newWidth, newHeight, true);
        }
    }

    public void OnQualityValueChange() {
        List<string> qualityNames = new List<string>(QualitySettings.names);

        if (! qualityList.value.Equals(qualityNames[QualitySettings.GetQualityLevel()])) {
            QualitySettings.SetQualityLevel(qualityNames.IndexOf(qualityList.value));
        }
    }

}
