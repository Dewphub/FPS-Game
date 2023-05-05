using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovementPlatform : MonoBehaviour
{
    [SerializeField] float frequency;
    [SerializeField] float xAmplitude;

    Vector3 currPosition;
    // Start is called before the first frame update
    void Start()
    {
        currPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newX = xAmplitude * Mathf.Sin(Time.time * frequency) + currPosition.x;
        transform.position = new(newX, currPosition.y, currPosition.z);
    }
}
