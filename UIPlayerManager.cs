using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPlayerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text textCoolDown;
    [SerializeField] private TMP_Text textKunai;
    [SerializeField] private Image fillImageDash;
    [SerializeField] private GameObject kunaiUI;
    [SerializeField] private GameObject permutUI;
    [SerializeField] private GameObject player;
    [SerializeField] private Inventory inventoryPlayer;
    [SerializeField] private Permutation permutationPlayer;
    [SerializeField] private AimAndShoot aas;


    private float coolDownTimer;
    private float coolDownTimeFix;

    private bool canDodge;
    private bool coolDownToDodge;
    private bool cooldownToHit;

    private void Awake()
    {
        player = GameObject.Find("Player");
        inventoryPlayer = player.GetComponent<Inventory>();
        permutationPlayer = player.GetComponent<Permutation>();
        aas = player.GetComponentInChildren<AimAndShoot>();
    }
    private void Start()
    {
        fillImageDash.fillAmount = 0f;
        textCoolDown.gameObject.SetActive(false);
    }

    private void Update()
    {
        DisplayUI();
        cooldownToHit = permutationPlayer.cooldownToHit;
        canDodge = permutationPlayer.canDodge;
        coolDownToDodge = permutationPlayer.cooldownToDodge;

        coolDownTimer = permutationPlayer.coolDownTimeBeforeLaunch;
        coolDownTimeFix = permutationPlayer.coolDownTimeMaxBeforeLaunch;

        if (!cooldownToHit || coolDownToDodge)
        {
            UseDash();
        }
        if (coolDownToDodge)
        {
            ApplyCountDown();
        }
        KunaiCounter();
    }

    private void DisplayUI()
    {
        kunaiUI.SetActive(aas.launchKunaiUnlocked);
        permutUI.SetActive(permutationPlayer.permutUnlocked);
    }
    private void ApplyCountDown()
    {
        //coolDownTimer -= Time.deltaTime;

        if(coolDownTimer < 0f)
        {// Desaffiche le texte et le remplissage
            textCoolDown.gameObject.SetActive(false);
            fillImageDash.fillAmount = 0f;
        }
        else if(coolDownTimer < coolDownTimeFix)
        {
            textCoolDown.gameObject.SetActive(true);
            textCoolDown.text = coolDownTimer.ToString("0.0"); // Fais defiler le temps
            fillImageDash.fillAmount = coolDownTimer / coolDownTimeFix; // Time who defilled
        }
        else 
        {
            return; 
        }
    }

    private void UseDash()
    {
        if (canDodge) // Si peut esquiver : Rien
        {
            return;
        } else // Recup le temps de cooldown
        {
           // textCoolDown.gameObject.SetActive(true);
        }
    }

    private void KunaiCounter()
    {
        textKunai.text = inventoryPlayer.kunaiCurrent.ToString();
    }
}
