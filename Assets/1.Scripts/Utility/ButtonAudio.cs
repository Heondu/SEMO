using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickAudio;
    [SerializeField] private AudioClip enterAudio;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button.interactable)
        {
            audioSource.PlayOneShot(clickAudio);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            audioSource.PlayOneShot(enterAudio);
        }
    }
}
