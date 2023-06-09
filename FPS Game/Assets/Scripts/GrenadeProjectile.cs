using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrenadeProjectile : MonoBehaviour
{
    [Tooltip("This damage will be if the grenade directly hits the target")]
    [SerializeField, Range(0, 20)] int directDamage;

    [Tooltip("This damage reflects the splash damage associated with missing the target and hitting the terrain")]
    [SerializeField, Range(0, 20)] int AOEDamageAmount;

    [Tooltip("How far away from impact should the splash damage occur")]
    [SerializeField] float AOEDamageArea;

    [Tooltip("How long after instantiating the projectile will it be in scene without collision")]
    [SerializeField] int timer;

    [SerializeField] GameObject explosionFX;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip explosionAudio;
    //This is used for the damage indicator
    Transform shooter;
    private void Start()
    {
        StartCoroutine(ProcessDefaultDestruction());
    }
    void OnTriggerEnter(Collider other)
    {
        IDamage damageable = other.GetComponent<IDamage>();
        damageable?.TakeDamage(directDamage);
        if(!aud.isPlaying)
        {
            aud.PlayOneShot(explosionAudio);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            Register();
            explosionFX.SetActive(true);
            ProcessDestruction(0.5f);
        }
        else if(!other.GetComponent<Collider>().CompareTag("Player"))
        {
            SetShooter(transform);
            IDamage AOEDamage;
            Collider[] colliders = Physics.OverlapSphere(transform.position, AOEDamageArea);
            foreach(Collider collider in colliders)
            {
                if(collider.GetComponent<IDamage>() != null)
                {
                    if(collider.CompareTag("Player"))
                    {
                        Register();
                    }
                    AOEDamage = collider.GetComponent<IDamage>();
                    AOEDamage.TakeDamage(AOEDamageAmount);
                }
            }
            ProcessDestruction(0.5f);
        }
    }
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

    void ProcessDestruction(float delayTime)
    {
        StopAllCoroutines();
        explosionFX.SetActive(true);
        Destroy(gameObject, delayTime);
    }
    IEnumerator ProcessDefaultDestruction(float delayTime = 1.5f)
    {
        yield return new WaitForSeconds(delayTime);
        explosionFX.SetActive(true);
        Destroy(gameObject);
    }
}
