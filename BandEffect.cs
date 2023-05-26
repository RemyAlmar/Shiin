using UnityEngine;

public class BandEffect : MonoBehaviour
{
    private float timeCurrent;
    private bool appears = true;
    private bool canGet;
    [SerializeField, Range(0.2f, 2f)] private float timeMax;
    [SerializeField, Range(1f, 20f)] private float appearForce;
    [SerializeField] private LayerMask layerPlayer;

    // Components
    private Collider2D getBox;
    private BoxCollider2D box;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        Pop();
    }
    // Si le bandeau apparait, il se fait projetter, s'il est déjà apparu alors le timer se met en route
    private void Pop()
    {
        if(appears)
        {
            PopDirection();
        }else if(LapsBeforeCanGetting())
        {
            GetBand();
        }
    }    
    // Projection du bandeau aléatoirement dans les airs
    private void PopDirection()
    {
        timeCurrent = timeMax;
        rb.gravityScale = 2f;
        float dirX = Random.Range(-1f, 1f);
        float dirY = Random.Range(1f, 2f);
        Vector2 dirRandom = new(dirX, dirY);
        rb.AddForce(dirRandom * appearForce, ForceMode2D.Impulse);
        appears = false;
    }

    // Temps avant de pouvoir récuperer le bandeau
    private bool LapsBeforeCanGetting()
    {
        if(timeCurrent >= 0f)
        {
            timeCurrent -= Time.deltaTime;
            canGet = false;
        } else
        {
            timeMax = 0f;
            canGet = true;
        }
        return canGet;
    }

    //Si le joueur passe sur le bandeau, le bandeau se détruit
    private void GetBand()
    {
        getBox = Physics2D.OverlapBox(transform.position, box.size, 0f, layerPlayer);
        if(getBox != null)
        {
            if(getBox.gameObject.TryGetComponent(out HealthLifeAndDeath healthLifeAndDeath))
            {
                healthLifeAndDeath.bandUnlock = true;
                healthLifeAndDeath.haveMyBand = true;
                healthLifeAndDeath.isInvincible = true;
                Destroy(gameObject);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            rb.velocity = new(0f, rb.velocity.y);
        }
    }
}
