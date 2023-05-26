using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

[RequireComponent(typeof(Animator))]
public class Animation_Player : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioManager audioManager;
    [SerializeField]  AudioSource audioSource;
    [SerializeField] List<AudioClip> ac_list_stepFoot;
    [SerializeField] List<AudioClip> ac_list_punch;
    [SerializeField] AudioClip ac_simpleJump;
    [SerializeField] AudioClip ac_doubleJump;
    [SerializeField] AudioClip ac_launchKunai;
    [SerializeField] AudioClip ac_kunaiInFlesh;
    [Header("Components")]
    [SerializeField] AimAndShoot sight;
    [SerializeField] ControlMovePlayer a_Player;
    [SerializeField] Inventory inventory;
    [SerializeField] Animator animator;
    [SerializeField] SpriteResolver resolver;
    [SerializeField] MeleeAttack meleeAttack;
    [SerializeField] TrailRenderer bandTrail;

    void Awake()
    {
        a_Player = GetComponentInParent<ControlMovePlayer>();
        inventory = GetComponentInParent<Inventory>();
        meleeAttack = GetComponentInParent<MeleeAttack>();
        audioSource = GetComponentInParent<AudioSource>();
        audioManager = GetComponentInParent<AudioManager>();
        sight = a_Player.GetComponentInChildren<AimAndShoot>();
        bandTrail = GetComponentInChildren<TrailRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new(a_Player.CurrentDirection, 1f, 1f);
        AnimationController();
    }

    private void AnimationController()
    {
        //animator.SetFloat("SpeedMove", a_Player.CurrentSpeed);
        animator.SetBool("IsGrounded", a_Player.IsGrounded);
        animator.SetBool("HangToWall", a_Player.HangToWall);
        animator.SetBool("IsJumping", a_Player.IsJumping);
        animator.SetBool("IsInAir", a_Player.IsInAir);
        animator.SetBool("IsFalling", a_Player.IsFalling);
        animator.SetBool("IsWallSliding", a_Player.IsWallSliding);
        animator.SetBool("IsWallRunning", a_Player.IsWallRunning);
        animator.SetBool("IsTowardWall", a_Player.IsTowardWall);
        animator.SetBool("IsTurning", a_Player.IsTurning);
        animator.SetBool("CanJump", a_Player.CanJump);
        animator.SetFloat("VelocityY", Mathf.Abs(a_Player.Rb.velocity.y));
        animator.SetFloat("VelocityX", Mathf.Abs(a_Player.Rb.velocity.x));
        animator.SetFloat("WallRunningSpeed", a_Player.WallRunningSpeed);
        animator.SetBool("LaunchKunai", sight.IsShoot && sight.IsAim && inventory.kunaiCurrent > 0f);
        animator.SetBool("IsAssassinate", meleeAttack.isMurdering);
    }

    public void ChangeSkin(bool isTrue)
    {
        if (isTrue)
        {
            string skinWB = "WithBand";
            resolver.SetCategoryAndLabel(skinWB, resolver.GetLabel());
            resolver.enabled = true;
            bandTrail.enabled = true;
        } else
        {
            string skinWB = "WithoutBand";
            resolver.SetCategoryAndLabel(skinWB, resolver.GetLabel());
            resolver.enabled = false;
            bandTrail.enabled = false;
        }
    }

    // Lancement en Animation Event dans FightHit
    private void LaunchAttack(int actionAttack)
    {
        switch(actionAttack){
            case 1:
                meleeAttack.Attack(1f, 1, 0f);
                break;
            case 2:
                meleeAttack.Attack(2f, 2, 0f);
                break;
            case 3:
                meleeAttack.Attack(3f, 3, 5f);
                break;
            case 4:
                meleeAttack.Attack(1.5f, 100, 0f);
                break;
        }
    }
    
    private void LaunchAudio(int i)
    {
        switch (i)
        {
            case 0:
                audioManager.PlayClip(audioSource, ac_list_stepFoot);
                break;
            case 1:
                audioManager.PlayClip(audioSource, ac_list_punch);
                break;
            case 2:
                audioManager.PlayClip(audioSource, ac_launchKunai);
                break;
            case 3:
                audioManager.PlayClip(audioSource, ac_kunaiInFlesh);
                break;
            case 4:
                if (a_Player.IsInAir)
                {
                    audioManager.PlayClip(audioSource, ac_doubleJump);
                }else
                {
                    audioManager.PlayClip(audioSource, ac_simpleJump);
                }
                break;
        }
    }
}


