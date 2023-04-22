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
    public List<gunStats> gunList = new();
    public MeshRenderer gunMaterial;
    public MeshFilter gunModel;
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.5f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDist;

    int selectedGun;
    int jumpedTimes;
    int HPOrig;
    bool groundedPlayer;
    bool isShooting;
    Vector3 move;
    Vector3 playerVelocity;
    private void Start()
    {
        HPOrig = HP;
        UIUpdate();
        Respawn();
    }

    void Update()
    {
        if (GameManager.Instance.activeMenu == null)
        {
            Movement();
            SelectGun();

            if (gunList.Count > 0 && !isShooting && Input.GetButton("Shoot"))
            {
                StartCoroutine(Shoot());
            }
        }

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

        controller.Move(playerSpeed * Time.deltaTime * move);

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

        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out RaycastHit hit, shootDist))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            damageable?.TakeDamage(shootDamage);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        Debug.Log("TakeDamage called");
        UIUpdate();

        if(HP <= 0) 
        {
            GameManager.Instance.OnDead();
        }
    }

    public void UIUpdate()
    {
        GameManager.Instance.HPBar.fillAmount = (float) HP / (float)HPOrig;
    }

    public void Respawn()
    {
        HP = HPOrig; 
        UIUpdate();
        controller.enabled = false;
        transform.position = GameManager.Instance.playerSpawnPos.transform.position;
        controller.enabled = true;
    }

    public void GunPickup(gunStats gunStat)
    {
        gunList.Add(gunStat);

        shootDamage = gunStat.shootDamage;
        shootDist = gunStat.shootDist;
        shootRate = gunStat.shootRate;

        gunModel.mesh = gunStat.model.GetComponent<MeshFilter>().sharedMesh;
        gunMaterial.material = gunStat.model.GetComponent<MeshRenderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;
    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            ChangeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            ChangeGun();
        }
    }

    void ChangeGun()
    {
        shootDamage = gunList[selectedGun].shootDamage;
        shootDist = gunList[selectedGun].shootDist;
        shootRate = gunList[selectedGun].shootRate;

        gunModel.mesh = gunList[selectedGun].model.GetComponent<MeshFilter>().sharedMesh;
        gunMaterial.material = gunList[selectedGun].model.GetComponent<MeshRenderer>().sharedMaterial;

        StopCoroutine(Shoot());
    }
}
