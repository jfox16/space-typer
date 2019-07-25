using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance = null;
    [SerializeField] public float globalVolume = 0.5f;
    AudioSource audioSource = null;
    [SerializeField] AudioClip mainClip = null;

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        PlayClip(mainClip, true);
    }

    public void PlayClip(AudioClip audioClip, bool playLoop)
    {
        if (audioSource != null) {
            audioSource.clip = audioClip;
            audioSource.loop = playLoop;
            audioSource.Play();
        }
    }
}
