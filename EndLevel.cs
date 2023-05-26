using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private AudioClip ac_GongSound;
    private Animator animator;
    private AudioManager audioManager;
    private AudioSource audioSource;
    private bool canGong;
    private bool interact;
    private ControlMovePlayer controlMovePlayer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();  
    }

    private void Update()
    {
        interact = Input.GetButton("Interact");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ControlMovePlayer cmp))
        {
            canGong = true;
            controlMovePlayer = cmp;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (interact && canGong && CurrentSceneManager.Instance.objectiveIsDone)
        {
            animator.SetTrigger("Gong");
            StartCoroutine(controlMovePlayer.CantMoving(10f));
        }
            Debug.Log(controlMovePlayer.canMove);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canGong = false;
    }
    private void LaunchSound()
    {
        audioManager.PlayClip(audioSource, ac_GongSound);
    }
    private void LoadNextScene()
    {
        SaveAndLoadSystem.instance.SaveData();
        LoadingScene.instance.LoadScene(CurrentSceneManager.Instance.levelToUnlock + 1); // Charge la scène suivante (décalage entre boutons et num de scène d'ou le +1)
    }
}
