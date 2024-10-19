using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour//, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private AudioClip enterAudio;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry;

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) => PlayClickAudio());
        eventTrigger.triggers.Add(entry);

        //entry = new EventTrigger.Entry();
        //entry.eventID = EventTriggerType.PointerEnter;
        //entry.callback.AddListener((eventData) => PlayEnterAudio());
        //eventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Submit;
        entry.callback.AddListener((eventData) => PlayClickAudio());
        eventTrigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Select;
        entry.callback.AddListener((eventData) => PlayEnterAudio());
        eventTrigger.triggers.Add(entry);
    }

    private void PlayClickAudio()
    {
        if (button.interactable)
        {
            audioSource.PlayOneShot(clickAudio);
        }
    }

    private void PlayEnterAudio()
    {
        if (button.interactable)
        {
            audioSource.PlayOneShot(enterAudio);
        }
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (button.interactable)
    //    {
    //        audioSource.PlayOneShot(clickAudio);
    //    }
    //}
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    if (button.interactable)
    //    {
    //        audioSource.PlayOneShot(enterAudio);
    //    }
    //}
}
