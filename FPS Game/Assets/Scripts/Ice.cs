using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    public float slidingForce = 100f; // The force applied to objects sliding on the ice

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Apply ice physics to the object
                // rb.material = ice;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Apply sliding force to objects on the ice
                rb.AddForce(rb.velocity.normalized * slidingForce * Time.deltaTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Reset the PhysicMaterial to its default value
                // rb.material = null;
            }
        }
    }
}
