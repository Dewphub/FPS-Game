using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class MainMenu : MonoBehaviour
{
    private FileDataHandler dataHandler;
    private FileDataHandler curDataHandler;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject startMenu;
    [SerializeField] Slider loadingBar;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] menuAud;
    [SerializeField] Animator transition;
    
    float transitionTime = 1f;

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, "saveData1.game");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if (dataHandler.LoadLevel() == -1)
        {
            Destroy(continueButton);
        }
    }

    public void playSound(int soundIndex)
    {
        if (soundIndex < 0)
            aud.PlayOneShot(menuAud[Random.Range(0, menuAud.Length)], 1);
        else
            aud.PlayOneShot(menuAud[soundIndex], 1);
    }

    public void NewGame(int sceneIndex)
    {
        dataHandler.Save(new GameData());
        StartCoroutine(LoadSceneAsynchronously(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void ContinueGame()
    {
        StartCoroutine(LoadSceneAsynchronously(dataHandler.LoadLevel()));
        //SceneManager.LoadScene(dataHandler.LoadLevel());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        SceneManager.LoadScene(sceneName: "Credits");
    }

    IEnumerator LoadSceneAsynchronously(int SceneIndex)
    {
        // Play Transition Animation
        transition.SetTrigger("Start");
        // Wait
        yield return new WaitForSeconds(transitionTime);
        // Load Scene
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneIndex);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            loadingBar.value = operation.progress;
            yield return null;
        }
    }
}
