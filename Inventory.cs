using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int kunaiMax = 5;
    public int kunaiCurrent;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioManager audioManager;
    [SerializeField] AudioClip getKunai;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioManager = GetComponent<AudioManager>();
    }
    private void Start()
    {
        FullKunai();
    }

    private void FixedUpdate()
    {
        KunaiInInventory();
    }

    private void KunaiInInventory()
    {
        kunaiCurrent = kunaiCurrent <= 0 ? 0 : kunaiCurrent >= kunaiMax ? kunaiMax : kunaiCurrent;
    }

    public void FullKunai()
    {
            kunaiCurrent = kunaiMax;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("CollectibleWeapon"))
        {
            audioManager.PlayClip(audioSource, getKunai);
            kunaiCurrent++;
            Destroy(col.gameObject);
        }
    }
}
