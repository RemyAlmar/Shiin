using UnityEngine;

public class SaveAndLoadSystem : MonoBehaviour
{
    public static SaveAndLoadSystem instance;
    private void Awake()
    {
        if(instance != null)
        {
            return;
        }
       
        instance = this;
        
    }
    /*
    private void Start()
    {
        LoadData();
    }*/

    //Fonction appelée lors de la fin d'un niveau
    public void SaveData()
    {
        if(CurrentSceneManager.Instance.levelToUnlock > PlayerPrefs.GetInt("levelReached", 0))
        {
            PlayerPrefs.SetInt("levelReached", CurrentSceneManager.Instance.levelToUnlock);
        }
        Debug.LogWarning("Faire appel à cette fonction lors de la fin du niveau");
    }
    /*
    public void LoadData()
    {
    }*/
}
