using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersitenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults");
            NewGame();
        }

        foreach (IDataPersistence curObject in dataPersistenceObjects)
        {
            curObject.LoadData(gameData);
        }

        Debug.Log("Game loaded successfully");
    }

    public void SaveGame()
    {
        foreach (IDataPersistence curObject in dataPersistenceObjects)
        {
            curObject.SaveData(ref gameData);
        }

        Debug.Log("Game saved successfully");

        dataHandler.Save(gameData);
        dataHandler.SaveLevel(SceneManager.GetActiveScene().buildIndex);
    }

    public void ModifyDeaths(int modifier, bool overwrite = false)
    {
        float tempTime = this.gameData.time;
        this.gameData = dataHandler.Load();
        this.gameData.time = tempTime;
        if (!overwrite)
            this.gameData.deaths += modifier;
        else
            this.gameData.deaths = modifier;
        dataHandler.Save(gameData);
    }
    public void ModifySecrets(int modifier, bool overwrite = false)
    {
        if (!overwrite)
            this.gameData.secretsFound += modifier;
        else
            this.gameData.secretsFound = modifier;
    }
    public void ModifyEnemiesKilled(int modifier, bool overwrite = false)
    {
        if (!overwrite)
            this.gameData.enemiesKilled += modifier;
        else
            this.gameData.enemiesKilled = modifier;
    }

    /*private void OnApplicationQuit()
    {
        SaveGame();
    }*/

    private List<IDataPersistence> FindAllDataPersitenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
