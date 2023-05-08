using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{

   private bool isClimbing = false; 
   [SerializeField] GameObject climber; 
   [SerializeField] float climbSpeed;

    float climbAmount;
    float verticalInput;


    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            climber = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
 
        if (other.gameObject == climber)
        {
            climber = null;
            isClimbing = false;
        }
    }

    private void Update()
    {
       
        if (!isClimbing) 
            return;

        verticalInput = Input.GetAxis("Vertical");
        
        if (verticalInput == 0) 
            return;

        climbAmount = verticalInput * climbSpeed * Time.deltaTime;

        climber.transform.Translate(Vector3.up * climbAmount);
    }

    private void FixedUpdate()
    {
       
        if (climber != null && GetComponent<Collider>().bounds.Contains(climber.transform.position))
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
    }
}
