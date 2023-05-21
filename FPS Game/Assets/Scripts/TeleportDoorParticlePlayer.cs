using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDoorParticlePlayer : MonoBehaviour
{
    [SerializeField] GameObject[] particles;
    private void FixedUpdate()
    {
        if(GameManager.Instance.GetEnemiesRemaining() > 0)
        {
            for(int i = 0; i < particles.Length; i++)
            {
                particles[i].gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0;i < particles.Length; i++)
            {
                particles[i].gameObject.SetActive(true);
            }
        }
    }
}
