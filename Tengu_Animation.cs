using UnityEngine;
using System.Collections.Generic;

public class Tengu_Animation : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private List<AudioClip> ac_list_stepFoot;
    [SerializeField] private AudioClip ac_drawKatana;
    [SerializeField] private AudioClip ac_shealthKatana;
    [SerializeField] private AudioClip ac_dashAttack;
    private AudioManager audioManager;
    private AudioSource audioSource;

    private ComportementAI comportementAI;
    private Rigidbody2D rb;
    private Animator animator;
    private Assassination assassination;
    private ActionAI actionAI;
    // Start is called before the first frame update
    void Awake()
    {
        audioManager = GetComponentInParent<AudioManager>();
        audioSource = GetComponentInParent<AudioSource>();
        rb = GetComponentInParent<Rigidbody2D>();
        comportementAI = GetComponentInParent<ComportementAI>();
        animator = GetComponentInParent<Animator>();
        assassination = GetComponentInParent<Assassination>();
        actionAI = GetComponentInParent<ActionAI>();
    }

    // Update is called once per frame
    void Update()
    {
        LaunchAnimation();
    }
    
    private void LaunchAnimation()
    {
        animator.SetBool("IsFollowingTarget", comportementAI.IsFollowingTarget);
        animator.SetBool("IsAlerted", comportementAI.IsAlerted);
        animator.SetBool("IsJumping", comportementAI.IsJumping);
        animator.SetBool("IsGrounded", comportementAI.IsGrounded);
        animator.SetBool("IsDashed", comportementAI.IsDashed);
        animator.SetBool("IsAttacking", comportementAI.IsAttacking);
        animator.SetBool("IsMoving", Mathf.Abs(rb.velocity.x) > 0.5f);
        animator.SetBool("IsFalling", rb.velocity.y < -1f && !comportementAI.IsGrounded);
        animator.SetBool("IsAssassinate", assassination.IsAssassinate);
    }
    private void Suprised()
    {
        if (assassination.IsAssassinate)
        {
            comportementAI.CurrentDirection -= comportementAI.CurrentDirection * 2;
        }
    }

    private void LaunchAttack()
    {
        actionAI.Attack();
    }

    private void LaunchAudio(int i)
    {
        switch (i)
        {
            case 0:
                audioManager.PlayClip(audioSource, ac_list_stepFoot);
                break;
            case 1:
                audioManager.PlayClip(audioSource, ac_drawKatana);
                break;
            case 2:
                audioManager.PlayClip(audioSource, ac_shealthKatana);
                break;
            case 3:
                audioManager.PlayClip(audioSource, ac_dashAttack);
                break;
        }
    }
}
