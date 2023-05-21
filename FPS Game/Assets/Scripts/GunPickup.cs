using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] gunStats gun;
    [SerializeField] MeshFilter model;
    [SerializeField] MeshRenderer mat;

    private bool isPickedUp;

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
            GameManager.Instance.playerScript.secrets++;
            isPickedUp = true;
            DataPersistenceManager.Instance.ModifySecrets(1);
            this.gameObject.SetActive(false);
        }
    }

    public void LoadData(GameData data)
    {
        data.gunsPickedUp.TryGetValue(id, out isPickedUp);
        if (isPickedUp)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.gunsPickedUp.ContainsKey(id))
        {
            data.gunsPickedUp.Remove(id);
        }
        data.gunsPickedUp.Add(id, isPickedUp);
    }
}
