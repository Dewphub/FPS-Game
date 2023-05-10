using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Tooltip("How long until the platform goes away")]
    [SerializeField] float waitToDestroy;

    [Tooltip("How long until the platform starts to flash.")]
    [SerializeField] float timeToBeginFlashing;

    [Tooltip("Set smaller to flash color faster once it starts flashing")]
    [SerializeField] float colorSwitchDelay;
    
    [Tooltip("Child object material")]
    [SerializeField] Material mat;

    bool isFlashing;

    private void Awake()
    {
        GameManager.PlayerHasDied += PlayerDied;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected");
        Debug.Log("other gametag" + other.gameObject.tag);
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Disappear());
        }
    }
    IEnumerator Disappear()
    {
        Debug.Log("Coroutine entered");
        isFlashing = true;
        InvokeRepeating("FlashColor", timeToBeginFlashing, colorSwitchDelay);
        yield return new WaitForSeconds(waitToDestroy);
        isFlashing = false;
        mat.color = Color.white;
        Destroy(gameObject);
    }
    
    void FlashColor()
    {
        if(isFlashing)
        {
            if(mat.color == Color.white)
            {
                mat.color = Color.red;
            }
            else
            {
                mat.color = Color.white;
            }
        }
    }

    void PlayerDied()
    {
        mat.color = Color.white;
    }
}
