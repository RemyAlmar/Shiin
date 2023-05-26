using System.Collections;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public static MeleeAttack instance;
    [SerializeField] private float rangeAssassinate = 1.5f;
    [SerializeField] private float pushForceX;
    [SerializeField, Range(0.1f, 1f)] private float repulseTime = 0.3f;
    [SerializeField] private int currentDirection;
    public bool isAttacking;
    public bool isMurdering;

    [SerializeField] private bool attackButton;
    [SerializeField] private bool aimButton;
    [SerializeField] private bool interact;

    [SerializeField] private Vector2 startHitBox;
    [SerializeField] private Vector2 sizeHitBox;
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private BoxCollider2D myBox;
    [SerializeField] private Inventory inventory;
    [SerializeField] private LayerMask notPlayer;
    
    public Animator myAnim;
    private ControlMovePlayer controlMovePlayer;
    
    private void Awake()
    {
        instance = this;    
        controlMovePlayer = GetComponent<ControlMovePlayer>();
        myBox = GetComponent<BoxCollider2D>();
        inventory = GetComponent<Inventory>();
        myAnim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        VariousVariable();
        Assassinate();
    }
    // Variable diverses comme les boutons ou autre initialisations de variable
    private void VariousVariable()
    {
        attackButton = Input.GetButtonDown("Attack");
        aimButton = Input.GetButton("Aiming");
        interact = Input.GetButton("Interact");
        currentDirection = controlMovePlayer.CurrentDirection;
        if (attackButton && !aimButton && !controlMovePlayer.IsTowardWall)
        {
            isAttacking = true;
        }
    }

    public void Assassinate()
    {
        if (interact && !isMurdering)
        {
            CreateHitBox(rangeAssassinate);
            if(hitBox != null)
            {
                if (hitBox.TryGetComponent(out Assassination assassination) && hitBox.TryGetComponent(out HealthEnemy healthEnemy))
                {
                    if (assassination.CanBeAssassinate && healthEnemy.Health > 0)
                    {
                        isMurdering = true;
                        // Launch Animation "Assassinate" - Case 4 + event who passed isMurdering to false
                    }
                }
            }
        }
    }
    /// <summary>
    /// Appelé en animation event du script Animation_Player.LaunchForce()
    /// </summary>
    /// <param name="rangeAttack"></param>
    /// <param name="damageAttack"></param>
    public void Attack(float rangeAttack, int damageAttack, float repulseForce)
    {
        CreateHitBox(rangeAttack);
        if (hitBox != null)
        {
            if (hitBox.TryGetComponent(out HealthEnemy healthEnemy))
            {
                pushForceX = currentDirection * repulseForce * rangeAttack;
                Vector2 pushForce = new(pushForceX, repulseForce);
                // Recupere les kunai envoyés précedemment sur la cible, inflige des dommages et repousse la cible
                StartCoroutine(healthEnemy.TakeDamage(damageAttack, repulseTime * repulseForce, controlMovePlayer.CurrentDirection, pushForce));
                inventory.kunaiCurrent += healthEnemy.TakeOutKunaiInBody(damageAttack);
            }
        }
    }

    public Collider2D CreateHitBox(float rangeAttack)
    {
        startHitBox = new(transform.position.x + currentDirection * rangeAttack / 2f, transform.position.y);
        sizeHitBox = new(rangeAttack, myBox.size.y);
        hitBox = Physics2D.OverlapBox(startHitBox, sizeHitBox, 0f, notPlayer);
        return hitBox;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color32(0, 255, 255, 90);
        Gizmos.DrawCube(startHitBox, sizeHitBox);
    }
}
