using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ExplosiveBarrel : MonoBehaviour, IDamage
{
    [SerializeField] GameObject[] explosionFX;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip explosionAudio;
    [SerializeField] float AOEDamageRadius;
    [SerializeField] int damage;

    bool isExploding;
    public void TakeDamage(int damage)
    {
        StartCoroutine(ProcessExplosion());
    }

    IEnumerator ProcessExplosion()
    {
        isExploding = true;
        if (isExploding)
        {
            for (int i = 0; i < explosionFX.Length; i++)
            {
                explosionFX[i].SetActive(true);
            }
            if (!aud.isPlaying)
            {
                aud.PlayOneShot(explosionAudio);
            }
            yield return new WaitForSeconds(0.5f);
            isExploding = false;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, AOEDamageRadius);
        {
            foreach(Collider collider in colliders)
            {
                if(collider.GetComponent<IDamage>() != null)
                {
                    if(collider.CompareTag("Player"))
                    {
                        Register();
                    }
                    IDamage damageable = collider.GetComponent<IDamage>();
                    damageable?.TakeDamage(damage);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    void Register()
    {
        if (!DamageIndicatorSystem.CheckIfObjectInSight(transform))
        {
            DamageIndicatorSystem.CreateIndicator(transform);
        }
    }
}
