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

    GameObject player;
    Transform playerTransform;
    float elapsedTime;
    bool isExploding;

    bool isAttacking;
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
        if(HP <= 0)
        {
            StartCoroutine(ProcessExplosion());
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
            IDamage damageable = collision.gameObject.GetComponent<IDamage>();
            damageable?.TakeDamage(5);
            StartCoroutine(ProcessExplosion());
        }
    }

    IEnumerator ProcessExplosion()
    {
        isExploding = true;
        if(isExploding)
        { 
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
            isExploding = false;
        }
        Destroy(gameObject);
    }
}
