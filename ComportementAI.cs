using UnityEngine;
using System.Collections;

public class ComportementAI : MonoBehaviour
{
    [Header("MoveStat")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFollowingTarget;
    [SerializeField] private bool isAlerted;
    [SerializeField] private bool isStopped;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool searchNoiseOrigin;
    [SerializeField] private bool sawPlayer;

    [Header("State AI For Other Script")]
    [SerializeField] private int currentDirection;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isDashed;
    
    [Header("Stats")]
    [SerializeField, Range(1f, 100f)] private float speedWalk = 75f;
    [SerializeField, Range(1f, 400f)] private float speedFollowingTarget = 100f;
    [SerializeField, Range(1f, 100f)] private float speedAlerted = 55f;
    [SerializeField, Range(1f, 100f)] private float forceJump = 10f;

    [Header("Condition Raycast")]
    [SerializeField] private bool canMoveToward;
    [SerializeField] private bool canJump;
    [SerializeField] private bool canFall;

    [Header("RayCast")]
    RaycastHit2D wallDetection;
    RaycastHit2D jumpPossibleDetection;
    RaycastHit2D holeDetection;

    [Header("Divers")]
    [SerializeField] private LayerMask layerGround;
    [SerializeField] private IEnumerator stopMove;

    [Header("Other Script Variable")]
    private Vector2 dirToTarget;

    [Header("Component")]
    private ActionAI actionAI;
    private HealthEnemy healthEnemy;
    private FieldOfView fieldOfView;
    private Rigidbody2D rb;
    private BoxCollider2D myCollider;
    private Assassination assassination;


    // Properties
    public int CurrentDirection { get { return currentDirection; } set { currentDirection = value; } }
    public bool IsDashed { get { return isDashed; } set { isDashed = value; } }
    public bool IsStopped { get { return isStopped; } set { isStopped = value; } }
    public bool SearchNoiseOrigin { get { return searchNoiseOrigin; } set { searchNoiseOrigin = value; } }
    public bool IsAlerted { get { return isAlerted; } set { isAlerted = value; } }
    public bool IsFollowingTarget { get { return isFollowingTarget; } set { isFollowingTarget = value; } }
    
    // Animation Properties

    public bool IsGrounded => isGrounded;
    public bool IsAttacking => isAttacking;
    public bool IsJumping => isJumping;

    private void Awake()
    {
        actionAI = GetComponent<ActionAI>();
        healthEnemy = GetComponent<HealthEnemy>();
        fieldOfView = GetComponentInChildren<FieldOfView>();
        rb = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        assassination = GetComponent<Assassination>();
    }
    private void Start()
    {
        WatchAnywhere();
    }
    private void FixedUpdate()
    {
        FlipDirection();
        GroundCheck();
        VariableInitialisedFromOtherScripts();
        RaycastToDetect();
        Jump();
        ManageFunction();
    }

    private void VariableInitialisedFromOtherScripts()
    {
        //distTarget = fieldOfView.FindVisibleTarget();
        dirToTarget = fieldOfView.DirToTarget;
        dirToTarget.x = dirToTarget.x > 0f ? 1 : -1;
        if (!isGrounded)
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y); 
        }
    }

    private void FlipDirection()
    {
        transform.localScale = new(currentDirection, 1f, 1f);
    }
    private void ManageFunction()
    {
        if (!healthEnemy.IsHurted && !assassination.IsAssassinate)
        {
            actionAI.AreaAttack();
            Move();
            MoveToTarget();
            ReturnToContinue();
            if (fieldOfView.TargetInView)
            {
                actionAI.LaunchAttack(fieldOfView.DistToTarget, ref isAttacking, ref dirToTarget);
            } else
            {
                isFollowingTarget = false;
                isDashed = false;
                isAttacking = false;
                actionAI.canAttack = true;
                actionAI.canDash = true;
            }
        }
    }
    private void Move()
    {
        if (!isStopped && !isFollowingTarget && !healthEnemy.IsHurted && !searchNoiseOrigin && !isAlerted)
        {
            rb.velocity = new(currentDirection * speedWalk * Time.deltaTime, rb.velocity.y);
        } else if (isAlerted)
        {
            rb.velocity = new(currentDirection * speedAlerted * Time.deltaTime, rb.velocity.y);
        }
    }

    private void MoveToTarget()
    {
        if (fieldOfView.TargetInView && !healthEnemy.IsHurted)
        {
            sawPlayer = true;
            if (!isDashed && !isAttacking)
            {
                isFollowingTarget = true;
                isAlerted = false;
                if (fieldOfView.DistToTarget > actionAI.attackRangeMax)
                {
                    rb.velocity = new(dirToTarget.x * speedFollowingTarget * Time.deltaTime, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new(0f, rb.velocity.y);
                }
                currentDirection = dirToTarget.x > 0 ? 1 : -1;
            }
        }
        else
        {
            isFollowingTarget = false;
        }
    }

    private void Jump()
    {
        if(rb.velocity.y > 20f)
        {
            rb.velocity = new(rb.velocity.x, 20f);
        }
        if (canJump && isGrounded)
        {
            isJumping = true;
            rb.AddForce(new Vector2(rb.velocity.x, forceJump), ForceMode2D.Impulse);
        }
        else if (rb.velocity.y < 0)
        {
            isJumping = false;
        }
        else return;
    }

    private void ReturnToContinue()
    {
        if (!fieldOfView.TargetInView && !isStopped)
        {
            if ((!canMoveToward || !canFall) && !canJump && isGrounded)
            {
                isStopped = true;
                StartCoroutine(StopMoveAndBack());
            }
        }
    }
    public void StopMoving(bool isDo)
    {
        if (isDo)
        {
            rb.velocity = new(0f, rb.velocity.y);
        }
        else
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y);
        }
    }

    public IEnumerator StopMoveAndBack()
    {
        float randomDelay = Random.Range(0.1f, 5f);
        StopMoving(isStopped);
        currentDirection -= currentDirection * 2;
        yield return new WaitForSeconds(randomDelay);
        isStopped = false;
        
    }

    public void GoToNoise(Vector2 targetDir)
    {
        if (searchNoiseOrigin && !fieldOfView.TargetInView)
        {
            currentDirection = targetDir.x > 0 ? 1 : -1;
            rb.velocity = new(currentDirection * speedAlerted * Time.deltaTime, rb.velocity.y);
        }
    }
    public void WatchAnywhere()
    {
        int randomOne = Random.Range(-1, 1);
        currentDirection = randomOne == 0 ? 1 : randomOne;
    }

    private void SoundAlarm()
    {
        if(sawPlayer && !isFollowingTarget)
        {
            //Va faire sonner l'alarme
        }
    }
    /// <summary>
    /// Raycast pour détecter le sol, les murs infranchissables et sautables
    /// </summary>
    private void RaycastToDetect()
    {
        float heightJumpMax = 2f;
        float distanceWallTowardMax = 2f;

        wallDetection = Physics2D.Raycast(new Vector2(transform.position.x + currentDirection / 2, transform.position.y), new Vector2(currentDirection, 0f), Mathf.Infinity, layerGround);
        holeDetection = Physics2D.Raycast(new Vector2(transform.position.x + currentDirection, transform.position.y - myCollider.size.y / 2 + myCollider.offset.y), new Vector2(0f, -1f), Mathf.Infinity, layerGround);
        jumpPossibleDetection = Physics2D.Raycast(new Vector2(transform.position.x + currentDirection / 2, transform.position.y + heightJumpMax), new Vector2(currentDirection, 0f), Mathf.Infinity, layerGround);

        canMoveToward = wallDetection.distance > distanceWallTowardMax || wallDetection == false;
        canFall= holeDetection.distance <= heightJumpMax && holeDetection == true;
        canJump = !canMoveToward && jumpPossibleDetection.distance >= distanceWallTowardMax;

        Debug.DrawRay(new Vector2(transform.position.x + currentDirection / 2, transform.position.y), new Vector2(currentDirection, 0f), Color.red);
        Debug.DrawRay(new Vector2(transform.position.x + currentDirection, transform.position.y - myCollider.size.y / 2), new Vector2(0f, - 1f), Color.blue);
        Debug.DrawRay(new Vector2(transform.position.x + currentDirection / 2, transform.position.y + heightJumpMax), new Vector2(currentDirection, 0f), Color.green);
    }
    // Zone pour détecter si l'on touche le sol
    private void GroundCheck()
    {
        Vector2 groundPosition = new(transform.position.x - myCollider.offset.x * transform.localScale.x, transform.position.y - myCollider.size.y / 2f - myCollider.edgeRadius + myCollider.offset.y);
        float widthCheck = myCollider.size.x - 0.2f + myCollider.edgeRadius;
        float heightCheck = myCollider.size.y / 10f;
        isGrounded = Physics2D.OverlapBox(groundPosition, new Vector2(widthCheck, heightCheck), 0f, layerGround);
    }

    /*
    private void OnDrawGizmos()
    {
        Vector2 groundPosition = new (transform.position.x + myCollider.offset.x * transform.localScale.x, transform.position.y - myCollider.size.y / 2f - myCollider.edgeRadius + myCollider.offset.y);
        float widthCheck = myCollider.size.x - 0.2f + myCollider.edgeRadius;
        float heightCheck = myCollider.size.y / 10f;
        Gizmos.color = new Color32(0, 255, 255, 90);
        Gizmos.DrawCube(groundPosition, new Vector2(widthCheck, heightCheck));
    }*/
}
