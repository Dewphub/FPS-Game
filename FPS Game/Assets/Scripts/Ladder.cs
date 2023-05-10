using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{

   private bool isClimbing = false; 
   //[SerializeField] GameObject climber; 
   //[SerializeField] float climbSpeed;

    //float climbAmount;
    //float verticalInput;



    private void Update()
    {
       
/*        if (!isClimbing) 
            return;

        verticalInput = Input.GetAxis("Vertical");
        
        if (verticalInput == 0) 
            return;

        climbAmount = verticalInput * climbSpeed * Time.deltaTime;
        Debug.Log("Vertical Input = " + verticalInput);
        climber.transform.Translate(Vector3.up * climbAmount);*/
    }

    private void FixedUpdate()
    {
       
/*        if (climber != null && GetComponent<Collider>().bounds.Contains(climber.transform.position))
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }*/
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            /*climber = other.gameObject;*/
            isClimbing = true;
            GameManager.Instance.playerScript.SetIsOnLadder(isClimbing);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            isClimbing = false;
            GameManager.Instance.playerScript.SetIsOnLadder(isClimbing);
/*            climber = null;
            isClimbing = false;*/
        }
    }

}
