using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StopMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject OnScreenMenu;
    bool isPaused;
    [SerializeField] private TMP_Text PauseMenuscoreText;
    [SerializeField] private TMP_Text PauseMenuwrongClicksText;
    [SerializeField] private TMP_Text PauseMenuAccuracyText;
    public GameLogic gameLogic;
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
        UpdateStats();
        OnScreenMenu.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        OnScreenMenu.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }
    void UpdateStats()
    {
        PauseMenuscoreText.text = "Score: " + gameLogic.GetScore();
        PauseMenuwrongClicksText.text = "Wrong Clicks: " + gameLogic.GetWrongClicks();
        PauseMenuAccuracyText.text = $"Accuracy: {gameLogic.GetAccuracy():F1}%";
    }

}
