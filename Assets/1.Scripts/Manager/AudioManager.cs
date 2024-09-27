using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioClip defaultAudio;
    public float timeToFade = 1.5f;

    private AudioSource track01, track02;
    private bool isPlayingTrack01;

    public static AudioManager instance;

    public AudioMixer audioMixer;
    public AudioMixerGroup audioMixerGroup;

    [Header("Slider")]
    public Slider bgmSlider;
    public Slider sfxSlider;
    public Slider micSlider;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);

        track01 = gameObject.AddComponent<AudioSource>();
        track01.priority = 0;
        track01.loop = true;
        track01.outputAudioMixerGroup = audioMixerGroup;
        track02 = gameObject.AddComponent<AudioSource>();
        track02.priority = 0;
        track02.loop = true;
        track02.outputAudioMixerGroup = audioMixerGroup;
    }

    private void Start()
    {
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        micSlider.onValueChanged.AddListener(SetMicVolume);

        isPlayingTrack01 = true;

        SwapTrack(defaultAudio);
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

    public void SwapTrack(AudioClip newClip)
    {
        StopAllCoroutines();

        StartCoroutine(FadeTrack(newClip));

        isPlayingTrack01 = !isPlayingTrack01;
    }

    public void ReturnToDefault()
    {
        SwapTrack(defaultAudio);
    }

    private IEnumerator FadeTrack(AudioClip newClip)
    {
        float timeElapsed = 0;
        if (isPlayingTrack01)
        {
            track02.clip = newClip;
            track02.Play();

            while (timeElapsed < timeToFade)
            {
                track02.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                track01.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track01.Stop();
        }

        else
        {
            track01.clip = newClip;
            track01.Play();

            while (timeElapsed < timeToFade)
            {
                track01.volume = Mathf.Lerp(0, 1, timeElapsed / timeToFade);
                track02.volume = Mathf.Lerp(1, 0, timeElapsed / timeToFade);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            track02.Stop();
        }
    }
}
