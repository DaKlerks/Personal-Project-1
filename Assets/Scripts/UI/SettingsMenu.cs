using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public GameObject startMenuUI;
    public GameObject SettingsUI;

    public AudioMixer audioMixer;


    public void SetVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void Back()
    {
        SettingsUI.SetActive(false);
        startMenuUI.SetActive(true);
    }
}
