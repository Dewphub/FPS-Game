using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switches : MonoBehaviour
{
    public bool isActivated = false;
    public GameObject puzzleObject; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateSwitch();
        }
    }

    private void ActivateSwitch()
    {
        isActivated = true;
        Debug.Log("Switch activated!");

        if (puzzleObject != null)
        {
            puzzleObject.SetActive(true);
        }
    }
}
