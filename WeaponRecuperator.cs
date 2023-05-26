using UnityEngine;

public class WeaponRecuperator : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioManager audioManager;
    [SerializeField] AudioClip soundRecupField;
    [Header("Parametre")]
    [SerializeField] bool recuperatorField;
    [SerializeField] bool vfxIsPlaying;
    [SerializeField, Range(0f, 5f)] float timeMax = 0.5f;
    [SerializeField, Range(0f, 5f)] float time;
    [SerializeField, Range(5f, 100f)] float speedRecuperationMax = 50;
    [SerializeField, Range(5f, 1000f)] float fieldRecuperatorRange;
    [SerializeField] LayerMask groundWeaponMask;
    [SerializeField] LayerMask weaponMask;

    [SerializeField] ParticleSystem vfx_attractiveField;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioManager = GetComponent<AudioManager>();
        vfx_attractiveField = GetComponentInChildren<ParticleSystem>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("RightShoulder") != 0)
        {
           // Debug.Log(Input.GetAxis("RightShoulder"));
        }
        recuperatorField = Input.GetButton("FieldRecuperator");
        ActiveFieldRecuperator();
    }

    private void ActiveFieldRecuperator()
    {
        Collider2D[] weaponToRecover = Physics2D.OverlapCircleAll(transform.position, fieldRecuperatorRange, weaponMask);
        if (recuperatorField)
        {
            if (!vfxIsPlaying)
            {
                vfxIsPlaying = true;
                vfx_attractiveField.Play();
            }
            audioManager.PlayClip(audioSource, soundRecupField, recuperatorField);
            if(time > 0)
            {
                time -= 0.5f * Time.deltaTime;
            }
            if(weaponToRecover != null)
            {
                foreach (Collider2D weapon in weaponToRecover)
                {
                    if (weapon.TryGetComponent(out Kunai kunai) && weapon.CompareTag("CollectibleWeapon"))
                    {
                        kunai.returnToTarget = true;
                        kunai.GoToTarget(transform, speedRecuperationMax, time);
                    }
                }
            } 
        } 
        else
        {
            vfxIsPlaying = false;
            vfx_attractiveField.Stop();
            time = timeMax;
            foreach(Collider2D weapon in weaponToRecover)
            {
                if (weapon.TryGetComponent(out Kunai kunai))
                {
                    kunai.returnToTarget = false;
                }
            }
        }
    }
    /*
    private void OnDrawGizmos()
    {
        if (recuperatorField)
        {
            Gizmos.color = new Color32(0, 255, 255, 90);
            Gizmos.DrawSphere(transform.position, fieldRecuperatorRange);
        }
    }*/
}
