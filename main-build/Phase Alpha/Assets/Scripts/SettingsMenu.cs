using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject settingsPanel;
    private Resolution[] res;
    public Dropdown resDropdown;
    public AudioMixer audioMixer;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(QualitySettings.GetQualityLevel());
        res = Screen.resolutions;
        resDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResIndex = 0;
        for (int i = 0; i < res.Length; i++)
        {
            string option = res[i].width + " x " + res[i].height;
            options.Add(option);
            if (res[i].width == Screen.currentResolution.width && res[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        resDropdown.AddOptions(options);
        resDropdown.value = currentResIndex;
        resDropdown.RefreshShownValue();
    }

    public void OpenSettings()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void SetQuality(int qualityIndex)
    {
        Debug.Log(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeMusik(float volume)
    {
        audioMixer.SetFloat("volumeMusik", Mathf.Log10(volume) * 20);
    }
    public void SetVolumeEffects(float volume)
    {
        audioMixer.SetFloat("volumeEffects", Mathf.Log10(volume) * 20);
    }

    public void SetFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }

    public void SetRes(int resIndex)
    {
        Resolution resolution = res[resIndex];
        Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
    }
}
