using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlanes : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
       
        if (collision.collider.CompareTag("Player"))
        {
        
            KillPlayer(collision.collider.gameObject);
        }
    }

    public void KillPlayer(GameObject player)
    {
        GameManager.Instance.OnDead();
    }
}
