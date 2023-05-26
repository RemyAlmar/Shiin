using System;
using System.Collections.Generic;
using UnityEngine;

public class AimAndShoot : MonoBehaviour
{
    public bool launchKunaiUnlocked;         
    [SerializeField] private Inventory Inventory;
    [SerializeField] private SpriteRenderer GraphicSight;
    [SerializeField] private Transform ShotPoint;
    [SerializeField] private Kunai Kunai;


    [SerializeField] private float launchForce = 50f;
    [SerializeField] private bool isShoot;          // Tir clic gauche
    [SerializeField] private bool isAim;            // Maintien clic droit
    [SerializeField] private float concentrationButton;            // Maintien clic droit
    [SerializeField] private bool isConcentrate;            // Maintien clic droit
    [SerializeField] private float isShootJoy;      // Maintien la gachette appuyée
    [SerializeField] private bool isShootByJoy;     // Tire avec joystick
    [SerializeField] private bool canShoot;         // Peut tirer avec joystick
    [SerializeField] private float isAimJoystickX;  // Joystick gauche en X
    [SerializeField] private float isAimJoystickY;  // Joystick gauche en Y

    [SerializeField] private Vector2 mousePosition;
    [SerializeField] private Vector2 mouseDirection;
    [SerializeField] private Vector2 joystickDir;
    public bool IsShoot => isShoot;
    public bool IsAim => isAim;
    public SpriteRenderer GFX
    {
        get { return GraphicSight; }
        set { GFX = value; }
    }
    private float defaultFixedDeltaTime;
    [Space]
    [Header("SlowMotion")]
    [SerializeField, Range(0.01f, 0.9f)] private float timeScaleSlowMotion = 0.5f;
    [SerializeField] private float durationSlowMotion;
    [SerializeField, Range(0.5f, 1f)] private float timeMax = 0.5f;

    private void Awake()
    {
        defaultFixedDeltaTime = Time.fixedDeltaTime;
        durationSlowMotion = timeMax;
        GraphicSight.enabled = false;
    }
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isShoot = Input.GetButtonDown("Attack");
        isShootJoy = Input.GetAxis("RightShoulder");
        concentrationButton = Input.GetAxis("LeftShoulder");
        isAim = Input.GetButton("Aiming");
        isAimJoystickX = Input.GetAxis("LeftJoystickHorizontal");
        isAimJoystickY = Input.GetAxis("LeftJoystickVertical");
        joystickDir = new(isAimJoystickX, isAimJoystickY);

        if (!PauseMenu.GameIsPaused && launchKunaiUnlocked)
        {
            if(concentrationButton == 0f && !isAim)
            {
                isConcentrate = false;
            }
            if ((isAim || concentrationButton > 0.5f) && !isConcentrate)
            {
                StartSlowingTime(timeScaleSlowMotion);
            }else
            {
                ResetTimeScale();
            }

            TriggerJoy();
            Aiming();
            Shoot();
        }
    }
    private void StartSlowingTime(float scaleTime)
    {
        if (durationSlowMotion > 0)
        {
            SlowTimeScale(scaleTime);
            durationSlowMotion -= Time.deltaTime;
        }
        else
        {
            isConcentrate = true;
            ResetTimeScale();
            durationSlowMotion = timeMax;
        }
    }
    private void SlowTimeScale(float scaleTime)
    {
        Time.timeScale = scaleTime;
        Time.fixedDeltaTime = defaultFixedDeltaTime * Time.timeScale;
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
    }
    private void Aiming()
    {
        Vector2 sightPosition = transform.position;
        mouseDirection = (mousePosition - sightPosition).normalized;
        transform.right = joystickDir != Vector2.zero ? joystickDir : mouseDirection;

        if (isAim || joystickDir != Vector2.zero)
        {
            GraphicSight.enabled = true;
        }
        else
        {
            GraphicSight.enabled = false;
        }
    }

    // Pour ne pas tirer tous les kunais
    private void TriggerJoy()
    {
        if (isShootJoy >= 0.5f && canShoot)
        {
            if (!isShootByJoy)
            {
                isShootByJoy = true;
            }
        }
        else if (isShootJoy <= 0.2f)
        {
            canShoot = true;
            isShootByJoy = false;
        }
        else return;
    }

    private void Shoot()
    {
        if (((isShoot && isAim) ||(joystickDir != Vector2.zero && isShootByJoy)) && Inventory.kunaiCurrent > 0 && canShoot)
        {
            canShoot = false;
            LaunchKunai(ShotPoint, launchForce, Kunai);
            Inventory.kunaiCurrent--;
        }
    }

    /// <summary>
    /// Instancie un objet à un point donné en lui appliquant la position et la rotation du ShotPoint
    /// , l'ajoute dans une liste et lui donne une vélocité en rapport avec l'orientation du ShotPoint
    /// </summary>
    /// <param name="shotPoint"> Position du point d'ou le kunai sera instancier</param>
    /// <param name="launchForce"> Puissance de lancer</param>
    /// <param name="kunai"> Objet instancier</param>
    private void LaunchKunai(Transform shotPoint, float launchForce, Kunai kunai)
    {
        Kunai newKunai = Instantiate(kunai, shotPoint.position, shotPoint.rotation);
        //listKunai.Add(newKunai);
        newKunai.Rb.velocity = shotPoint.right * launchForce;
    }
}
