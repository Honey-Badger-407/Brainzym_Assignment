using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        
        SceneManager.LoadScene("StoneTrial");
        Debug.Log("start");
    }
    public void OnQuitButtonPressed()
    {
        Debug.Log("quit");
        Application.Quit();
    }
    
}