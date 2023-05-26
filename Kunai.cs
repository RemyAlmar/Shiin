using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Kunai : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip ac_impact;
    [SerializeField] private AudioClip ac_hit;
    [SerializeField] private AudioClip ac_speedTrail;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource audioSource;
    [Header("Divers")]
    [SerializeField] private bool hasHit;
    [SerializeField] private float radiusNoise = 10f;
    [SerializeField] private int direction;
    [SerializeField] private Transform myParent;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private ParticleSystem spark;
    [SerializeField] private ParticleSystem repulsiveField;
    private int damage = 1;
    private Vector2 refVelocity;
    private Rigidbody2D rb;

    private GameObject player;
    public Rigidbody2D Rb => rb;
    private Collider2D collideKunai;
    private HealthEnemy healthEnemy;

    public bool returnToTarget;
    void Awake()
    {
        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        collideKunai = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        audioManager = GetComponent<AudioManager>();
    }
    private void Update()
    {
        CheckIfIsChildren();
        ModifyAngle();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!returnToTarget)
        {
            if (!collision.gameObject.CompareTag("Player") && collision.gameObject != null)
            {
                myParent = collision.gameObject.transform;

                FreezeKunai();
                if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platforme"))
                {
                    gameObject.tag = "CollectibleWeapon";
                    transform.SetParent(myParent);
                    audioSource.Stop();
                    audioManager.PlayClip(audioSource, ac_impact);
                    spark.Play();
                    NoiseImpact(radiusNoise);
                }
                else if (collision.gameObject.CompareTag("Enemy")/* || collision.gameObject.CompareTag("Objects")*/)
                {
                    healthEnemy = collision.gameObject.GetComponent<HealthEnemy>();
                    if (healthEnemy.Health > 0f)
                    {
                        float pushForceX = rb.velocity.x * damage;
                        Vector2 pushForce = new(pushForceX, damage);
                        transform.SetParent(myParent);
                        healthEnemy.TakeKunaiInBody();
                        StartCoroutine(healthEnemy.TakeDamage(damage, 0.1f, direction, pushForce));
                    }
                    audioSource.Stop();
                    audioManager.PlayClip(audioSource, ac_hit);
                }
            }
        }
    }
    private void FreezeKunai()
    {
        if (!returnToTarget)
        {
            hasHit = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            collideKunai.isTrigger = true;
            repulsiveField.Play();
        }
    }
    private void NoiseImpact(float radiusNoise)
    {
        if (!returnToTarget)
        {
            //Creer une bulle de detection de collider 2d
            Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, radiusNoise);
            foreach (Collider2D collider2D in colliderArray)
            {// Si l'objet au collider 2D possède un script DetectingNoise alors démarre une coroutine pour lancer la fonction ComeNoise
                if (collider2D.TryGetComponent(out DetectingNoise detectNoise))
                {
                    detectNoise.StartCoroutine(detectNoise.ComeNoise(transform.position));
                }
            }
        }
    }

    private void ModifyAngle()
    {
        //Modifie l'angle de l'objet selon sa trajectoire
        if (hasHit == false)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            direction = direction == 0 && rb.velocity.x > 0 ? 1 : -1;
            audioManager.PlayClip(audioSource, ac_speedTrail, true);
        }
    }
    private void DefreezeKunai()
    {
        if (!returnToTarget)
        {
            collideKunai.isTrigger = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
    private void CheckIfIsChildren()
    {
        if (!returnToTarget)
        {
            if (transform.parent != null)
            {
                if (transform.parent.CompareTag("Enemy"))
                {
                    if (healthEnemy.Health <= 0f)
                    {
                        transform.SetParent(null);
                        hasHit = false;
                        gameObject.tag = "CollectibleWeapon";
                    }
                }
            }
            else if ((myParent == null && !hasHit) || !myParent.CompareTag("Ground"))
            {
                DefreezeKunai();
            }
            else
            {
                DefreezeKunai();
            }
        }
    }

    public void GoToTarget(Transform target, float speedRecupMax, float smoothTime)
    {
        if (returnToTarget)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            Collider2D trigger = Physics2D.OverlapCircle(transform.position, 3f, playerMask);
            if (trigger)
            {
                collideKunai.isTrigger = true;
            }
            else { 
                collideKunai.isTrigger = false; 
            }
            transform.SetParent(null);
            rb.bodyType = RigidbodyType2D.Dynamic;
            hasHit = false;
            Vector2 targetVelocity = new (direction.x * speedRecupMax, direction.y * speedRecupMax);
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref refVelocity, smoothTime);
            repulsiveField.Stop();
        }
        else 
        { 
            rb.velocity = new(rb.velocity.x, rb.velocity.y);
        }
    }

    public void ReturnToPLayer()
    {
        float posX = player.transform.position.x;
        float posY = player.transform.position.y + 2f;
        rb.velocity = new(0f, 0f);
        transform.position = new(posX, posY);
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color32(0, 255, 255, 90);
       // Gizmos.DrawSphere(transform.position, radiusNoise);
        Gizmos.color = new Color32(0, 255, 255, 80);
        Gizmos.DrawSphere(transform.position, 3f);
    }
}
