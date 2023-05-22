//using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DefenderBossAI : MonoBehaviour, IDamage
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
    [SerializeField] Transform grenadePrefab;
    [SerializeField] Transform grenadeLaunchPos;

    [Header("----- Enemy Stats -----")]
    [Range(0, 1000)][SerializeField] int HP;
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
    [Tooltip("This stops the bullet from shooting under the player")]
    [SerializeField] Vector3 playerPosOffset;
    [SerializeField, Range(0, 30)] int grenadeSpeed;
    [SerializeField] float grenadeFiringAngle = 45f;
    [SerializeField] float gravity = 9.8f;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] takeDamageSFX;
    [Range(0, 1)][SerializeField] float takeDamageSFXVolume;
    [SerializeField] AudioClip shootSFX;
    [Range(0, 1)][SerializeField] float shootSFXVolume;

    [SerializeField] bool takeCoverEnabled;

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
    public static event Action Dying;
    public static event Action Spawned;
    void Start()
    {
        stoppingDistOrig = agent.stoppingDistance;
        startingPos = transform.position;
        spawnFX.Play();
        originalHP = HP;
        TakingDamageFromPlayer += OtherAI_TakingDamageFromPlayer;
        GameManager.Instance.BossHasSpawned();
        Spawned?.Invoke();
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
                    { FacePlayer(); }

                    if (Mathf.Abs(angleToPlayer) <= 5f)
                    {
                        if (!isShooting)
                        {
                            if (HP >= originalHP * 0.5f)
                            {
                                StartCoroutine(Shoot());
                            }
                            else
                            {
                                StartCoroutine(LaunchGrenade());
                            }
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
        HP -= amount;
        audioSource.PlayOneShot(takeDamageSFX[UnityEngine.Random.Range(0, takeDamageSFX.Length)], takeDamageSFXVolume);
        bloodFX.Play();
        StartCoroutine(FlashColor());
        if (HP <= 0)
        {
            StopAllCoroutines();
            anim.SetBool("Dead", true);
            GetComponent<CapsuleCollider>().enabled = false;
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

    IEnumerator LaunchGrenade()
    {
        isShooting = true;
        Transform grenadeProjectileTransform = Instantiate(grenadePrefab, grenadeLaunchPos.position, Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.SetShooter(transform);
        audioSource.PlayOneShot(shootSFX, shootSFXVolume);
        /*grenadeProjectileTransform.position = grenadeLaunchPos.position;*/
        float targetDistance = Vector3.Distance(grenadeLaunchPos.position, GameManager.Instance.GetGrenadeTargetPos());
        float projectileVelocity = targetDistance / (Mathf.Sin(2 * grenadeFiringAngle * Mathf.Deg2Rad) / gravity);
        float Vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(grenadeFiringAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(grenadeFiringAngle * Mathf.Deg2Rad);
        float flightDuration = targetDistance / Vx;
        grenadeProjectileTransform.rotation = Quaternion.LookRotation(GameManager.Instance.GetGrenadeTargetPos() - grenadeProjectileTransform.position);

        float elapseTime = 0;

        while(elapseTime < flightDuration && grenadeProjectileTransform != null)
        {
                grenadeProjectileTransform.transform.Translate(0, (Vy - (gravity * elapseTime)) * Time.deltaTime, Vx * Time.deltaTime);

                elapseTime += Time.deltaTime;

                yield return null;
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        audioSource.PlayOneShot(shootSFX, shootSFXVolume);
        GameObject bulletClone = Instantiate(bullet, shootPos.position, bullet.transform.rotation);
        bulletClone.GetComponent<Rigidbody>().velocity = (playerDir + playerPosOffset) * bulletSpeed;
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
        GameManager.Instance.playerScript.SetShootenabled(false);
        agent.enabled = false;
        yield return new WaitForSeconds(5f);
        StartCoroutine(FadeDeath());
    }

    IEnumerator Hide(Transform _player)
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

    IEnumerator FadeDeath()
    {
        Dying?.Invoke();
        yield return null;
        Destroy(gameObject);
    }
}
