using UnityEngine;

public class CurrentSceneManager : MonoBehaviour
{
    public int levelToUnlock;
    public bool objectiveIsDone;
    public static CurrentSceneManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        else
            Instance = this;
    }
}
