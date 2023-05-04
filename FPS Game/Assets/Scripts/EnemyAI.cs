using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;
    [SerializeField] Transform shootPos;
    [SerializeField] AudioSource audioSource;
    [SerializeField] ParticleSystem spawnFX;
    [SerializeField] ParticleSystem bloodFX;

    [Header("----- Enemy Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(1, 10)][SerializeField] int playerFaceSpeed;
    [Range(0, 179)][SerializeField] int sightAngle;
    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;
    [SerializeField] float animTransSpeed;
    [SerializeField] float rotationSpeed;

    [Header("----- Gun Stats -----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.5f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [Range(1, 100)][SerializeField] int bulletSpeed;
    [SerializeField] GameObject bullet;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] takeDamageSFX;
    [Range(0, 1)] [SerializeField] float takeDamageSFXVolume;
    [SerializeField] AudioClip shootSFX;
    [Range(0, 1)][SerializeField] float shootSFXVolume;

    Vector3 playerDir;
    Vector3 startingPos;
    bool playerInRange;
    bool isShooting;
    bool destinationChosen;
    float angleToPlayer;
    float stoppingDistOrig;
    float speed;
    void Start()
    {
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        spawnFX.Play();
    }
    void Update()
    {
        if (agent.isActiveAndEnabled) 
        {
            speed = Mathf.Lerp(speed, agent.velocity.normalized.magnitude, Time.deltaTime * animTransSpeed);
            anim.SetFloat("Speed", speed);

            if (playerInRange && !CanSeePlayer())
            {
                StartCoroutine(Roam());
            }
            else if (agent.destination != GameManager.Instance.player.transform.position)
                StartCoroutine(Roam());
        }
    }

    IEnumerator Roam()
    {
        if (!destinationChosen && agent.remainingDistance < 0.05f)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamPauseTime);

            Vector3 ranPos = Random.insideUnitSphere * roamDist;
            ranPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);

            agent.SetDestination(hit.position);
            destinationChosen = false;
        }
    }

    bool CanSeePlayer()
    {
        playerDir = (GameManager.Instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(GameManager.Instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                    FacePlayer();

                if (!isShooting)
                    StartCoroutine(Shoot());

                return true;
            }
        }
        return false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        audioSource.PlayOneShot(shootSFX, shootSFXVolume);
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed; //Always shoots toward player
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }

    IEnumerator FlashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        audioSource.PlayOneShot(takeDamageSFX[Random.Range(0, takeDamageSFX.Length)], takeDamageSFXVolume);
        bloodFX.Play();
        StartCoroutine(FlashColor());
        if (HP <= 0)
        {
            StopAllCoroutines();
            GameManager.Instance.UpdateGameGoal(-1);
            anim.SetBool("Dead", true);
            GetComponent<CapsuleCollider>().enabled = false;
            agent.enabled = false;
            StartCoroutine(OnDead());
        }
        else
        {
            anim.SetTrigger("Damage");
            agent.SetDestination(GameManager.Instance.player.transform.position);
            agent.stoppingDistance = 0;
        }
    }

    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    IEnumerator OnDead()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
