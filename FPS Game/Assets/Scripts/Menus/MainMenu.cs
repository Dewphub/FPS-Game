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

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, "saveData1.game");
        if (dataHandler.LoadLevel() == -1)
        {
            Destroy(continueButton);
        }
    }

    public void NewGame()
    {
        dataHandler.Save(new GameData());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(dataHandler.LoadLevel());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
