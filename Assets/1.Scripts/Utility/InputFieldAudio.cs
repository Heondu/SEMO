using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InputFieldAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip submitAudio;
    [SerializeField] private AudioClip selectAudio;
    [SerializeField] private AudioClip typeAudio;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry;


        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => PlaySelectAudio());
        eventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((eventData) => PlaySelectAudio());
        eventTrigger.triggers.Add(entry);

        inputField.onValueChanged.AddListener((s) => PlayTypeAudio());
        inputField.onSubmit.AddListener((s) => PlaySubmitAudio());
    }

    private void PlaySubmitAudio()
    {
        if (inputField.interactable)
        {
            audioSource.PlayOneShot(submitAudio);
        }
    }

    private void PlaySelectAudio()
    {
        if (inputField.interactable)
        {
            audioSource.PlayOneShot(selectAudio);
        }
    }

    private void PlayTypeAudio()
    {
        if (inputField.interactable)
        {
            audioSource.PlayOneShot(typeAudio);
        }
    }
}
