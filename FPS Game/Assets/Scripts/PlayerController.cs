using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, IDamage, IDataPersistence
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] Recoil recoil;
    [SerializeField] Aim newAimPos;
    [SerializeField] GameObject muzzleFlashObject;

    [Header("----- Player Stats -----")]
    [Range(1, 10)] [SerializeField] int HP;
    [Range(3, 8)][SerializeField] float playerSpeed;
    [Range(1.5f, 5)][SerializeField] float sprintMod;
    [Range(8, 25)][SerializeField] float jumpHeight;
    [Range(10, 50)][SerializeField] float gravityValue;
    [Range(1, 3)][SerializeField] int jumpsMax;
    [Range(0, 1)][SerializeField] float aimSnap;

    [Header("----- Gun Stats -----")]
    public List<gunStats> gunList = new List<gunStats>();
    public MeshRenderer gunMaterial;
    public MeshFilter gunModel;
    [Range(1, 10)][SerializeField] int shootDamage;
    [Range(0.5f, 5)][SerializeField] float shootRate;
    [Range(1, 100)][SerializeField] int shootDist;
    [Range(0, 0.5f)][SerializeField] float muzzleFlashFX;

    [Header("----- Audio -----")]
    [SerializeField] AudioClip[] audSteps;
    [SerializeField][Range(0, 1)] float audStepsVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField][Range(0, 1)] float audJumpVol;
    [SerializeField] AudioClip[] audDamage;
    [SerializeField][Range(0, 1)] float audDamageVol;
    [SerializeField] AudioClip replenishHP;
    [SerializeField] [Range(0, 1)] float replenishHPVol;
    [SerializeField] AudioClip gunPickupSFX;
    [SerializeField] [Range(0, 1)] float gunPickupSFXVolume;

    int selectedGun;
    int jumpedTimes;
    int HPOrig; 
    float aimTimeElapsed;
    float gunResetTimeElapsed;


    bool groundedPlayer;
    bool isSprinting;
    bool isPlayingSteps;
    bool isShooting;

    Vector3 playerVelocity;
    Vector3 move;

    private void Start()
    {
        HPOrig = HP;
        UIUpdate();
        Respawn();
        //Testing Aim
    }

    void Update()
    {
        Sprint();

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

    private void OnTriggerEnter(Collider other)
    {
        String triggerTag = other.tag;

        switch(triggerTag)
        {
            case "Health":
            aud.PlayOneShot(replenishHP, replenishHPVol);
            HP = HPOrig;
            UIUpdate();
            Destroy(other.gameObject);
                break;
        }
    }

    void Movement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer)
        {
            if (!isPlayingSteps && move.normalized.magnitude > 0.5f)
            {
                StartCoroutine(PlaySteps());
            }

            if (playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
                jumpedTimes = 0;
            }
        }

        move = (transform.right * Input.GetAxis("Horizontal")) +
            (transform.forward * Input.GetAxis("Vertical"));

        controller.Move(playerSpeed * Time.deltaTime * move);

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && jumpedTimes < jumpsMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            playerVelocity.y = jumpHeight;
            jumpedTimes++;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void Sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            playerSpeed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            playerSpeed /= sprintMod;
        }
    }

    IEnumerator PlaySteps()
    {
        isPlayingSteps = true;

        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (!isSprinting)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.3f);

        isPlayingSteps = false;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        StartCoroutine(MuzzleFlash());
        recoil.RecoilFire();
        aud.PlayOneShot(gunList[selectedGun].gunShotAud, gunList[selectedGun].gunShotAudVol);
        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out RaycastHit hit, shootDist))
        {
            IDamage damageable = hit.collider.GetComponent<IDamage>();
            damageable?.TakeDamage(shootDamage);
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    IEnumerator MuzzleFlash()
    {
        muzzleFlashObject.SetActive(true);
        yield return new WaitForSeconds(muzzleFlashFX);
        muzzleFlashObject.SetActive(false);
    }
    public void TakeDamage(int amount)
    {
        aud.PlayOneShot(audDamage[Random.Range(0, audDamage.Length)], audDamageVol);
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
        DataPersitenceManager.Instance.LoadGame();
        controller.enabled = true;
    }

    public void GunPickup(gunStats gunStat)
    {
        Debug.Log("GunPickup Entered");
        gunList.Add(gunStat);
        Debug.Log("gunList added");
        aud.PlayOneShot(gunPickupSFX, gunPickupSFXVolume);

        shootDamage = gunStat.shootDamage;
        shootDist = gunStat.shootDist;
        shootRate = gunStat.shootRate;

        gunModel.mesh = gunStat.model.GetComponent<MeshFilter>().sharedMesh;
        gunMaterial.material = gunStat.model.GetComponent<MeshRenderer>().sharedMaterial;

        selectedGun = gunList.Count - 1;
        UpdateMuzzleFlashLocation(gunList[selectedGun]);
        newAimPos.SetGunAimPos(gunList[selectedGun].gunAimPos);
        recoil.UpdateGun(gunList[selectedGun]);

    }

    private void UpdateMuzzleFlashLocation(gunStats gun)
    {
        muzzleFlashObject.transform.localPosition = gunList[selectedGun].muzzleFlashPos;
    }

    void SelectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            newAimPos.SetGunAimPos((gunList[selectedGun].gunAimPos));
            recoil.UpdateGun(gunList[selectedGun]);
            ChangeGun();
            
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            newAimPos.SetGunAimPos((gunList[selectedGun].gunAimPos));
            recoil.UpdateGun(gunList[selectedGun]);
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

        UpdateMuzzleFlashLocation(gunList[selectedGun]);
        StopCoroutine(Shoot());
    }

    public bool GetIsShooting()
    {
        return isShooting;
    }

    public void LoadData(GameData data)
    {
        this.gunList = data.gunList;
        this.selectedGun = data.selectedGun;
        GameManager.Instance.playerSpawnPos.transform.position = data.playerPos;
        transform.position = data.playerPos;
        ChangeGun();
    }

    public void SaveData(ref GameData data)
    {
        data.gunList = this.gunList;
        data.selectedGun = this.selectedGun;
        data.playerPos = GameManager.Instance.playerSpawnPos.transform.position;
    }
}
