using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] GameObject triggerEffect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.playerSpawnPos.transform.position != transform.position)
        {
            GameManager.Instance.playerSpawnPos.transform.position = transform.position;
            DataPersistenceManager.Instance.SaveGame();

            if (triggerEffect)
                Instantiate(triggerEffect, transform.position, triggerEffect.transform.rotation);

            StartCoroutine(flashColor());
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        GameManager.Instance.checkpointMenu.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        model.material.color = Color.white;
        GameManager.Instance.checkpointMenu.SetActive(false);
    }
}
