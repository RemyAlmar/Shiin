using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    [SerializeField] private bool isNear;
    [SerializeField] private GameObject iconDialog;
    private Animator animator;
    private AudioSource audioSource;
    private AudioManager audioManager;
    [SerializeField] private List<AudioClip> ac_dialSound_list;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioManager = GetComponent<AudioManager>();
    }
    private void Update()
    {
        if (isNear && Input.GetButtonDown("Interact"))
        {
            TriggerDialogue();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            iconDialog.SetActive(DialogueManager.talkToFirstTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNear = true;
            StartCoroutine(DisplayIcon(isNear));
            iconDialog.SetActive(true);
            animator.SetTrigger("Appear");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNear = false;
            StartCoroutine(DisplayIcon(isNear));
            DialogueManager.instance.EndDialogue();
            animator.SetTrigger("Disappear");
        }
    }

    private IEnumerator DisplayIcon(bool isActive)
    {
        yield return new WaitForSeconds(1.5f);
        iconDialog.SetActive(isActive);
    }

    private void TriggerDialogue()
    {
        if (DialogueManager.talkToFirstTime)
        {
            DialogueManager.instance.StartDialogue(dialogue, audioManager, audioSource, ac_dialSound_list);
        }else
        {
            DialogueManager.instance.DisplayNextSentence(dialogue, audioManager, audioSource, ac_dialSound_list);
        }
    }
}
