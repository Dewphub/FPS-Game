using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float snappiness;
    public float returnSpeed;
    public GameObject model;
    public AudioClip gunShotAud;
    [Range(0, 1)] public float gunShotAudVol;
    public Vector3 gunAimPos;
    public Vector3 muzzleFlashPos;
    public Texture gunIcon;
    public float fieldOfView;
    public int clipSize;
    public int maxClips;

    int remainingClipAmount;
    int remainingAmmo;
    int ammoToBeReloaded;
    public int CalcMaxAmmo()
    {
    return clipSize * maxClips;
    }

    public void UseAmmo()
    {
        if (remainingClipAmount > 0)
        {
            remainingClipAmount--;
        }
        else return;
    }

    public int GetRemainingAmmo() { return remainingAmmo; }
    public int GetRemainingClipAmount(){ return remainingClipAmount; }

    public void SetDefaultGunStats()
    {
        remainingClipAmount = clipSize;
        remainingAmmo = clipSize;
    }

    public void CalcReload()
    {
        //Check to see if player has ammo to reload with
        if(remainingAmmo > 0)
        {
            //if there is some, check to see if there's enough for a full clip
            if(remainingAmmo >= clipSize)
            {
                ammoToBeReloaded = clipSize - remainingClipAmount;
            }
            else
            {
                ammoToBeReloaded = remainingAmmo;
            }
            
            remainingClipAmount += ammoToBeReloaded;
            remainingAmmo -= ammoToBeReloaded;
        }
        else { return ; }
    }

    public void PickedUpGunAsAmmo()
    {
        if (remainingAmmo < CalcMaxAmmo())
        {
            if (clipSize + remainingAmmo < CalcMaxAmmo())
            {
                remainingAmmo += clipSize;
            }
            else
            {
                remainingAmmo = CalcMaxAmmo();
            }
        }
        else { return; }
    }

    public void PickedUpAmmoBox(AmmoStats ammo)
    {
        if (remainingAmmo == CalcMaxAmmo()) { return; }
        else if(remainingAmmo  + ammo.ammoAmount < CalcMaxAmmo())
        {
            remainingAmmo += ammo.ammoAmount;
        }
        else
        {
            remainingAmmo = CalcMaxAmmo();
        }
    }
}
