using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeekerAI : MonoBehaviour, IDamage
{
    [Header("----- Stats -----")]
    [SerializeField] int HP;
    [SerializeField] float lerpDuration;
    [SerializeField] int attackDistance;
    [SerializeField] GameObject[] particles;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip explosion;
    [SerializeField] int damage;

    GameObject player;
    Transform playerTransform;
    float elapsedTime;
    bool isExploding;

    bool isAttacking;
    bool isDead;
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerTransform = player.transform;
    }
    private void Update()
    {
        Vector3 attackDirection = playerTransform.position - transform.position;
        transform.forward = Vector3.Lerp(transform.forward, attackDirection, Time.fixedDeltaTime);
        if (Vector3.Distance(transform.position, playerTransform.position) < attackDistance)
        {
            Invoke("AttackPlayer", .05f);
        }
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;
        if(HP <= 0 && !isDead)
        {
            StartCoroutine(ProcessDeathExplosion());
        }
    }

    public void AttackPlayer()
    {
        if (elapsedTime < lerpDuration)
        {
            transform.position = Vector3.Lerp(transform.localPosition, playerTransform.position, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Register();
            IDamage damageable = collision.gameObject.GetComponent<IDamage>();
            damageable?.TakeDamage(damage);
            StartCoroutine(ProcessHitExplosion());
        }
    }

    IEnumerator ProcessDeathExplosion()
    {
        isExploding = true;
        if(isExploding)
        { 
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(true);
            }
            if(!aud.isPlaying && explosion != null)
            {
                aud.PlayOneShot(explosion);
            }
            yield return new WaitForSeconds(0.5f);
            isExploding = false;
        }
        if (!isDead)
        {
            Die();
        }
    }
    IEnumerator ProcessHitExplosion()
    {
        isExploding = true;
        if (isExploding)
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(true);
            }
            if (!aud.isPlaying && explosion != null)
            {
                aud.PlayOneShot(explosion);
            }
            yield return new WaitForSeconds(0.01f);
            isExploding = false;
        }
        if(!isDead)
        {
            Die();
        }
    }

    void Register()
    {
        if (!DamageIndicatorSystem.CheckIfObjectInSight(transform))
        {
            DamageIndicatorSystem.CreateIndicator(transform);
        }
    }

    void Die()
    {
        GameManager.Instance.UpdateGameGoal(-1);
        if(!isDead)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}
