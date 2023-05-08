using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour
{
    [SerializeField] private float iceFriction;
    [SerializeField] private float iceStopThreshold;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Enemy"))
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();

            // Apply ice physics
            rb.sharedMaterial.friction = iceFriction;

            // Detect when object stopped moving
            StartCoroutine(DetectStoppedMovement(rb));
        }
    }

    private IEnumerator DetectStoppedMovement(Rigidbody2D rb)
    {
        while (rb.velocity.magnitude <= iceStopThreshold)
        {
            yield return null;
        }
        rb.sharedMaterial.friction = 0f;
    }
}
