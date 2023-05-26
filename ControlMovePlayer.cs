using System.Collections;
using UnityEngine;

public class ControlMovePlayer : MonoBehaviour

{
    [Header("Running")]
    [SerializeField] private float moveHorizontal;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float smoothTime;
    [SerializeField] private float jumpingTime;

    [Header("Jump")]
    [SerializeField] private bool jumpButton;
    [SerializeField] private bool jumpingIsRequired;
    [SerializeField, Range(10f, 50f)] private float jumpForce = 15f;
    private int jumpAvailable;
    private int jumpMax = 1;
    private bool canJump;

    [Header("JumpForce")]
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private Vector2 wallJumpForceRun;
    [SerializeField] private Vector2 wallJumpForceFall;
    [SerializeField] private Vector2 wallJumpForceToSameWall;

    [Header("ActionOnWall")]
    [SerializeField] private float wallSlideSpeed = 0.5f;
    [SerializeField] private float wallRunningSpeed;
    [SerializeField] private Vector2 wallHopDirection;
    [SerializeField, Range(0.1f, 2f)] private float timeToDropMax;
    [SerializeField] private float timeToDrop;

    [Header("Changement Direction")]
    [SerializeField] private int currentDirection = 1;

    //[Header("Gravity")]
    private float currentGravity = 3f;
    private float maxFallSpeed = 25f;

    [Header("CheckerSize")]
    [SerializeField] private float widthCheck = 0.8f;
    [SerializeField] private float heightCheck = 0.1f;

    [Header("Condition")]
    [SerializeField] private bool canWallRunning;
    [SerializeField] private bool isTurning;
    [SerializeField] private bool touchWall;
    [SerializeField] private bool hangToWall;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isInAir;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isWallSliding;
    [SerializeField] private bool isTowardWall;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isWallRunning;
    [SerializeField] private bool onLeftWall;
    [SerializeField] private bool onRightWall;
    public bool canMove = true;

    //Properties 
    public float WallRunningSpeed => wallRunningSpeed;
    public bool IsGrounded => isGrounded;
    public bool IsJumping => isJumping;
    public bool IsTurning => isTurning;
    public bool IsInAir => isInAir;
    public bool IsFalling => isFalling;
    public bool IsWallSliding => isWallSliding;
    public bool IsWallRunning => isWallRunning;
    public bool HangToWall => hangToWall;
    public bool IsTowardWall => isTowardWall;
    public int CurrentDirection => currentDirection;
    //public float CurrentSpeed => currentSpeed;
    public bool CanJump => canJump;
    public Rigidbody2D Rb => rb;

    //[Header("Component")]
    private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer = 8;
    [SerializeField] private LayerMask multipleLayer;

