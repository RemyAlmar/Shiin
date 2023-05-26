using UnityEngine;

public class Permutation : MonoBehaviour
{
    [Header("Get Your Audio HERE")]
    [SerializeField] private AudioClip permutSound;
    private AudioManager audioManager;
    private AudioSource audioSource;

    [Header("CoolDown && Others")]
    [SerializeField] private float coolDownTimeCurrentToHit;
    [SerializeField] private float coolDownTimeMaxToHit;
    [SerializeField, Range(1f, 5f)] private float dodgeDistance;
    [SerializeField] private bool dodgeActivated;
    [SerializeField] private bool dodgeButton;
    [SerializeField] private GameObject permutPuppet;
    [SerializeField] private LayerMask groundLayer;

    public bool cooldownToDodge;
    public float coolDownTimeBeforeLaunch;
    public bool canDodge;
    public bool cooldownToHit;
    public float coolDownTimeMaxBeforeLaunch;
    public bool permutUnlocked;

    private ControlMovePlayer cmp;
    private PlayParticle playParticle;
    // Start is called before the first frame update
    void Awake()
    {
        cmp = GetComponent<ControlMovePlayer>();
        playParticle = GetComponent<PlayParticle>();
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        coolDownTimeCurrentToHit = coolDownTimeMaxToHit;
    }
    // Update is called once per frame
    void Update()
    {
        dodgeButton = Input.GetButtonDown("Dash");
        PermutActivation();
    }

    // Fonction pour dash
    private void PermutActivation()
    {
        if (dodgeButton && canDodge && !cmp.IsWallSliding && permutUnlocked)
        {
            canDodge = false;
            cooldownToHit = true;
            audioManager.PlayClip(audioSource, permutSound);
        }
        CooldownForHit();
    }
    public void PermutationTrigger()
    {
        dodgeActivated = true;
        playParticle.PlaySound();
        playParticle.LaunchParticle();
        GameObject newPuppet = Instantiate(permutPuppet, transform.position, transform.rotation);
        Destroy(newPuppet, 5f);
        // Lance Un raycast à des coordonnées aléatoires et s'y rend, si il y'a un obstacle le joueur s'arrete avant 
        float dodgePosXRandom = Random.Range(-2f, 2f);
        float dodgePosYRandom = Random.Range(1f, 5f);
        Vector3 dodgeDir = new(dodgePosXRandom * dodgeDistance, dodgePosYRandom);
        RaycastHit2D dodgeRay = Physics2D.Raycast(transform.position, dodgeDir, dodgeDistance, groundLayer);
        if (dodgeRay)
        {
            transform.position = new(dodgeRay.point.x - 0.5f * cmp.CurrentDirection, dodgeRay.point.y);
        }
        else
        {
            transform.position += dodgeDir;
        }
    }
    /// <summary>
    /// Si cooldown pour se faire frapper est sup à 0 et que la compétence dodge a été activée : le cooldown pour se faire frapper se réduit;
    /// Si le cooldown pour se faire frapper n'est pas terminé et que le joueur se fait frapper, le cooldown de la compétence ne durera qu'1 sec;
    /// Sinon il durera 5s
    /// </summary>
    private void CooldownForHit()
    {
        if (cooldownToHit && coolDownTimeCurrentToHit >= 0)
        {
            coolDownTimeCurrentToHit -= Time.deltaTime;
            if (dodgeActivated)
            {
                coolDownTimeMaxBeforeLaunch = 1f;
                coolDownTimeBeforeLaunch = coolDownTimeMaxBeforeLaunch;
                cooldownToHit = false;
                cooldownToDodge = true;
                dodgeActivated = false;
            } else
            {
                coolDownTimeMaxBeforeLaunch = 5f;
                coolDownTimeBeforeLaunch = coolDownTimeMaxBeforeLaunch;
                cooldownToDodge = true;
            }
        }
        else
        {
            coolDownTimeCurrentToHit = coolDownTimeMaxToHit;
            cooldownToHit = false;
            LaunchCoolDownToLaunch();
        }
    }
    private void LaunchCoolDownToLaunch()
    {
        if (coolDownTimeBeforeLaunch >= 0 && !canDodge && cooldownToDodge)
        {
            coolDownTimeBeforeLaunch -= Time.deltaTime;
        }
        else
        {
            coolDownTimeBeforeLaunch = coolDownTimeMaxBeforeLaunch;
            canDodge = true;
            cooldownToDodge = false;
        }
    }
}
