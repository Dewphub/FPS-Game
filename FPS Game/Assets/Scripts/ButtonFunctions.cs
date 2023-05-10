using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Instance.ResumeState();
        GameManager.Instance.isPaused = !GameManager.Instance.isPaused;
    }

    public void Restart()
    {
        GameManager.Instance.ResumeState();
        GameManager.Instance.ReloadLevelAsynchronously();
    }

    public void Continue()
    {
        if(SceneManager.GetActiveScene().buildIndex+1 == SceneManager.sceneCountInBuildSettings)
            GameManager.Instance.LoadNextLevelAsynchronously();
    }

    public void Quit()
    {
        Debug.Log("Quit is being called");
        //DataPersistenceManager.Instance.SaveGame();
        Application.Quit();
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.ResumeState();
        GameManager.Instance.playerScript.Respawn();
    }
}
