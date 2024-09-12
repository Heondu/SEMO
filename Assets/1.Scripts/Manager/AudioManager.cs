using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager singleton;

    public AudioMixer audioMixer;

    [Header("Slider")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider micSlider;

    private void Start()
    {
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        micSlider.onValueChanged.AddListener(SetMicVolume);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMicVolume(float volume)
    {
        audioMixer.SetFloat("MICVolume", Mathf.Log10(volume) * 20);
    }
}
