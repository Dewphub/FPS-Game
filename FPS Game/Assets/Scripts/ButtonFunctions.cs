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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Debug.Log("Quit is being called");
        Application.Quit();
    }
}
