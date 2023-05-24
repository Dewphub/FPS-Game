using System;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] Transform gunHipPosTransform;
    [SerializeField] Transform gunAimPosTransform;
    [SerializeField] float aimSnap;
    [SerializeField] Camera cam;
    [SerializeField] Camera weaponCam;
    [SerializeField] float camAnimationSpeed;

    float aimTimeElapsed;
    float resetTimeElapsed;

    Vector3 gunHipPos;
    Vector3 gunAimPos;
    Quaternion gunHipRot;
    Quaternion gunAimRot;

    bool canAim;
    bool isAiming;
    Transform gunPos;
    float originalFOV;

    private void Start()
    {
        gunHipPos = gunHipPosTransform.localPosition;
        gunAimPos = gunAimPosTransform.localPosition;
        gunHipRot = gunHipPosTransform.localRotation;
        gunAimRot = gunAimPosTransform.localRotation;
        gunPos = transform;
        originalFOV = cam.fieldOfView;
        isAiming = false;

    }
    private void Update()
    {
        if(Input.GetMouseButton(1) && controller.gunList.Count > 0 && canAim)
        {
            resetTimeElapsed = 0;
            controller.GetAimPos();
            gunStats activeGun = controller.GetSelectedGun();
            GunPosAim();
            SetFieldOfView(Mathf.Lerp(cam.fieldOfView, activeGun.fieldOfView, camAnimationSpeed * Time.deltaTime));
        }
        else if(controller.gunList.Count > 0)
        {
            aimTimeElapsed = 0;
            GunPosReset();
            SetFieldOfView(Mathf.Lerp(cam.fieldOfView, originalFOV, camAnimationSpeed * Time.deltaTime));
        }
    }
    public void GunPosAim()
    {
        if (aimTimeElapsed < aimSnap)
        {
            isAiming = true;
            gunPos.localPosition = Vector3.Lerp(gunHipPos, gunAimPos, (aimTimeElapsed / aimSnap));
            gunPos.localRotation = Quaternion.Slerp(gunHipRot, gunAimRot, (aimTimeElapsed / aimSnap));
            aimTimeElapsed += Time.deltaTime;
        }
    }

    public void GunPosReset()
    {
        if (resetTimeElapsed < aimSnap)
        {
            isAiming = false;
            gunPos.localPosition = Vector3.Lerp(gunAimPos, gunHipPos, resetTimeElapsed / aimSnap);
            gunPos.localRotation = Quaternion.Slerp(gunAimRot, gunHipRot, resetTimeElapsed / aimSnap);
            resetTimeElapsed += Time.deltaTime;
        }
    }

    public bool GetIsAiming()
    {
        return isAiming;
    }

    public void SetCanAim(bool _canAim)
    {
        canAim = _canAim;
    }
    public void SetGunAimPos(Vector3 _gunAimPos)
    {
        gunAimPos = _gunAimPos;
    }

    private void SetFieldOfView(float fov)
    {
        cam.fieldOfView = fov;
        weaponCam.fieldOfView = fov;

    }
}
