//using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System;
using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Rigidbody))]
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
    [SerializeField] SphereCollider coverChecker;
    [SerializeField] LayerMask hideLayer;
    [SerializeField] GameObject[] loot;
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField] SphereCollider headshotCollider;

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

    [SerializeField] bool takeCoverEnabled;

    Vector3 playerDir;
    Vector3 playerHeadDir;
    Vector3 startingPos;
    Vector3 courseCorrectionDir;
    public Collider[] colliders = new Collider[10];
    bool playerInRange;
    bool isShooting;
    bool destinationChosen;
    bool coverTaken;
    bool isDead;
    float angleToPlayer;
    float stoppingDistOrig;
    float speed;
    int originalHP;

    public static event EventHandler TakingDamageFromPlayer;
    void Start()
    {
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        spawnFX.Play();
        originalHP = HP;
        TakingDamageFromPlayer += OtherAI_TakingDamageFromPlayer;
        //GameManager.Instance.UpdateGameGoal(1);
        rb = gameObject.GetComponent<Rigidbody>();
        shootRate = UnityEngine.Random.Range(0.5f, 2f);
    }

    private void OtherAI_TakingDamageFromPlayer(object sender, EventArgs e)
    {
        if (sender as GameObject != this)
        {
            agent.SetDestination(GameManager.Instance.player.transform.position);
            agent.stoppingDistance = 0;
        }
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
    void FacePlayer()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }
    bool CanSeePlayer()
    {
        playerDir = (GameManager.Instance.player.transform.position - headPos.position).normalized;
        playerHeadDir = (GameManager.Instance.playerHead.transform.position - headPos.position).normalized;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                if ((float)HP / originalHP >= 0.5f || coverTaken || !takeCoverEnabled)
                {
                    agent.stoppingDistance = stoppingDistOrig;
                    agent.SetDestination(GameManager.Instance.player.transform.position);

                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        FacePlayer();
                    }
                    if (Mathf.Abs(angleToPlayer) <= 5f)
                    {
                        if (!isShooting)
                        {
                            StartCoroutine(Shoot());
                        }
                    }
                }
                else if (takeCoverEnabled)
                {
                    StartCoroutine(Hide(GameManager.Instance.player.transform));
                }
                return true;
            }

        }
        return false;
    }
    public void TakeDamage(int amount)
    {
        if(!isDead)
        {
        HP -= amount;
        audioSource.PlayOneShot(takeDamageSFX[UnityEngine.Random.Range(0, takeDamageSFX.Length)], takeDamageSFXVolume);
        bloodFX.Play();
        StartCoroutine(FlashColor());
        }
        if (HP <= 0 && !isDead)
        {
            isDead = true;
            agent.isStopped = true;
            TakingDamageFromPlayer -= OtherAI_TakingDamageFromPlayer;
            StopAllCoroutines();
            Debug.Log("Enemy ID: " + gameObject.name + " died");
            GameManager.Instance.UpdateGameGoal(-1);
            DataPersistenceManager.Instance.ModifyEnemiesKilled(1);
            anim.SetBool("Dead", true);
            GetComponent<CapsuleCollider>().enabled = false;
            agent.enabled = false;
            StartCoroutine(OnDead());
        }
        else
        {
            TakingDamageFromPlayer?.Invoke(this, EventArgs.Empty);
            anim.SetTrigger("Damage");
            agent.SetDestination(GameManager.Instance.player.transform.position);
            agent.stoppingDistance = 0;
        }
    }
    IEnumerator Roam()
    {
        if (!destinationChosen && !isDead && agent.remainingDistance < 0.05f)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamPauseTime);

            Vector3 ranPos = UnityEngine.Random.insideUnitSphere * roamDist;
            ranPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
            if(!isDead)
            {
                agent.SetDestination(hit.position);
                destinationChosen = false;
            }
        }
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        audioSource.PlayOneShot(shootSFX, shootSFXVolume);
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = playerDir.normalized * bulletSpeed;
        Bullet bulletMessenger = bulletClone.GetComponent<Bullet>();
        bulletMessenger.SetShooter(transform);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    IEnumerator FlashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator OnDead()
    {
        headshotCollider.enabled = false;
        capsuleCollider.enabled = false;
        agent.GetComponent<Collider>().enabled = false;
        agent.enabled = false;
        rb.Sleep();
        yield return new WaitForSeconds(1f);
        GameObject lootDrop = Instantiate(loot[UnityEngine.Random.Range(0, loot.Length)], transform.position + new Vector3(0f, 0.75f, 0f), Quaternion.identity);
        StartCoroutine(FadeDeath(true));
    }

    IEnumerator Hide(Transform _player)
    {
        if (!isDead)
        {
            if (!destinationChosen)
            {
                int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, coverChecker.radius, colliders, hideLayer);

                for (int i = 0; i < hits; i++)
                {
                    if (NavMesh.SamplePosition(colliders[i].transform.position, out NavMeshHit hit, 2f, agent.areaMask))
                    {
                        if (!NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
                        {
                            Debug.LogError("Unable to find edge close to " + hit.position);
                        }
                        if (Vector3.Dot(hit.normal, (GameManager.Instance.player.transform.position - hit.position).normalized) <= 0f)
                        {
                            destinationChosen = true;
                            agent.stoppingDistance = 1f;
                            animTransSpeed += 10f;
                            agent.SetDestination(hit.position);
                            yield return new WaitForSeconds(3);
                            coverTaken = true;
                            Quaternion.LookRotation(playerDir);
                            destinationChosen = false;
                        }
                        else
                        {
                            if (NavMesh.SamplePosition(colliders[i].transform.position - (GameManager.Instance.player.transform.position - hit.position).normalized * 2f,
                                out NavMeshHit hit2, 2f, agent.areaMask))
                            {
                                if (!NavMesh.FindClosestEdge(hit2.position, out hit2, agent.areaMask))
                                {
                                    Debug.LogError("Unable to find edge close to " + hit2.position + " (second attempt)");
                                }
                                if (Vector3.Dot(hit2.normal, (GameManager.Instance.player.transform.position - hit2.position).normalized) <= 0f)
                                {
                                    destinationChosen = true;
                                    agent.stoppingDistance = 1f;
                                    animTransSpeed += 10f;
                                    agent.SetDestination(hit2.position);
                                    yield return new WaitForSeconds(3f);
                                    Quaternion.LookRotation(playerDir);
                                    coverTaken = true;
                                    destinationChosen = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    IEnumerator FadeDeath(bool _toFade)
    {
        Vector3 deathPosition = transform.position - new Vector3(0, 2, 0);
        while(_toFade)
        {
            if (transform.position.y >= deathPosition.y)
            {
                transform.position -= new Vector3(0, 0.5f * Time.deltaTime, 0);
                yield return null;
            }
            else
            {
                _toFade = false;
            }
        }
        Destroy(gameObject);
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }
}
