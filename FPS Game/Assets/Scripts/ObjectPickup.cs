using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [SerializeField] Transform carryPos;
    [SerializeField] float carryDist;

    private GameObject carriedObject;
    private bool isCarrying = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isCarrying)
            {
                PlaceObject();
            }
            else
            {
                PickUpObject();
            }
        }

        if (isCarrying)
        {
            UpdateCarryPosition();
        }
    }

    void PickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, carryDist))
        {
            if (hit.transform.CompareTag("Pickup"))
            {
                isCarrying = true;
                carriedObject = hit.transform.gameObject;
                carriedObject.GetComponent<Rigidbody>().isKinematic = true;
                carriedObject.transform.position = carryPos.position;
                carriedObject.transform.parent = carryPos;
            }
        }
    }

    void PlaceObject()
    {
        isCarrying = false;
        carriedObject.transform.parent = null;
        carriedObject.GetComponent<Rigidbody>().isKinematic = false;
        carriedObject = null;
    }

    void UpdateCarryPosition()
    {
        carriedObject.transform.position = carryPos.position;
    }
}
