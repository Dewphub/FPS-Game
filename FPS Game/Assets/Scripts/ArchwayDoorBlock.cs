using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchwayDoorBlock : MonoBehaviour
{
    [SerializeField] float lerpDuration;

    Vector3 doorBlockOffset;
    Vector3 doorPosOriginal;
    float blockTimeElapsed;
    float openTimeElapsed;
    private void Start()
    {
        doorBlockOffset = new Vector3(0, -2f, 0);
        doorPosOriginal = transform.localPosition;
    }

    private void Update()
    {
        if(GameManager.Instance.GetEnemiesRemaining() > 0)
        {
            openTimeElapsed = 0;
            ProcessBlockPath();
        }
        else if(transform.localPosition != doorPosOriginal && GameManager.Instance.GetEnemiesRemaining() == 0)
        {
            blockTimeElapsed = 0;
            ProcessOpenPath();
        }
    }
    private void ProcessBlockPath()
    {
        Vector3 newDoorPos = doorPosOriginal + doorBlockOffset;
        if (blockTimeElapsed < lerpDuration)
        {
            transform.localPosition = Vector3.Lerp(doorPosOriginal, newDoorPos, blockTimeElapsed / lerpDuration);
            blockTimeElapsed += Time.deltaTime;
        }
    }

    private void ProcessOpenPath()
    {
        if (openTimeElapsed < lerpDuration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, doorPosOriginal, openTimeElapsed / lerpDuration);
            openTimeElapsed += Time.deltaTime;
        }
    }


}
