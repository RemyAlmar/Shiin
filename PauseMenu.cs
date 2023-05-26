using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuContainer;
    // Update is called once per frame
    private void Start()
    {
        Resume();
    }
    void Update()
    {
        GetPaused();
    }

    public void GetPaused()
    {
        if (Input.GetButtonDown("Escape"))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuContainer.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1;
    }

    private void Pause()
    {
        pauseMenuContainer.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
        GameIsPaused = false;
    }

    public void QuitGame()
    {
    }
}
