using UnityEngine;

public class TileTrapDamage : MonoBehaviour
{
    private int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        InflictDamage(collision);
    }
    private void InflictDamage(Collision2D collision)
    {
        Vector2 dir = (collision.transform.position - transform.position).normalized;
        dir.x = dir.x > 0 ? 1 : -1;
        if (collision.gameObject.TryGetComponent(out HealthLifeAndDeath hld))
        {
            hld.GetsHurt(damage, (int)dir.x);
        }
        if (collision.gameObject.TryGetComponent(out HealthEnemy he))
        {
            StartCoroutine(he.TakeDamage(20, 1f, (int)dir.x, dir));
        }
    }
}
