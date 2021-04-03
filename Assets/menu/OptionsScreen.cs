using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    public event Action Close;

    private static float maxMaster = 0;
    private static float maxSFX = 0;
    private static float maxMusic = 0;

    [SerializeField] Slider masterVolumeSlider, sfxVolumeSlider, musicVolumeSlider;
    [SerializeField] TextMeshProUGUI masterVolumeText, sfxVolumeText, musicVolumeText;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Audio pressButtonSound;

    private static float SliderToVolume(float slider, float min, float max)
    {
        if (slider == 0) return -80;

        return Mathf.Lerp(min, max, slider / 100);
    }

    private static float VolumeToSlider(float volume, float min, float max)
    {
        if (volume == -80) return 0;

        return 100 * (volume - min) / (max - min);
    }

    private void UpdateUI(string name, float max, Slider slider, TextMeshProUGUI text)
    {
        float volume;
        audioMixer.GetFloat(name, out volume);
        print($"{name} {volume}");
        slider.value = VolumeToSlider(volume, -35, max);
        text.text = Mathf.Round(VolumeToSlider(volume, -35, max)).ToString();
    }

    void OnEnable()
    {
        UpdateUI("MasterVolume", maxMaster, masterVolumeSlider, masterVolumeText);
        UpdateUI("SFXVolume", maxSFX, sfxVolumeSlider, sfxVolumeText);
        UpdateUI("MusicVolume", maxMusic, musicVolumeSlider, musicVolumeText);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", SliderToVolume(volume, -35, maxMaster));
        UpdateUI("MasterVolume", maxMaster, masterVolumeSlider, masterVolumeText);
        if (isActiveAndEnabled) AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", SliderToVolume(volume, -35, maxSFX));
        UpdateUI("SFXVolume", maxSFX, sfxVolumeSlider, sfxVolumeText);
        if (isActiveAndEnabled) AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", SliderToVolume(volume, -35, maxMusic));
        UpdateUI("MusicVolume", maxMusic, musicVolumeSlider, musicVolumeText);
    }

    public void DoClose()
    {
        Close?.Invoke();
    }
}
