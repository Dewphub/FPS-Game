using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] AmmoStats ammoType;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(GameManager.Instance.playerScript.GetGunList().Count > 0 && GameManager.Instance.playerScript.GetGunList().Contains(ammoType.gun))
            {
                GameManager.Instance.playerScript.AmmoPickUp(ammoType);
                gameObject.SetActive(false);
            }
        }
    }
}
