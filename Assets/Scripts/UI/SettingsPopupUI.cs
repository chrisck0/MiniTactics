using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupUI : PopupUI
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void OnEnable() => BindSliderEvents();
    private void OnDisable() => UnbindSliderEvents();

    private void BgmChange(float volume)
    {
        AudioManager.Instance.SetBgmVolume(volume);
    }

    private void SfxChange(float volume)
    {
        AudioManager.Instance.SetSfxVolume(volume);
    }

    private void BindSliderEvents()
    {
        float bgmVolume = AudioManager.Instance.GetBgmVolume() * AudioManager.AUDIO_RATE;
        _bgmSlider.value = bgmVolume;
        float sfxVolume = AudioManager.Instance.GetSfxVolume() * AudioManager.AUDIO_RATE;
        _sfxSlider.value = sfxVolume;
        _bgmSlider.onValueChanged.AddListener(BgmChange);
        _sfxSlider.onValueChanged.AddListener(SfxChange);
    }

    private void UnbindSliderEvents()
    {
        _bgmSlider.onValueChanged.RemoveListener(BgmChange);
        _sfxSlider.onValueChanged.RemoveListener(SfxChange);
    }
}
