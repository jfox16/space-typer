using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    [SerializeField] List<AudioClip> keyPressClips = new List<AudioClip>();

    public void PlayKeyPress()
    {
        if (keyPressClips == null || keyPressClips.Count == 0)
        {
            return;
        }
        
        AudioClip randomKeyPressClip = keyPressClips[Random.Range(0, keyPressClips.Count)];
        // AudioController.Instance.PlayOneShot(randomKeyPressClip);
    }
}
