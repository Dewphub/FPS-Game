using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
