using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Tooltip("How long until the platform goes away")]
    [SerializeField] float waitToDestroy = 5f;

    [Tooltip("How long until the platform starts to flash.")]
    [SerializeField] float timeToBeginFlashing = 1f;

    [Tooltip("Set smaller to flash color faster once it starts flashing")]
    [SerializeField] float colorSwitchDelay = 0.5f;

    [Tooltip("Child object material")]
    [SerializeField] Material mat;

    private GameObject currPlatform;
    private bool isFlashing;

    private void Awake()
    {
        GameManager.PlayerHasDied += PlayerDied;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currPlatform = other.gameObject.GetComponentInParent<GameObject>();
            if (!isFlashing)
            {
                StartCoroutine(FlashColor());
            }
            StartCoroutine(Disappear());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isFlashing && other.gameObject.GetComponentInParent<GameObject>() == currPlatform)
        {
            StopFlashing();
        }
    }

    private IEnumerator Disappear()
    {
        isFlashing = true;
        yield return new WaitForSeconds(timeToBeginFlashing);

        StartCoroutine(FlashColor());
        yield return new WaitForSeconds(waitToDestroy);

        isFlashing = false;
        mat.color = Color.white;
        Destroy(gameObject);
    }

    private IEnumerator FlashColor()
    {
        while (isFlashing)
        {
            if (mat.color == Color.white)
            {
                mat.color = Color.red;
            }
            else
            {
                mat.color = Color.white;
            }

            yield return new WaitForSeconds(colorSwitchDelay);
        }
    }

    private void PlayerDied()
    {
        mat.color = Color.white;
    }

    private void StopFlashing()
    {
        isFlashing = false;
        currPlatform.GetComponent<Renderer>().material.SetFloat("_EmissionIntensity", 0f);
    }
}