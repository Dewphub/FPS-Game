using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTrigger: MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] GameObject trigger;
    [SerializeField] int intervalTime;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] int prefabMaxNum;

    public List<GameObject> prefabList = new List<GameObject>();

    int prefabsSpawnCount;
    bool playerInRange;
    bool isSpawning;

    // Start is called before the first frame update
    void Start()
    {
        //GameManager.Instance.UpdateGameGoal(prefabMaxNum);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !isSpawning && prefabsSpawnCount < prefabMaxNum)
        {
            StartCoroutine(Spawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    IEnumerator Spawn()
    {
        isSpawning = true;
        GameObject prefabClone = Instantiate(trigger, spawnPos[Random.Range(0, spawnPos.Length)].position, trigger.transform.rotation);
        prefabList.Add(prefabClone);
        prefabsSpawnCount++;
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
}
