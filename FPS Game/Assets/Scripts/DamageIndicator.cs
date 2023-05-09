using System;
using System.Collections;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private const float MAX_TIMER = 8f;
    private float timer = MAX_TIMER;

    private CanvasGroup canvasGroup = null;
    protected CanvasGroup Canvas_Group
    {
        get
        {
            if (canvasGroup == null)
            {
                TryGetComponent<CanvasGroup>(out canvasGroup);
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            return canvasGroup;
        }
    }

    private RectTransform rect = null;
    protected RectTransform Rect
    {
        get
        {
            if (rect == null)
            {
                TryGetComponent<RectTransform>(out rect);
                if (rect == null)
                {
                    rect = gameObject.AddComponent<RectTransform>();
                }
            }
            return rect;
        }
    }

    public Transform Target { get; protected set; } = null;
    private Transform player = null;

    private IEnumerator IE_CountDown = null;
    private Action unRegister = null;

    private Quaternion targetRotation = Quaternion.identity;
    private Vector3 targetPosition = Vector3.zero;

    public void Register(Transform _target, Transform _player, Action _unRegister)
    {
        Target = _target;
        player = _player;
        unRegister = _unRegister;

        StartCoroutine(RotateToTarget());
        StartTimer();

    }
    void StartTimer()
    { 
        if(IE_CountDown != null)
        {
            StopCoroutine(IE_CountDown);
        }
        IE_CountDown = CountDown();
        StartCoroutine(IE_CountDown);
    }

    private IEnumerator CountDown()
    {
        while(Canvas_Group.alpha < 1.0f)
        {
            Canvas_Group.alpha += 4 * Time.deltaTime;
            yield return null;
        }
        while(timer > 0f)
        {
            timer--;
            yield return new WaitForSeconds(1);
        }
        while(Canvas_Group.alpha > 0.0f)
        {
            Canvas_Group.alpha -= 2 * Time.deltaTime;
            yield return null;
        }
        unRegister();
        Destroy(gameObject);
    }

    IEnumerator RotateToTarget()
    {
        while(enabled)
        {
            if(Target != null)
            {
                targetPosition = Target.position;
                targetRotation = Target.rotation;
            }
            Vector3 direction = (player.position - targetPosition).normalized;
            targetRotation = Quaternion.LookRotation(direction);
            targetRotation.z = -targetRotation.y;
            targetRotation.x = 0;
            targetRotation.y = 0;

            Vector3 northDirection = new(0, 0, player.eulerAngles.y);
            Rect.localRotation = targetRotation * Quaternion.Euler(northDirection);

            yield return null;
        }
    }

    public void Restart()
    {
        timer = MAX_TIMER;
        StartTimer();
    }

}
