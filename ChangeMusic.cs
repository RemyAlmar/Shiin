using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioManager audioManager;
    [SerializeField] List<AudioClip> list_music;
    [SerializeField] bool hasChanged;
    [SerializeField, Range(0f, 2f)] private float timeTransition;
    [SerializeField, Range(0f, 1f)] private float defaultVolume;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        LaunchClip();
    }
    private void LaunchClip()
    {
        if (AmbiantMusicManager.isSpotted == true)
        {
            if (!hasChanged)
            {
                StartCoroutine(audioManager.AlternateClip(audioSource, list_music, timeTransition, defaultVolume));
                hasChanged = true;
            }
        }
        else if (AmbiantMusicManager.isSpotted == false)
        {
            if (hasChanged)
            {
                StartCoroutine(audioManager.AlternateClip(audioSource, list_music, timeTransition, defaultVolume));
                hasChanged = false;
            }
        }
    }
}
