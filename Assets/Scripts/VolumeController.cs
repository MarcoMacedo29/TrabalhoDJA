using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider sfxSlider;

    private const string MasterVolumeParam = "MasterVolume";
    private const string SFXVolumeParam = "SFXVolume";
    private const string MasterVolumePrefKey = "MasterVolume";
    private const string SFXVolumePrefKey = "SFXVolume";

    private void Awake()
    {
        float savedMaster = PlayerPrefs.GetFloat(MasterVolumePrefKey, 1f);
        float savedSFX = PlayerPrefs.GetFloat(SFXVolumePrefKey, 1f);

        if (masterSlider != null)
        {
            masterSlider.minValue = 0f;
            masterSlider.maxValue = 1f;
            masterSlider.value = 1f;
            SetMasterVolume(1f);
        }
        else
        {
            SetMasterVolume(savedMaster);
        }

        if (sfxSlider != null)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = 1f;
            SetSFXVolume(1f);
        }
        else
        {
            SetSFXVolume(savedSFX);
        }
    }

    private void Start()
    {
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMasterVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(MasterVolumeParam, dB);
        PlayerPrefs.SetFloat(MasterVolumePrefKey, sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(SFXVolumeParam, dB);
        PlayerPrefs.SetFloat(SFXVolumePrefKey, sliderValue);
        PlayerPrefs.Save();
    }
}
