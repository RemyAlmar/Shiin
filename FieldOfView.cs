using System.Collections;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float detectRadius;
    [Range(0, 360)]
    [SerializeField] private float viewAngle;
    [SerializeField] private Vector2 enemyViewDirection;
    [SerializeField] private bool targetInView;
    [SerializeField] private Vector2 dirToTarget;
    [SerializeField] private float distToTarget;
    public bool TargetInView => targetInView;
    public float DistToTarget => distToTarget;
    public Vector2 DirToTarget => dirToTarget;
    public Vector2 TargetPos => targetPos;
    private Vector2 targetPos;

    //Properties
    public float DetectRadius => detectRadius;
    public float ViewAngle => viewAngle;

    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private ComportementAI comportementAI;
    [SerializeField] private HealthEnemy healthEnemy;
    private void Awake()
    {
        comportementAI = GetComponentInParent<ComportementAI>();
        healthEnemy = GetComponentInParent<HealthEnemy>();
    }

    private void Start()
    {
        enemyViewDirection = new(comportementAI.CurrentDirection, 0f);
    }
    private void Update()
    {
        FindVisibleTarget();
    }

    /// <summary>
    ///     Creation d'un rayon de detection, si le joueur est dedans et dans l'angle de vue on envoie un raycast et l'angle suis le joueur, si il touche la cible alors il la vois sinon ne la vois pas
    /// </summary>
    public float FindVisibleTarget()
    {
        Collider2D targetInViewRadius = Physics2D.OverlapCircle(transform.position, detectRadius, targetMask);

        if (targetInViewRadius != null /*&& targetInViewRadius.TryGetComponent(out ControlMovePlayer cmp)*/)
        {
            Transform target = targetInViewRadius.transform;
            dirToTarget = (target.position - transform.position).normalized;
            targetPos = target.position;

            if (Vector2.Angle(enemyViewDirection, dirToTarget) < viewAngle / 2)
            {
                distToTarget = Vector2.Distance(transform.position, target.position);

                if (!Physics2D.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask) && healthEnemy.Health > 0)
                {
                    enemyViewDirection = new(comportementAI.CurrentDirection * transform.eulerAngles.x, 0f);
                    Debug.DrawLine(transform.position, target.position, Color.red);
                    targetInView = true;
                    detectRadius = 50f;
                    viewAngle = 90f;
                    transform.right = target.position - transform.position;
                }
                else
                {
                    DontViewTarget();
                }
            }
            else
            {
                DontViewTarget();
            }
        }
        else
        {
            DontViewTarget();
        }
        return distToTarget;
    }

    private void DontViewTarget()
    {
        if (!comportementAI.IsAlerted) 
        {
            targetInView = false;
            detectRadius = 5f;
            viewAngle = 90f;
            enemyViewDirection = new(comportementAI.CurrentDirection, 0f);
        } else
        {
            StartCoroutine(AlertedState());
        }
    }

    private IEnumerator AlertedState()
    {
        float alertedTime = Random.Range(5f, 15f);
        float alertedDetectRadius = 20f;
        float alertedViewAngle = 200f;
        if (comportementAI.IsAlerted)
        {
            detectRadius = alertedDetectRadius;
            viewAngle = alertedViewAngle;
            enemyViewDirection = new(comportementAI.CurrentDirection, 0f);
        }
        yield return new WaitForSeconds(alertedTime);
        comportementAI.IsAlerted = false;
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f);
    }
}
