using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] PlayerController controller;
    [SerializeField] Transform gunHipPosTransform;
    [SerializeField] Transform gunAimPosTransform;
    [SerializeField] float aimSnap;

    float aimTimeElapsed;
    float resetTimeElapsed;

    Vector3 gunHipPos;
    Vector3 gunAimPos;
    Quaternion gunHipRot;
    Quaternion gunAimRot;

    bool isAiming;
    Transform gunPos;

    private void Start()
    {
        gunHipPos = gunHipPosTransform.localPosition;
        gunAimPos = gunAimPosTransform.localPosition;
        gunHipRot = gunHipPosTransform.localRotation;
        gunAimRot = gunAimPosTransform.localRotation;
        gunPos = transform;

    }
    private void Update()
    {
        if(Input.GetMouseButton(1) && controller.gunList.Count > 0)
        {
            resetTimeElapsed = 0;
            GunPosAim();
        }
        else if(controller.gunList.Count > 0)
        {
            aimTimeElapsed = 0;
            GunPosReset();
        }
    }
    public void GunPosAim()
    {
        if (aimTimeElapsed < aimSnap)
        {
            isAiming = true;
            gunPos.localPosition = Vector3.Lerp(gunHipPos, gunAimPos, aimTimeElapsed / aimSnap);
            gunPos.localRotation = Quaternion.Slerp(gunHipRot, gunAimRot, aimTimeElapsed / aimSnap);
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
}
