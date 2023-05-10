using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlanes : MonoBehaviour
{

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            KillPlayer(other.gameObject);
        }
    }

    public void KillPlayer(GameObject player)
    {
        GameManager.Instance.OnDead();
    }
}
