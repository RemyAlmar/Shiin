using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    // Start is called before the first frame update
    void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        for(int i = 0; i < buttons.Length; i++)
        {
            if(i + 1 > levelReached)
            {
                buttons[i].interactable = false;
            }
        }
    }
}
