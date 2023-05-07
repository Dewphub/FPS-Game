using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System;
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
    [SerializeField] SphereCollider coverChecker;
    [SerializeField] LayerMask hideLayer;

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
    public Collider[] colliders = new Collider[10];
    bool playerInRange;
    bool isShooting;
    bool destinationChosen;
    bool coverTaken;
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
        playerDir = (GameManager.Instance.player.transform.position - headPos.position);
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                if((float)HP / originalHP >= 0.5f || coverTaken)
                {
                    agent.stoppingDistance = stoppingDistOrig;
                    agent.SetDestination(GameManager.Instance.player.transform.position);

                    if (agent.remainingDistance < agent.stoppingDistance)
                    { FacePlayer(); }

                    if (Mathf.Abs(angleToPlayer) <= 5f)
                    {
                        if (!isShooting)
                            StartCoroutine(Shoot());
                    }
                }
                else
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
        HP -= amount;
        audioSource.PlayOneShot(takeDamageSFX[UnityEngine.Random.Range(0, takeDamageSFX.Length)], takeDamageSFXVolume);
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
            TakingDamageFromPlayer(this, EventArgs.Empty);
            anim.SetTrigger("Damage");
            agent.SetDestination(GameManager.Instance.player.transform.position);
            agent.stoppingDistance = 0;
        }
    }
    IEnumerator Roam()
    {
        if (!destinationChosen && agent.remainingDistance < 0.05f)
        {
            destinationChosen = true;
            agent.stoppingDistance = 0;

            yield return new WaitForSeconds(roamPauseTime);

            Vector3 ranPos = UnityEngine.Random.insideUnitSphere * roamDist;
            ranPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);

            agent.SetDestination(hit.position);
            destinationChosen = false;
        }
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
    IEnumerator FlashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    IEnumerator OnDead()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    IEnumerator Hide(Transform _player)
    {
        if(!destinationChosen)
        {
            int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, coverChecker.radius, colliders, hideLayer);

            for(int i = 0; i < hits; i++)
            {
                if (NavMesh.SamplePosition(colliders[i].transform.position, out NavMeshHit hit, 2f, agent.areaMask))
                {
                    if(!NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
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
                            if(!NavMesh.FindClosestEdge(hit2.position, out hit2, agent.areaMask))
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
