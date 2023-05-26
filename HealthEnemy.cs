using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private List<AudioClip> ac_list_hurted;
    [SerializeField] private AudioClip ac_hurtedByKnife;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioManager audioManager;
    [Header("Parametre")]
    [SerializeField] private int health = 2;
    [SerializeField] private float timeToRagdoll;
    [SerializeField] private float timeBeforeDestruction = 1f;
    public int Health => health;
    [SerializeField] private int kunaiInBody;
    [SerializeField] private int kunaiRendu;
    [SerializeField] private Vector2 spawnPos;

    private bool isHurted;

    //Properties
    public bool IsHurted => isHurted;

    private ComportementAI comportementAI;
    private Assassination assassination;
    private DetectingNoise detectingNoise;
    private FieldOfView fieldOfView;
    private Rigidbody2D rb;
    private Ragdoll ragdoll;
    private AmbiantMusicManager Amb;
    private BoxCollider2D myCollider;
    // private Transform[] childTransforms;


    private void Awake()
    {
        comportementAI = GetComponent<ComportementAI>();
        myCollider = GetComponent<BoxCollider2D>();
        assassination = GetComponent<Assassination>();
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();
        detectingNoise = GetComponent<DetectingNoise>();
        rb = GetComponent<Rigidbody2D>();
        ragdoll = GetComponent<Ragdoll>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
        Amb = GameObject.FindWithTag("Player").GetComponent<AmbiantMusicManager>();
    }
    private void Start()
    {
        spawnPos = transform.position;
    }
    private void Update()
    {
        VerifyListFightMusic();
    }
    /// <summary>
    /// Si a toujours de la vie et ne se fait pas assassiner, se prend damage et PushBack, devient alerte.
    /// <br></br>Si meurs après ca ne suis plus la cible et LaunchFightMusic() et Die()
    /// <br></br>Sinon après le delai se retourne vers le joueur
    /// </summary>
    /// <param name="damage"> Dommage du joueur ou autre</param>
    /// <param name="delay"> Temps pour la coroutine (temps de PushBack)</param>
    /// <param name="killerDirection"> Direction de celui qui inflige les dommages</param>
    /// <param name="forceDirX"></param>
    /// <returns></returns>
    public IEnumerator TakeDamage(int damage, float delay, int killerDirection, Vector2 forceDirX)
    {
        if(health > 0)
        {
            isHurted = true;
            if (!assassination.IsAssassinate)
            {
                audioManager.PlayClip(audioSource, ac_list_hurted);
            }
            PushBack(forceDirX);
            health -= damage;
            comportementAI.IsAlerted = true;
        }
        if(health <= 0)
        {
            health = 0;
            comportementAI.IsFollowingTarget = false;
            VerifyListFightMusic();
            Die(killerDirection);
        }
        yield return new WaitForSeconds(delay);
        isHurted = false;
        comportementAI.CurrentDirection = -killerDirection;
    }

    public void TakeKunaiInBody()
    {
        kunaiInBody++;
    }

    public int TakeOutKunaiInBody(int damage)
    {
        kunaiRendu = 0;
        for (int i = 0; i < damage; i++)
        {
            if(kunaiInBody > 0)
            {
                kunaiInBody--;
                kunaiRendu++;
                for(int child = transform.childCount -1; child > kunaiInBody; child--)
                {
                    if(transform.GetChild(child).gameObject.CompareTag("ThrowWeapon"))
                    {
                        Destroy(transform.GetChild(child).gameObject);
                    }
                }
            }
        }
        return kunaiRendu;
    }

    // Se fais expulser à l'opposé de l'adversaire
    private void PushBack(Vector2 forceDirX)
    {
        if (isHurted)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(forceDirX, ForceMode2D.Impulse);
        }
    }
    public void Die(int killerDirection)
    {
        StopAllCoroutines();
        comportementAI.enabled = false;
        detectingNoise.enabled = false;
        fieldOfView.enabled = false;
        StartCoroutine(ragdoll.RagdollOn(timeToRagdoll, killerDirection));
        StartCoroutine(AutoDestroy(timeToRagdoll));
    }
    /// <summary>
    /// Si est vivant et suis une cible et n'est pas dans la liste s'ajoute dans la liste
    /// <br></br>Sinon si est dans la liste mais ne suis plus de cible ou est mort se retire de la liste 
    /// </summary>
    private void VerifyListFightMusic()
    {
        if (health > 0)
        {
            if (comportementAI.IsFollowingTarget && !Amb.enemies.Contains(myCollider))
            {
                Amb.enemies.Add(myCollider);
            }
            else if (Amb.enemies.Contains(myCollider))
            {
                if (!comportementAI.IsFollowingTarget)
                {
                    Amb.enemies.Remove(myCollider);
                }
            }
        }
        else
        {
            if (Amb.enemies.Contains(myCollider))
            {
                Amb.enemies.Remove(myCollider);
            }
        }
    }
    private IEnumerator AutoDestroy(float time)
    {
        float delay = time + timeBeforeDestruction;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void Respawn()
    {
        transform.position = spawnPos;
    }
}
