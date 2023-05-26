using Unity.VisualScripting;
using UnityEngine;

public class Assassination : MonoBehaviour
{    
    [SerializeField] private float rangeToAssassinate = 1.5f;
    [SerializeField] private bool canBeAssassinate;
    [SerializeField] private bool isAssassinate;
    [SerializeField] public bool IsAssassinate => isAssassinate;
    public bool CanBeAssassinate => canBeAssassinate;
    [SerializeField] private Collider2D assassinateArea;
    [SerializeField] private LayerMask player;
    // Icone de meurtre
    [SerializeField] private GameObject murdererIcon;
    [SerializeField] private bool isDisplay;

    [SerializeField] private ComportementAI comportementAI;
    [SerializeField] private HealthEnemy healthEnemy;

    private void Awake()
    {
        comportementAI = GetComponent<ComportementAI>();
        healthEnemy = GetComponent<HealthEnemy>();

    }
    private void Start()
    {
        Vector2 newPos = new(transform.position.x + 0.25f, transform.position.y + 1.5f);
        murdererIcon = Instantiate(murdererIcon, newPos, transform.rotation, transform);
    }
    // Update is called once per frame
    void Update()
    {
        AreaToAssassinate(); 
        DisplayMurderIcon();
    }
    private void AreaToAssassinate()
    {
        assassinateArea = Physics2D.OverlapCircle(transform.position, rangeToAssassinate, player);
        if(assassinateArea != null && !comportementAI.IsFollowingTarget && assassinateArea.TryGetComponent(out MeleeAttack meleeAttack))
        {
            // Afficher Icone ou autre
            if (comportementAI.IsAlerted)
            {
                comportementAI.CurrentDirection = -comportementAI.CurrentDirection;
            }
            canBeAssassinate = true;
            isAssassinate = meleeAttack.isMurdering;
            Debug.Log("suis je assassiné ? " + isAssassinate);
            comportementAI.StopMoving(isAssassinate);
        } else
        {
            canBeAssassinate = false;
        }
    }

    private void DisplayMurderIcon()
    {
        if (canBeAssassinate && !isAssassinate && healthEnemy.Health > 0f)
        {
            if (!isDisplay)
            {
                murdererIcon.SetActive(true);
                isDisplay = true;
            }
            
        } else
        {
            murdererIcon.SetActive(false);
            isDisplay = false;
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255, 0, 255, 90);
        Gizmos.DrawSphere(transform.position, rangeToAssassinate);
    }
}
