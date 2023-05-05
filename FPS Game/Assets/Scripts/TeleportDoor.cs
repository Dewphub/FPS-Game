using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDoor : MonoBehaviour
{
    [SerializeField] Transform door1;
    [SerializeField] Transform door2;
    [SerializeField] GameObject trigger1;
    [SerializeField] GameObject trigger2;
    [SerializeField] GameObject teleportTrigger1;
    [SerializeField] GameObject teleportTrigger2;
    [SerializeField] float lerpDuration;

    [SerializeField] Vector3 localYOffset;

    Vector3 door1PosOriginal;
    Vector3 door2PosOriginal;

    float openTimeElapsed;
    float closeTimeElapsed;

    bool isOpening;
    bool isClosing;

    private void Update()
    {
        if (isOpening)
        {
            closeTimeElapsed = 0;
            ProcessOpenDoors();
        }
        else if (isClosing)
        {
            openTimeElapsed = 0;
            ProcessCloseDoors();
        }
    }
    private void Start()
    {
        door1PosOriginal = door1.localPosition;
        door2PosOriginal = door2.localPosition;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isOpening = true;
            if(gameObject == trigger1)
            {
                teleportTrigger1.SetActive(true);
            }
            else
            {
                teleportTrigger2.SetActive(true);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isClosing = true;
            if(gameObject == trigger1)
            {
                teleportTrigger1.SetActive(false);
            }
            else
            {
                teleportTrigger2.SetActive(false);
            }
        }
    }

    public void ProcessOpenDoors()
    {
        Vector3 newDoor1Pos = door1PosOriginal + localYOffset;
        Vector3 newDoor2Pos = door2PosOriginal + localYOffset;

        if (openTimeElapsed < lerpDuration)
        {
            door1.transform.localPosition = Vector3.Lerp(door1.transform.localPosition, newDoor1Pos, openTimeElapsed / lerpDuration);
            door2.transform.localPosition = Vector3.Lerp(door2.transform.localPosition, newDoor2Pos, openTimeElapsed / lerpDuration);
            openTimeElapsed += Time.deltaTime;
        }
        else if(openTimeElapsed  >= lerpDuration)
        {
            isOpening = false;
        }
    }

    public void ProcessCloseDoors()
    {
        if (closeTimeElapsed < lerpDuration)
        {
            door1.transform.localPosition = Vector3.Lerp(door1.transform.localPosition, door1PosOriginal, closeTimeElapsed / lerpDuration);
            door2.transform.localPosition = Vector3.Lerp(door2.transform.localPosition, door2PosOriginal, closeTimeElapsed / lerpDuration);
            closeTimeElapsed += Time.deltaTime;
        }
        else if(closeTimeElapsed >= lerpDuration)
        {
            isClosing = false;
        }
    }
}
