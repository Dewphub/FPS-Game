using System.Collections;
using UnityEngine;

public class TurretAim : MonoBehaviour
{
    [SerializeField] float speed;

    GameObject player;

    Coroutine LookCoroutine;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void LateUpdate()
    {
        StartRotating();
    }
    void StartRotating()
    {
        if(LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt());
    }

    IEnumerator LookAt()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        lookRotation = Quaternion.Euler(lookRotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        float time = 0;

        while(time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);

            time += Time.deltaTime * speed;

            yield return null;
        }
    }
}
