using System.Collections;
using UnityEngine;

public class HealthLifeAndDeath : MonoBehaviour
{
    [Header("Get Your Audio HERE")]
    [SerializeField] private AudioClip ac_hurted;
    [SerializeField] private AudioClip ac_loseBand;
    private AudioManager audioManager;
    private AudioSource audioSource;
    [Header("Stats")]
    [SerializeField] private int enemyDirection;
    [SerializeField] private Vector2 pushForce;
    [SerializeField] private int healthMax = 1;
    [SerializeField, Range(1f, 5f)] private float invincibleTime = 1f;
    [SerializeField, Range(0.1f, 1f)] private float hurtedTime = 0.5f;
    [SerializeField, Range(1f, 15f)] private float timeBeforeRespawn = 1.5f;
    [SerializeField, Range(1f, 15f)] private float ragdollDieTime = 2f;

    public int health;

    public bool isInvincible;
    public bool haveMyBand;
    public bool bandUnlock;
    [Header("Etat")]
    [SerializeField] private bool isHurted;
    [SerializeField] private bool isDead;
    [SerializeField] private bool ragdollOn;
    [SerializeField] private IEnumerator die;
    public bool IsDead => isDead;
    //[Header("Component")]
    [SerializeField] private GameObject band;
    [SerializeField] private Animator fadeLoadingSystem;
    private GameObject myBand;
    private Respawn respawn;
    private Ragdoll ragdoll;
    private MeleeAttack meleeAttack;
    private Animation_Player animationPlayer;
    private ControlMovePlayer controlMovePlayer;
    private Inventory inventory;
    private AimAndShoot aimAndShoot;
    private Permutation permutation;

    void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();
        controlMovePlayer = GetComponent<ControlMovePlayer>();
        inventory = GetComponent<Inventory>();
        permutation = GetComponent<Permutation>();
        meleeAttack = GetComponent<MeleeAttack>();
        respawn = GetComponent<Respawn>();
        ragdoll = GetComponent<Ragdoll>();
        aimAndShoot = GetComponentInChildren<AimAndShoot>();
        animationPlayer = GetComponentInChildren<Animation_Player>();
    }

    private void Start()
    {
        BackToFullLife();
    }

    // Update is called once per frame
    void Update()
    {
        BandEffect();
        Dead();
        ComponentManager(enemyDirection);
        animationPlayer.ChangeSkin(haveMyBand);
    }

    /// <summary>
    /// Si a son bandeau le perd, sinon se prend des dommages et devient invincible un instant T 
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public void GetsHurt(int damage, int currentDirectionEnemy)
    {
        if (!permutation.cooldownToHit)
        {
            isHurted = true;
            KnockBack(currentDirectionEnemy);
            enemyDirection = currentDirectionEnemy;
            if (!isInvincible && !haveMyBand)
            {
                audioManager.PlayClip(audioSource, ac_hurted);
                health -= damage;
            }
            else
            {
                audioManager.PlayClip(audioSource, ac_loseBand);
                LoseMyBand();
            }
        }
        else
        {
            permutation.PermutationTrigger();
        }
    }
    private void BandEffect()
    {
        if (haveMyBand)
        {
            isInvincible = true;
        }
    }

    private IEnumerator BecomeInvincible()
    {
        if (isInvincible)
        {
            yield return new WaitForSeconds(invincibleTime);
            isInvincible = false;
        }

    }

    private void KnockBack(int currentDirectionEnemy)
    {
        pushForce = new(currentDirectionEnemy * 10f, 10f);
        if (isHurted)
        {
            StartCoroutine(controlMovePlayer.CantMoving(hurtedTime));
            controlMovePlayer.Rb.AddForce(pushForce, ForceMode2D.Impulse);
            isHurted = false;
        }
    }
    // Si a son bandeau, le perd et deviens vulnérable
    private void LoseMyBand()
    {
        if (haveMyBand)
        {
            myBand = Instantiate(band, transform.position, transform.rotation);
            haveMyBand = false;
            StartCoroutine(BecomeInvincible());
        }
    }
    // Si meurs, désactive les composants suivant et active la Coroutine pour respawn, sinon active les composants
    private void ComponentManager(int killerDirection)
    {
        if (isDead)
        {
            StartCoroutine(DieAndRespawn(timeBeforeRespawn, killerDirection)); 
            controlMovePlayer.enabled = false;
            aimAndShoot.enabled = false;
            meleeAttack.enabled = false;
            animationPlayer.enabled = false;
        } else
        {
            meleeAttack.enabled = true;
            aimAndShoot.enabled = true;
            controlMovePlayer.enabled = true;
            animationPlayer.enabled = true;
        }
    }
    // Active le ragdoll, détruit le bandeau et fais réapparaitre le joueur au dernier checkpoint avec tous ses pv et son bandeau
    private IEnumerator DieAndRespawn(float delay, int killerDirection)
    {
        //Faire un ralentissement du temps 
        if (isDead && ragdollOn)
        {
            ragdollOn = false;
            Destroy(myBand);
            StartCoroutine(ragdoll.RagdollOn(ragdollDieTime, killerDirection));
            //yield return new WaitForSeconds(delay);
            yield return new WaitForSeconds(delay);
            fadeLoadingSystem.SetTrigger("FadeIn");
            yield return new WaitForSeconds(delay);

            respawn.RespawnToLastCheckPoint();
            BackToFullLife();
        }
        else yield return null;
    } 

    /// <summary>
    /// Met la vie à zero et peut activer le ragdoll, reinitialise le time scale et desactive la visee
    /// </summary>
    public void Dead()
    {
        if (health <= 0 && isDead == false)
        {
            health = 0;
            isDead = true;
            ragdollOn = true;
            //aimAndShoot.DestroyAllKunai();
            aimAndShoot.ResetTimeScale();
            aimAndShoot.GFX.enabled = false;
        }
    }
    /// <summary>
    /// Reset la vie et le nombre de kunai et remet le bandeau si il est unlock
    /// </summary>
    public void BackToFullLife()
    {
        isDead = false;
        inventory.FullKunai();
        health = healthMax;
        controlMovePlayer.Rb.velocity = Vector2.zero;
        if (bandUnlock)
        {
            haveMyBand = true;
            isInvincible = true;
        }
        else
        {
            haveMyBand = false;
            isInvincible = false;
        }
    }
}
