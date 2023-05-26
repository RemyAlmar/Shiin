using UnityEngine;

public class DontDestroyOnLoadFade : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
