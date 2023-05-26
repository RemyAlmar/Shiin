using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour
{
    private Scene scene;
    public static DontDestroyOnLoad instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "MainMenu" || scene.name == "LevelChoice" || scene.name == "Credits")
        {
            DontDestroyOnLoad(gameObject);    
        }else
        {
            DestroyImmediate(gameObject);
        }
    }
}
