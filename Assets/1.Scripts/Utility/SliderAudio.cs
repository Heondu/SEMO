using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip slideAudio;

    private Slider slider;
    private float lastChangedValue;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(OnValueChanged);
        lastChangedValue = slider.value;
    }

    private void OnValueChanged(float value)
    {
        if (lastChangedValue - value < -0.05f || lastChangedValue - value > 0.05f)
        {
            audioSource.PlayOneShot(slideAudio);
            lastChangedValue = value;
        }
    }
}
