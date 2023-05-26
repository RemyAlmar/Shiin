using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsNavigation : MonoBehaviour
{
    [Header ("Push your audio HERE")]
    [SerializeField] private List<AudioClip> audioClip_list;
    [SerializeField] private AudioManager audioMan;
    [SerializeField] private AudioSource audioSource;
    [Header("Buttons")]
    [SerializeField] private Button[] buttons;
    [SerializeField] private int currentIndex;
    [SerializeField] private float verticalJoy;
    [SerializeField] private bool buttonUp;
    [SerializeField] private bool buttonDown;
    [SerializeField] private bool buttonEnter;
    [SerializeField] private bool canChange;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponentInParent<AudioSource>();
        audioMan = GetComponentInParent<AudioManager>();
        buttons = GetComponentsInChildren<Button>();
    }
    private void Start()
    {
        currentIndex = 0;
    }
    void Update()
    {
        verticalJoy = Input.GetAxisRaw("Vertical");
        buttonEnter = Input.GetButtonDown("Enter");
        buttonUp = Input.GetButtonDown("Up");
        buttonDown = Input.GetButtonDown("Down");
        verticalJoy = verticalJoy > 0.2 ? 1 : verticalJoy < -0.2 ? - 1 : 0;
        Navigation();
    }

    private void Navigation()
    {
        if (canChange)
        {
            if (verticalJoy == 1 || buttonUp)
            {
                if (currentIndex > 0 && buttons[currentIndex - 1].interactable == true)
                {
                    currentIndex--;
                    canChange = false;
                }
                else
                {
                    currentIndex = buttons.Length - 1;
                    canChange = false;
                }
                audioSource.Stop();
                audioMan.PlayClip(audioSource, audioClip_list, 0);
            }
            else if (verticalJoy == -1 || buttonDown)
            {
                if (currentIndex < buttons.Length - 1 && buttons[currentIndex + 1].interactable == true)
                {
                    currentIndex++;
                    canChange = false;
                }
                else
                {
                    currentIndex = 0;
                    canChange = false;
                }
                audioSource.Stop();
                audioMan.PlayClip(audioSource, audioClip_list, 0);
            }
        } else
        {
            TransformJoyInButton();
        }
        if (buttonEnter)
        {
            audioSource.Stop();
            audioMan.PlayClip(audioSource, audioClip_list, 1);
            buttons[currentIndex].onClick.Invoke();
        }
        buttons[currentIndex].Select();
    }

    private void TransformJoyInButton()
    {
        if(verticalJoy < 0.5f && verticalJoy > -0.5f)
        {
            canChange = true;
        }
    }
}
