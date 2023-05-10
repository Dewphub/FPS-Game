using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public GameObject treasure;
    public Transform spawnPos;
    public bool isOpen;

    private Animator animator;
      
    void Start()
    {
       animator = GetComponent<Animator>(); 
    }

    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        isOpen = true;
        animator.SetTrigger("Open");
        
        Instantiate(treasure, spawnPos.position, Quaternion.identity);
    }
}
