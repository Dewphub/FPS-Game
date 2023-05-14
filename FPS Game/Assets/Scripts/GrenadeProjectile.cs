using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] int timer;
    Transform shooter;
    void Start()
    {
        Destroy(gameObject, timer);
    }
    void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();
        damageable?.TakeDamage(damage);
        if (other.gameObject.CompareTag("Player"))
        {
            Register();
        }
        Destroy(gameObject);
    }

    /*IEnumerator Flip()
    {
        isFlipping = true;  
        Quaternion startRot = transform.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(-180, 0, 0);
        float t = 0;
        while(t < flipDuration)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(startRot, targetRot, t/flipDuration);
            yield return null;
        }
        transform.localRotation = Quaternion.Slerp(startRot, targetRot, 1);
        isFlipping = false;
    }*/
    public void SetShooter(Transform _shooter)
    {
        this.shooter = _shooter;
    }

    void Register()
    {
        if (!DamageIndicatorSystem.CheckIfObjectInSight(shooter))
        {
            DamageIndicatorSystem.CreateIndicator(shooter);
        }
    }
}
