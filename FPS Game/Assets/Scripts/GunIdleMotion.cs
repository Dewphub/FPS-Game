using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunIdleMotion : MonoBehaviour
{
    [SerializeField] float heightChangeSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float height;

    Vector3 currPosition;

    private void Start()
    {
        currPosition = transform.position;
    }

    private void Update()
    {
        float newY = height * Mathf.Cos(Time.time * heightChangeSpeed) + currPosition.y;
        transform.position = new(currPosition.x, newY, currPosition.z);
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
