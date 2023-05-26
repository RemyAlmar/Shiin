using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene instance;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance);
            Debug.Log(1);
            return;
        }
        else
        {
            Debug.Log(2);
            instance = this;
        }
    }
    public void LoadScene(int sceneID)
    {
        Time.timeScale = 1;
        StartCoroutine(LoadSceneAsync(sceneID));
        Debug.Log(3);
    }

    private IEnumerator LoadSceneAsync(int sceneID)
    {
        animator.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneID);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
