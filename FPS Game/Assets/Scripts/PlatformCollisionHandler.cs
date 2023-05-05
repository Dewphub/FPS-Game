using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollisionHandler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
        }
    }
}
