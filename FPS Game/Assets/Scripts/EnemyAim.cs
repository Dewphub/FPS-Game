using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAim : MonoBehaviour
{
    GameObject playerDir;

    private void Start()
    {
        playerDir = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
       transform.position = playerDir.transform.position;
    }
}
