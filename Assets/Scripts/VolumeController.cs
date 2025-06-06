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

    private void Awake()
    {
        masterSlider = transform.Find("MasterSlider").GetComponent<Slider>();
        sfxSlider = transform.Find("SFXSlider").GetComponent<Slider>();

        masterSlider.minValue = 0f;
        masterSlider.maxValue = 1f;
        masterSlider.value = 1f;
        SetMasterVolume(1f);

        sfxSlider.minValue = 0f;
        sfxSlider.maxValue = 1f;
        sfxSlider.value = 1f;
        SetSFXVolume(1f);
    }

    private void Start()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(MasterVolumeParam, dB);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(SFXVolumeParam, dB);
    }
}
