using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingBarrelDamageDealer : MonoBehaviour
{
    [SerializeField] int damage;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<IDamage>() != null)
        {
            IDamage damageable = other.gameObject.GetComponent<IDamage>();
            damageable?.TakeDamage(damage);
        }
    }
}
