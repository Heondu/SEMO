using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField]
    private MovePattern movePattern;
    [SerializeField]
    private TrailRenderer trail;
    [SerializeField]
    private AudioClip collideAudio;
    [SerializeField]
    private AudioSource audioSource;

    private void Start()
    {
        if (movePattern)
        {
            movePattern.Setup(transform, trail);
        }
    }

    private void Update()
    {
        if (movePattern)
        {
            movePattern.Apply(transform);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collideAudio) 
        {
            audioSource.PlayOneShot(collideAudio);
        }
    }
}
