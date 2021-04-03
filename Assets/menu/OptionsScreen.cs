using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsScreen : MonoBehaviour
{
    public event Action Close;

    [SerializeField] Slider masterVolumeSlider, sfxVolumeSlider, musicVolumeSlider;
    [SerializeField] TextMeshProUGUI masterVolumeText, sfxVolumeText, musicVolumeText;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Audio pressButtonSound;

    private void UpdateUI(AudioGroup group, Slider slider, TextMeshProUGUI text)
    {
        var value = AudioManager.audioManger.GetValue(group);
        slider.value = value;
        text.text = Mathf.Round(value).ToString();
    }

    void OnEnable()
    {
        UpdateUI(AudioManager.Master, masterVolumeSlider, masterVolumeText);
        UpdateUI(AudioManager.Effects, sfxVolumeSlider, sfxVolumeText);
        UpdateUI(AudioManager.Music, musicVolumeSlider, musicVolumeText);
    }

    public void SetMasterVolume(float volume)
    {
        if (!isActiveAndEnabled) return;
        AudioManager.audioManger.SetValue(AudioManager.Master, volume);
        UpdateUI(AudioManager.Master, masterVolumeSlider, masterVolumeText);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void SetSFXVolume(float volume)
    {
        if (!isActiveAndEnabled) return;
        AudioManager.audioManger.SetValue(AudioManager.Effects, volume);
        UpdateUI(AudioManager.Effects, sfxVolumeSlider, sfxVolumeText);
        AudioManager.audioManger.PlaySFX(pressButtonSound);
    }

    public void SetMusicVolume(float volume)
    {
        if (!isActiveAndEnabled) return;
        AudioManager.audioManger.SetValue(AudioManager.Music, volume);
        UpdateUI(AudioManager.Music, musicVolumeSlider, musicVolumeText);
    }

    public void DoClose()
    {
        Close?.Invoke();
    }
}
