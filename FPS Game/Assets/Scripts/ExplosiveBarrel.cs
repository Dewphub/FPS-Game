using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ExplosiveBarrel : MonoBehaviour, IDamage
{
    [SerializeField] private GameObject explosionTrigger;
    [SerializeField] int damage;
    [SerializeField] GameObject[] explosionFX;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip explosionAudio;

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
        explosionTrigger.SetActive(true);
        Destroy(gameObject);
    }
}
