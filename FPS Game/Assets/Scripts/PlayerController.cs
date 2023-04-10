using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [Range(1, 10)][SerializeField] int HP;
    [Range(3, 8)][SerializeField] float playerSpeed;
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(10, 50)][SerializeField] float gravityValue;
    [Range(1, 3)][SerializeField] int jumpsMax;

    [Header("----- Gun Stats -----")]
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.5f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDist;

    int jumpedTimes;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    bool isShooting;
    Vector3 move;
    int HPOrig;

    public static event Action hasDied;
    private void Start()
    {
        HPOrig = HP;
    }

    void Update()
    {
        Movement();
        if (!isShooting && Input.GetButton("Shoot")) StartCoroutine(Shoot());
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            jumpedTimes = 0;
        }

        move = (transform.right * Input.GetAxis("Horizontal")) +
            (transform.forward * Input.GetAxis("Vertical"));

        controller.Move(move * Time.deltaTime * playerSpeed);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if (damageable != null)
            {
                damageable.TakeDamage(shootDamage);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        if(HP <= 0) 
        {
            hasDied?.Invoke();
        }
    }
}
