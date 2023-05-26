using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Peut ne pas être un monobehaviour pour une utilisation plus simple

    /// <summary>
    /// Lis aléatoirement une source de la liste audio en OneShot
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioList"></param>
    public void PlayClip(AudioSource audioSource, List<AudioClip> audioList)
    {
        int randomClip = Random.Range(0, audioList.Count);
        audioSource.PlayOneShot(audioList[randomClip]);
    }
    /// <summary>
    /// Lis un audio clip du choix du joueur en One Shot
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="audioList"></param>
    public void PlayClip(AudioSource audioSource, List<AudioClip> audioList, int indexAudio)
    {
        audioSource.PlayOneShot(audioList[indexAudio]);
    }
    /// <summary>
    /// Lis la source audio en OneShot
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="clip"></param>
    public void PlayClip(AudioSource audioSource, AudioClip clip)
    {
            audioSource.PlayOneShot(clip);
    }
    /// <summary>
    /// Lis la source audio en Boucle
    /// </summary>
    /// <param name="audioSource"></param>
    /// <param name="clip"></param>
    /// <param name="isLooping"></param>
    public void PlayClip(AudioSource audioSource, AudioClip clip, bool isLooping)
    {
        if (isLooping)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
    /// <summary>
    /// Fais une transition entre deux sources audio d'une même liste
    /// </summary>
    /// 
    public IEnumerator AlternateClip(AudioSource audioSource, List<AudioClip> list_audioClip, float timeTransition, float volume)
    {
        float percentage = 0f;
        while(audioSource.volume > 0)
        {
            audioSource.volume = Mathf.Lerp(volume, 0, percentage);
            percentage += Time.deltaTime / timeTransition;
            yield return null;
        }
        int i;
        if (audioSource.clip == list_audioClip[0])
        {
            i = 1;
        }
        else { i = 0; }
        audioSource.clip = list_audioClip[i];
        audioSource.Play();
        percentage = 0f;
        while (audioSource.volume < volume)
        {
            audioSource.volume = Mathf.Lerp(0, volume, percentage);
            percentage += Time.deltaTime / timeTransition;
            yield return null;
        }
    }
    public void PauseClip(AudioSource audioSource, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Pause();
    }
}