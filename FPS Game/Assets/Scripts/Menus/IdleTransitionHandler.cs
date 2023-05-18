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
    public void ChangeAnimationState(int stateNum)
    {
        switch(stateNum)
        {
            case 0:
                anim.SetInteger("IdleState", 0);
                if(!isTransitioning)
                {
                    StartCoroutine(ChangeState());
                }
                break;
            case 1:
                anim.SetInteger("IdleState", 1);
                if (!isTransitioning)
                {
                    StartCoroutine(ChangeState());
                }
                break;
            case 2:
                anim.SetInteger("IdleState", 2);
                if (!isTransitioning)
                {
                    StartCoroutine(ChangeState());
                }
                break;
            case 3:
                anim.SetInteger("IdleState", 3);
                if (!isTransitioning)
                {
                    StartCoroutine(ChangeState());
                }
                break;
            default:
                anim.SetInteger("IdleState", -1);
                if (!isTransitioning)
                {
                    StartCoroutine(ChangeState());
                }
                break;
        }
    }

    IEnumerator ChangeState()
    {
        isTransitioning = true;
        int animationPhase = Random.Range(0, 4);
        ChangeAnimationState(animationPhase);
        yield return new WaitForSeconds(transitionDelayTime);
        isTransitioning = true;
    }
}
