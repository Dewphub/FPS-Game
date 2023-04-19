using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    [SerializeField] gunStats gun;
    [SerializeField] MeshFilter model;
    [SerializeField] MeshRenderer mat;

    private void Start()
    {
        model.mesh = gun.model.GetComponent<MeshFilter>().sharedMesh;
        mat.material = gun.model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.playerScript.GunPickup(gun);
            Destroy(gameObject);
        }
    }
}