    private Collider2D groundCheckArea;
    private Collider2D leftWallCheckArea;
    private Collider2D rightWallCheckArea;
    private BoxCollider2D myBoxCollider;
    private MeleeAttack meleeAttack;
    //[SerializeField] private CameraOffset camOff; //Cinemachine

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        meleeAttack = GetComponent<MeleeAttack>();
    }


    void Update()
    {

        if (!PauseMenu.GameIsPaused)
        {
            if (canMove)
            {
                InputManager();
                ColliderEvent();
                DropWall();
            }
        }
    }
    private void FixedUpdate()
    {
        if(!PauseMenu.GameIsPaused)
        {
            if (canMove)
            {
                CheckCollider();
                HorizontalMovement();
                WallSliding();
                GravityJump();
                WallRunning();     
                Jump();
                WallJump();
            }
        }
    }

    private void InputManager()
    {
        
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        jumpButton = Input.GetButtonDown("Jump");
        currentDirection = moveHorizontal > 0 ? 1 : moveHorizontal < 0 ? -1 : currentDirection;
        isTurning = rb.velocity.x > 0.1f && currentDirection < 0 || rb.velocity.x < -0.1f && currentDirection > 0;
        if (jumpButton)
        {
            jumpingIsRequired = true;
        }
        float jumpingTimeMax = 0.1f;
        if (jumpingIsRequired && jumpingTime > 0)
        {
            jumpingTime -= Time.deltaTime;
        }else
        {
            jumpingTime = jumpingTimeMax;
            jumpingIsRequired = false;
        }
    }
    /*
     * A utiliser avec Cinemachine 
    private void UpdateCam()
    {
        camOff?.SetOffset(currentDirection);
    }*/

    private void HorizontalMovement()
    {
        if (!meleeAttack.isMurdering)
        {
            if(moveHorizontal != 0f)
            {
                Vector2 target = new(currentDirection * maxSpeed, rb.velocity.y);
                rb.velocity = Vector2.Lerp(rb.velocity, target, smoothTime * Time.deltaTime);
            } else if(moveHorizontal == 0 || isWallRunning)
            {
                Vector2 target = new(0f, rb.velocity.y);
                rb.velocity = Vector2.Lerp(rb.velocity, target, smoothTime * Time.deltaTime);
                if(Mathf.Abs(rb.velocity.x) <= 2f)
                {
                    rb.velocity = target;
                }
            }
        }else if (meleeAttack.isMurdering)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public IEnumerator CantMoving(float delay)
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(delay);
        canMove = true;
    }
    //Fonction de saut
    private void Jump()
    {
        canJump = jumpAvailable > 0;
        if (jumpingIsRequired && (!isWallSliding || !hangToWall))
        {
            isJumping = true;
            isFalling = false;
            if (isGrounded) // Saut depuis le sol
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(new Vector2(moveHorizontal, jumpForce), ForceMode2D.Impulse);
                jumpingIsRequired = false;
            }
            else if(canJump && isInAir) // Saut depuis partout sauf depuis le sol tant qu'un saut est disponible 
            {
                rb.velocity = new (rb.velocity.x, 0f);
                rb.AddForce(new (moveHorizontal, jumpForce), ForceMode2D.Impulse);
                jumpAvailable--;
                jumpAvailable = jumpAvailable <= 0 ? 0 : jumpAvailable;
                jumpingIsRequired = false;
            }
        }
        else
        isJumping = false;
    }

    //fonction Saut de Mur
    private void WallJump()
    {
        if(rb.velocity.y > 25f)
        {
            rb.velocity = new(rb.velocity.x, 25f);
        }
        // Permet de passer d'un mur à l'autre en WallRunning
        if (jumpingIsRequired && isWallRunning)
        {
            Vector2 forceToAdd = new(wallJumpForceRun.x * -currentDirection, wallJumpForceRun.y);
            Debug.Log("Fauuuux");
            rb.AddForce(forceToAdd, ForceMode2D.Impulse);
            currentDirection = -currentDirection;
            jumpingIsRequired = false;
        }

        if (jumpingIsRequired && !isGrounded && touchWall)
        {
            // Permet de sauter d'un mur à l'opposé en maintenant la touche de déplacement opposé au mur
            if(moveHorizontal != 0 && hangToWall && !isTowardWall)
            {
                Vector2 forceToAdd = new(wallJumpForce.x * currentDirection * 2f, wallJumpForce.y);
                rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                jumpingIsRequired = false;
            }// Permet de faire un petit saut depuis le mur et de revenir dessus en maintenant la touche de déplacement vers le mur
            else if ((isTowardWall || hangToWall) && moveHorizontal != 0 && isWallSliding)
            {
                rb.velocity = new(rb.velocity.x, 1f);
                Vector2 forceToAdd = new(wallJumpForceToSameWall.x * -currentDirection, wallJumpForceToSameWall.y);
                rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                jumpingIsRequired = false;
            }// Permet de sauter du mur en appuyant sur la touche de saut sans se déplacer
            else if(moveHorizontal == 0f && isWallSliding)
            {
                rb.velocity = new(rb.velocity.x, 1f);
                int direction = isTowardWall ? -currentDirection : currentDirection;
                Vector2 forceToAdd = new(direction * wallJumpForceFall.x, wallJumpForceFall.y);
                rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                timeToDrop = 0f;
                jumpingIsRequired = false;
            }
        }
    }

    private void WallRunning()
    {
        if (moveHorizontal != 0 && !isWallSliding)
        {// Stocke la velocite en X afin de l'appliquer lors du wall running
            if (!isTowardWall && Mathf.Abs(rb.velocity.x) > 2f)
            {
                wallRunningSpeed = Mathf.Abs(rb.velocity.x);
            }
        } else
        {
            wallRunningSpeed = 0f;
        }

        if (isTowardWall && !isWallSliding)
        {
            if (canWallRunning)
            {
                if (isGrounded && !isWallRunning)
                {
                    isWallRunning = true;
                    rb.AddForce(new(0f, wallRunningSpeed), ForceMode2D.Impulse);
                }
                else if (isTowardWall && rb.velocity.y > 0f && !isWallRunning)
                {
                    isWallRunning = true;
                    rb.AddForce(new(0f, wallRunningSpeed), ForceMode2D.Impulse);
                }
                canWallRunning = false;
            }
        } else
        {
            isWallRunning = false;
        }
    }

    private void WallSliding()
    {
        if (!isGrounded && (isTowardWall || hangToWall))
        {
            if (rb.velocity.y <= wallSlideSpeed)
            {
                isWallSliding = true;
                rb.velocity = new(0f, -wallSlideSpeed);
            }
        }
        else
        {
            rb.velocity = new(rb.velocity.x, rb.velocity.y);
            isWallSliding = false;
        }
    }

    // Fonction augmentant la gravité lorsque le personnage est au sommet de son saut
    private void GravityJump()
    {
        bool holdJumpButton = Input.GetButton("Jump");
        float fallGravityMultiplier = 2f;
        if ((rb.velocity.y < 0f || !holdJumpButton) && isInAir)
        {
            rb.gravityScale = currentGravity * fallGravityMultiplier;
            rb.velocity = new(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
            isFalling = true;
        }
        else if (holdJumpButton)
        {
            rb.gravityScale = currentGravity;
            if (isGrounded)
            {
                isFalling = false;
            }
        }
        else
        {
            isFalling = false;            
        }
    }

    // Fonction générant les zones de détection du personnage et vérifiant si il y a collision
    private void CheckCollider()
    {
        float widthBox = myBoxCollider.size.x * widthCheck;
        float heighthBox = myBoxCollider.size.x * heightCheck;
        Vector2 rightPoint = new(transform.position.x + myBoxCollider.size.x / 2 + myBoxCollider.edgeRadius, transform.position.y);
        Vector2 leftPoint = new(transform.position.x - myBoxCollider.size.x / 2 - myBoxCollider.edgeRadius, transform.position.y);
        Vector2 groundPoint = new(transform.position.x, transform.position.y - myBoxCollider.size.y / 2 - myBoxCollider.edgeRadius + myBoxCollider.offset.y);

        groundCheckArea = Physics2D.OverlapBox(groundPoint, new Vector2(widthBox, heighthBox), 0f, multipleLayer);
        leftWallCheckArea = Physics2D.OverlapBox(leftPoint, new Vector2(heighthBox, widthBox), 0f, groundLayer);
        rightWallCheckArea = Physics2D.OverlapBox(rightPoint, new Vector2(heighthBox, widthBox), 0f, groundLayer);
        // Area passe en true selon Layer detecte ou area activée / désactivée
        isGrounded = groundCheckArea != null;
        onLeftWall = leftWallCheckArea != null && currentDirection == -1;
        onRightWall = rightWallCheckArea != null && currentDirection == 1;
        isTowardWall = onLeftWall || onRightWall;
        touchWall = leftWallCheckArea != null || rightWallCheckArea != null;
        isInAir = !isGrounded && !leftWallCheckArea && !rightWallCheckArea;

    }
    // Fonction créant des evenement selon les collisions
    private void ColliderEvent()
    {
        rb.gravityScale = isGrounded ? 1f : currentGravity;
        jumpAvailable = isGrounded || isWallSliding ? jumpMax : jumpAvailable;
        if(isGrounded) isFalling = false;
        if (isGrounded || isInAir) canWallRunning = true;
    }

    // Si le joueur est accroché au mur ou non
    private void DropWall()
    {
        if ((leftWallCheckArea != null || rightWallCheckArea != null) && !isGrounded)
        {

            if (isTowardWall)
            {
                hangToWall = false;
                timeToDrop = timeToDropMax;
            }
            else if(moveHorizontal != 0f && !isTowardWall)
            {
                hangToWall = true;
                timeToDrop -= Time.deltaTime;
                if(timeToDrop < 0)
                {
                    hangToWall = false;
                    Vector2 forceToAdd = new(currentDirection, 0f);
                    rb.AddForce(forceToAdd, ForceMode2D.Impulse);
                    Debug.Log("hangTow");
                }
            }
            else
            {
                timeToDrop = timeToDropMax;
            }
        } else
        {
            hangToWall = false;
            timeToDrop = timeToDropMax;
        }
    }
    void OnDrawGizmos()
    {
        float widthBox = myBoxCollider.size.x * widthCheck;
        float heighthBox = myBoxCollider.size.x * heightCheck;
        Vector2 rightPoint = new(transform.position.x + myBoxCollider.size.x / 2 + myBoxCollider.edgeRadius, transform.position.y);
        Vector2 leftPoint = new(transform.position.x - myBoxCollider.size.x / 2 - myBoxCollider.edgeRadius, transform.position.y);
        Vector2 groundPoint = new(transform.position.x, transform.position.y - myBoxCollider.size.y / 2 - myBoxCollider.edgeRadius + myBoxCollider.offset.y);

        Gizmos.color = new Color32(0, 255, 255, 90);
        Gizmos.DrawCube(groundPoint, new Vector2(widthBox, heighthBox));
        Gizmos.DrawCube(leftPoint, new Vector2(heighthBox, widthBox));
        Gizmos.DrawCube(rightPoint, new Vector2(heighthBox, widthBox));
    }
}


