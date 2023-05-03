using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField] PlayerController player;

    Vector3 currentRotation;
    Vector3 targetRotation;

    float recoilX;
    float recoilY;
    float recoilZ;

    float snappiness;
    float returnSpeed;
    

    void Update()
    {
        if(player.GetIsShooting())
        {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        }
    }

    public void RecoilFire()
    {
        targetRotation += new Vector3(recoilX, Random.Range(recoilY, recoilY), Random.Range(recoilZ, recoilZ));
    }

    public void UpdateGun(gunStats gun)
    {
        recoilX = gun.recoilX;
        recoilY = gun.recoilY;
        recoilZ = gun.recoilZ;

        returnSpeed = gun.returnSpeed;
        snappiness = gun.snappiness;
    }
}
