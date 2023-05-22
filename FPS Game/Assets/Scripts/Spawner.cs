using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] int intervalTime;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int prefabMaxNum;
    [SerializeField] int spawnInterval;
    [SerializeField] ParticleSystem[] spawnFX;
    [SerializeField] AudioSource playOnActivate;

    public List<GameObject> prefabList = new List<GameObject>();

    int prefabsSpawnCount;
    bool playerInRange;
    bool isSpawning;

    public GameObject entrance;
    public GameObject exit;

    void Update()
    {
        if (playerInRange && !isSpawning && prefabsSpawnCount < prefabMaxNum)
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;
        if (playOnActivate)
            playOnActivate.Play();
        for (int i = 0; i < spawnFX.Length; i++)
        {
            spawnFX[i].Play();
        }
        yield return new WaitForSeconds(spawnInterval);
        for (int i = 0; i < spawnPos.Length; i++)
        {
            spawnFX[i].Stop();
            GameObject prefabClone = Instantiate(prefab, spawnPos[i].position, prefab.transform.rotation);
            prefabList.Add(prefabClone);
            prefabsSpawnCount++;
        }
        yield return new WaitForSeconds(intervalTime);
        isSpawning = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void ResetSpawner()
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            Destroy(prefabList[i]);
            prefabsSpawnCount--;
            GameManager.Instance.UpdateGameGoal(-1);
        }
        prefabList.Clear();
    }
}
