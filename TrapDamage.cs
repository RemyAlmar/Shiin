using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    private int damage = 1;
    [SerializeField] private bool isActivated;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Activation()
    {
        isActivated = !isActivated;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CanHurted(collision);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Active une animation qui a un moment lance Activation()
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            if (!isActivated)
            {
                animator.SetTrigger("TriggerIt");
            }
        }
    }
    private void CanHurted(Collision2D collision)
    {
        if (isActivated)
        {
            Vector2 dir = (collision.transform.position - transform.position).normalized;
            dir.x = dir.x > 0 ? 1 : -1; 
            if(collision.gameObject.TryGetComponent(out HealthLifeAndDeath hld))
            {
                hld.GetsHurt(damage, (int)dir.x);
            }
            if(collision.gameObject.TryGetComponent(out HealthEnemy he))
            {
                StartCoroutine(he.TakeDamage(20, 1f, (int)dir.x, dir));
            }
        }
    }
}
