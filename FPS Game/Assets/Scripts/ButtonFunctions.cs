using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] menuAud;
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

    public void playSound(int soundIndex)
    {
        if (soundIndex < 0)
            aud.PlayOneShot(menuAud[Random.Range(0, menuAud.Length)], 1);
        else
            aud.PlayOneShot(menuAud[soundIndex], 1);
    }

    public void Continue()
    {
        GameManager.Instance.ResumeState();
        GameManager.Instance.LoadNextLevelAsynchronously();
    }

    public void Quit()
    {
        Debug.Log("Quit is being called");
        DataPersistenceManager.Instance.LoadGame();
        DataPersistenceManager.Instance.SaveGame();
        Application.Quit();
    }

    public void RespawnPlayer()
    {
        GameManager.Instance.ResumeState();
        GameManager.Instance.playerScript.Respawn();
    }
}
