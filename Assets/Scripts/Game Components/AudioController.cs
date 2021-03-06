using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance = null;

    [SerializeField] public float soundVolume = 0.5f;
    [SerializeField] public float musicVolume = 0.5f;

    [SerializeField] AudioSource introAudioSource = null;
    [SerializeField] AudioSource loopAudioSource = null;
    [SerializeField] AudioSource soundEffectSource = null;
    
    [SerializeField] AudioClip bgMusicIntro = null;
    [SerializeField] AudioClip bgMusicLoop = null;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PlayLoop(bgMusicIntro, bgMusicLoop);
    }

    public void PlayLoop(AudioClip introClip, AudioClip loopClip)
    {
        if (introAudioSource != null && loopAudioSource != null) {
            introAudioSource.clip = introClip;
            introAudioSource.volume = musicVolume;
            loopAudioSource.clip = loopClip;
            loopAudioSource.volume = musicVolume;
            // loopAudioSource.loop = true;
            loopAudioSource.loop = false;
            introAudioSource.Play();
            // loopAudioSource.PlayDelayed(introClip.length);
            Invoke("RepeatLoop", introClip.length);
        }
    }

    void RepeatLoop()
    {
        if (loopAudioSource != null) {
            loopAudioSource.Play();
            Invoke("RepeatLoop", loopAudioSource.clip.length);
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        soundEffectSource.pitch = Random.Range(0.90f, 1.10f);
        if (clip != null) {
            soundEffectSource.PlayOneShot(clip, soundVolume);
        }
    }
}
