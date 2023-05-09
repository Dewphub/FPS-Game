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

    public void Quit()
    {
        Debug.Log("Quit is being called");
        Application.Quit();
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.ResumeState();
        GameManager.Instance.playerScript.Respawn();
    }
}
