using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour, IDamage
{
    [Header("Turret Stats")]
    [SerializeField, Range(1f, 50f)] float attackDistance;
    [SerializeField, Range(10f, 60f)] float bulletSpeed;
    [SerializeField, Range(0f, 1f)] float shootRate;
    [SerializeField, Range(0.01f, 1f)] float turnSpeed;
    [SerializeField] int HP;

    [Header("Components")]
    [SerializeField] Animator animator;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] GameObject[] particles;


    GameObject player;
    Transform playerTransform;
    bool isShooting;
    bool isExploding;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }
    void Update() 
    { 

        Vector3 attackDirection = playerTransform.position - transform.position;
        float angleToPlayerY = Vector3.Angle(new(0f, transform.position.y, 0f), new(0f, playerTransform.position.y, 0f));
        if (Vector3.Distance(playerTransform.position, transform.position) <= attackDistance && angleToPlayerY <= 10f) 
        {
            transform.forward = Vector3.Lerp(transform.forward, new(attackDirection.x, 0, attackDirection.z), turnSpeed * Time.fixedDeltaTime); 

            if (!isShooting)
            {
                StartCoroutine(ShootPlayer());
            }
            else
            {
                animator.SetBool("isShooting", false);
            }
        }
        
    }

    IEnumerator ShootPlayer()
    {
        isShooting = true;
        yield return new WaitForSeconds(shootRate);
        animator.SetBool("isShooting", true);
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        Debug.Log("TakeDamage called on Turret");
        if(HP <= 0)
        {
            StartCoroutine(ProcessExplosion());
        }
    }

    IEnumerator ProcessExplosion()
    {
        isExploding = true;
        if (isExploding)
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
