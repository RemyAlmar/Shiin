using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Vector2 checkPosition;
    private bool isActivated;
    private AudioManager audioManager;
    private AudioSource audioSource;
    [SerializeField] private AudioClip ac_yoi;

    private void Awake()
    {
        audioManager = GetComponent<AudioManager>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        checkPosition = transform.position;    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Respawn respawn))
        {
            respawn.RecordNewCheckPoint(checkPosition);
            if (!isActivated)
            {
                audioManager.PlayClip(audioSource, ac_yoi);
                isActivated = true;
            }
        }
    }
}
