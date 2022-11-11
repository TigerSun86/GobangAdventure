using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSfx : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverAudioClip;
    [SerializeField] private AudioClip missileAudioClip;
    [SerializeField] private AudioClip takeDamageAudioClip;

    public void PlayGameOverAudioClip()
    {
        audioSource.PlayOneShot(gameOverAudioClip);
    }

    public void PlayMissileAudioClip()
    {
        audioSource.PlayOneShot(missileAudioClip);
    }

    public void PlayTakeDamageAudioClip()
    {
        audioSource.PlayOneShot(takeDamageAudioClip);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
