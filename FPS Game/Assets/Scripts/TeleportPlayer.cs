using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    [SerializeField] Transform doorToTeleportTo;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.transform.position = doorToTeleportTo.position;
        }
    }
}
