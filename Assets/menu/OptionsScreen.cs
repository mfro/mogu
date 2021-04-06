using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class OptionsScreen : MonoBehaviour
{
    public event Action Close;

    [SerializeField] Slider masterVolumeSlider, sfxVolumeSlider, musicVolumeSlider;
    [SerializeField] TextMeshProUGUI masterVolumeText, sfxVolumeText, musicVolumeText;

    [SerializeField] Slider animationSpeedSlider;
    [SerializeField] TextMeshProUGUI animationSpeedText;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Audio pressButtonSound;

    [SerializeField] GameObject[] buttons;

    private void UpdateUI(AudioGroup group, Slider slider, TextMeshProUGUI text)
    {
        var value = AudioManager.instance.GetValue(group);
        slider.value = value;
        text.text = Mathf.Round(value).ToString();
    }

    void OnEnable()
    {
        UpdateUI(AudioManager.Master, masterVolumeSlider, masterVolumeText);
        UpdateUI(AudioManager.Effects, sfxVolumeSlider, sfxVolumeText);
        UpdateUI(AudioManager.Music, musicVolumeSlider, musicVolumeText);

        animationSpeedSlider.value = 1 - FlipPanel.FlipTime;
        animationSpeedText.text = $"{FlipPanel.FlipTime:#.##}s";

        SetSelected();
    }

    async void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        await Task.Yield();
        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    public void SetAnimationSpeed(float value)
    {
        if (!isActiveAndEnabled) return;
        FlipPanel.FlipTime = 1 - value;

        animationSpeedText.text = $"{FlipPanel.FlipTime:#.##}s";
    }

    public void SetMasterVolume(float volume)
    {
        if (!isActiveAndEnabled) return;
        AudioManager.instance.SetValue(AudioManager.Master, volume);
        UpdateUI(AudioManager.Master, masterVolumeSlider, masterVolumeText);
        AudioManager.instance.PlaySFX(pressButtonSound);
    }

    public void SetSFXVolume(float volume)
    {
        if (!isActiveAndEnabled) return;
        AudioManager.instance.SetValue(AudioManager.Effects, volume);
        UpdateUI(AudioManager.Effects, sfxVolumeSlider, sfxVolumeText);
        AudioManager.instance.PlaySFX(pressButtonSound);
    }

    public void SetMusicVolume(float volume)
    {
        if (!isActiveAndEnabled) return;
        AudioManager.instance.SetValue(AudioManager.Music, volume);
        UpdateUI(AudioManager.Music, musicVolumeSlider, musicVolumeText);
    }

    public void DoClose()
    {
        Close?.Invoke();
    }
}
