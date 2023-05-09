using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private FileDataHandler dataHandler;
    private FileDataHandler curDataHandler;
    [SerializeField] GameObject continueButton;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject startMenu;
    [SerializeField] Slider loadingBar;

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, "saveData1.game");
        if (dataHandler.LoadLevel() == -1)
        {
            Destroy(continueButton);
        }
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

    IEnumerator LoadSceneAsynchronously(int SceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneIndex);
        startMenu.SetActive(false);
        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            loadingBar.value = operation.progress;
            yield return null;
        }
    }
}
