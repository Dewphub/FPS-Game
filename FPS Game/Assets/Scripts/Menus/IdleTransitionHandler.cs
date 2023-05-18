using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTransitionHandler : MonoBehaviour
{
    [SerializeField] int transitionDelayTime;
    [SerializeField] Animator anim;

    bool isTransitioning;
    private void Start()
    {
        StartCoroutine(ChangeState());
    }

    IEnumerator ChangeState()
    {
        while (true)
        {
            int lastState = anim.GetInteger("IdleState");
            while (anim.GetInteger("IdleState") == lastState)
                anim.SetInteger("IdleState", Random.Range(0, 4));
            yield return new WaitForSeconds(transitionDelayTime);
            anim.SetTrigger("IdleFinished");
        }
    }
}
