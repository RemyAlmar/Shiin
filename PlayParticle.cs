using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particle_List;
    private AudioSource audioSource;
    private AudioManager audioManager;
    [SerializeField] private AudioClip ac_soundParticle;
    private void Awake()
    {
        particle_List = GetComponentsInChildren<ParticleSystem>();
        audioSource = GetComponentInParent<AudioSource>();
        audioManager = GetComponentInParent<AudioManager>();
    }

    public void LaunchParticle()
    {
        foreach (var particle in particle_List)
        {
            particle.Play();
        }
    }

    public void PlaySound()
    {
        audioManager.PlayClip(audioSource, ac_soundParticle);
    }
}
