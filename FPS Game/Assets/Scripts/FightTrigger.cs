using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightTrigger: MonoBehaviour
{
    [SerializeField] Transform[] block;
    [SerializeField] Vector3 triggerPos;

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        { 

            foreach (Transform t in block)
            {
                t.position = triggerPos;
             }    
        }
    }

}
