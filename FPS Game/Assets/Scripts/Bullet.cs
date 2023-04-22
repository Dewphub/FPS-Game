using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int timer;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);
    }

    void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();
        Debug.Log("Bullet OnTriggerEnter entered, collision with: " + other.gameObject.name);
        damageable?.TakeDamage(damage);

        Destroy(gameObject);
    }
}
