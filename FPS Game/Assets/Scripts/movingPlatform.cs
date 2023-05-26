using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [SerializeField] Waypoints waypoints;

    [SerializeField] float speed;

    int index;

    Transform prev;
    Transform target;

    float time;
    float elapse;

    float distancetoWaypoint;
    float elapsedPrecent;

    Vector3 gunRelativePosition;
    Quaternion gunRelativeRotation;
    Quaternion camHolderRelativeRotation;
    // Start is called before the first frame update
    void Start()
    {
        TargetNextwaypoint();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapse += Time.deltaTime;

        elapsedPrecent = elapse / time;
        elapsedPrecent = Mathf.SmoothStep(0,1,elapsedPrecent);
        transform.position = Vector3.Lerp(prev.position, target.position, elapsedPrecent);
        transform.rotation = Quaternion.Lerp(prev.rotation, target.rotation, elapsedPrecent);

        if (elapsedPrecent >= 1)
        {
            TargetNextwaypoint();
        }
    }

    private void TargetNextwaypoint()
    {
        prev = waypoints.GetWaypoint(index);
        index = waypoints.GetNextIndex(index);
        target = waypoints.GetWaypoint(index);

        elapse = 0;

        distancetoWaypoint = Vector3.Distance(prev.position, target.position);
        time = distancetoWaypoint / speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

            other.transform.SetParent(transform);
            gunRelativePosition = other.transform.Find("CamHolder").Find("Main Camera").Find("GunPos").transform.localPosition;
            camHolderRelativeRotation = other.transform.Find("CamHolder").transform.localRotation;
            gunRelativeRotation = other.transform.Find("CamHolder").Find("Main Camera").Find("GunPos").transform.localRotation;


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.transform.Find("CamHolder").Find("Main Camera").Find("GunPos").transform.localPosition = gunRelativePosition;
            other.transform.Find("CamHolder").Find("Main Camera").Find("GunPos").transform.localRotation = gunRelativeRotation;
            camHolderRelativeRotation = other.transform.Find("CamHolder").transform.localRotation = camHolderRelativeRotation;
            other.transform.SetParent(null);
            other.transform.localScale = new(1, 1, 1);
        }
    }
}
