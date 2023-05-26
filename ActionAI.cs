using UnityEngine;
using System.Collections;

public class ActionAI : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] public bool canDash = true;
    [SerializeField, Range(0f,10f)] private float dashRangeMin = 5f;
    [SerializeField, Range(10f, 20f)] private float dashRangeMax = 20f;
    [SerializeField, Range(100f, 300f)] private float speedDash = 100f;
    [SerializeField, Range(0f, 20f)] private float timeBeforeDash = 1f;
    [SerializeField, Range(0f, 20f)] private float coolDownDash = 5f;
    

    [Header("Attack")]
    [SerializeField] public bool canAttack = true;
    [SerializeField] private int damage = 1;
    [SerializeField, Range(1f, 5f)] public float attackRangeMax = 2f;
    [SerializeField] private float attackFrequency = 2f;
    [SerializeField, Range(0f, 20f)] private float attackFrequencyMax;


    [SerializeField] private ComportementAI comportementAI;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private LayerMask layerPlayer;
    [SerializeField] private IEnumerator delayToDash;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        comportementAI = GetComponent<ComportementAI>();
    }

    // Fonction globale permettant de faire dasher ou attaquer 
    public void LaunchAttack(float distToTarget, ref bool isAttacking, ref Vector2 dirToTarget)
    {
        if (comportementAI.IsFollowingTarget)
        {
            HaveNeedToAttack(ref isAttacking);
            if (comportementAI.IsDashed && canAttack)
            {
                Attack();
            }
            if (RangeToDash(dashRangeMin, dashRangeMax, distToTarget))
            {
                delayToDash = DelayToDash(dirToTarget);
                if (delayToDash != null)
                {
                    StartCoroutine(delayToDash);
                }
                else
                {
                    StopCoroutine(delayToDash);
                }
            }
        }
    }
    // Verifie si la cible est à portee
    private bool RangeToDash(float rangeMin, float rangeMax, float distToTarget)
    {
        if (distToTarget >= rangeMin && distToTarget <= rangeMax)
        {
            return true;
        }
        return false;
    }
    // Dash sur la cible
    private IEnumerator Dashing(Vector2 dirToTarget)
    {
        rb.velocity = new(dirToTarget.x * speedDash, dirToTarget.y);
        comportementAI.CurrentDirection = dirToTarget.x > 0 ? 1 : -1;
        yield return new WaitForSeconds(0.2f);
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.1f);
        comportementAI.IsDashed = false;
        rb.velocity = new(rb.velocity.x, rb.velocity.y);
        StartCoroutine(DelayBeforeCanDash(coolDownDash));
    }
    //Delai entre chaque Dash
    private IEnumerator DelayBeforeCanDash(float delay)
    {
        if (!canDash)
        {
            yield return new WaitForSeconds(delay);
            canDash = true;
        }
        else yield return null;
    }
    // Se fige sur place quelques secondes avant de dasher
    private IEnumerator DelayToDash(Vector2 dirToTarget)
    {              
        if (!comportementAI.IsDashed && canDash && canAttack && comportementAI.IsFollowingTarget)
        {
            canDash = false;
            rb.velocity = Vector2.zero;
            comportementAI.IsDashed = true;
            yield return new WaitForSeconds(timeBeforeDash);
            if (comportementAI.IsDashed)
            {
                StartCoroutine(Dashing(dirToTarget));
            }
        } 
    }

    // Methode Overlap
    public void AreaAttack()
    {
        Vector2 startBox = new(transform.position.x + comportementAI.CurrentDirection * attackRangeMax / 2f , transform.position.y);
        Vector2 sizeBox = new(attackRangeMax, 1.5f);
        hitBox = Physics2D.OverlapBox(startBox, sizeBox, 0f, layerPlayer);
    }
    // Tape si le joueur entre dans la zone et qu'il peut attaquer
    private void HaveNeedToAttack(ref bool isAttacking)
    {
        if (hitBox != null && canAttack)
        {
            comportementAI.StopMoving(isAttacking);
            isAttacking = true;
            // Lance Animation coup de sabre, Event AttackAnimation
        }
        else
        {
            ReloadAttack();
            isAttacking = false;
        }
    }

    // Se lance durant l'animation Attack
    public void Attack()
    {
        if (hitBox != null && hitBox.TryGetComponent(out HealthLifeAndDeath healthLifeAndDeath))
        {
            healthLifeAndDeath.GetsHurt(damage, comportementAI.CurrentDirection);
            canAttack = false;
        }
    }
    // Cooldown entre chaque attaque
    private void ReloadAttack()
    {
        if (attackFrequency >= 0f && !canAttack && comportementAI.IsFollowingTarget)
        {
            attackFrequency -= Time.fixedDeltaTime;

        } else
        {
            canAttack = true;
            attackFrequency = attackFrequencyMax;
        }
    }
    /*
    private void OnDrawGizmos()
    {
        Vector2 startBox = new(transform.position.x + comportementAI.CurrentDirection * attackRangeMax / 2f, transform.position.y);
        Vector2 sizeBox = new(attackRangeMax, 1f);
        Gizmos.color = new Color32(0, 255, 255, 90);
        Gizmos.DrawCube(startBox, sizeBox);
    }*/
}
