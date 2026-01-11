using UnityEngine;
using UnityEngine.SceneManagement;

public class StopMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject OnScreenMenu;
    bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    public void OnQuit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StoneTrial");
    }
    public void PauseGame()
    {
        OnScreenMenu.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        OnScreenMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
}
