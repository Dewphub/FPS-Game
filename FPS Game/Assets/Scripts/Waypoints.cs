using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
  public Transform GetWaypoint(int index)
    {
        return transform.GetChild(index);
    }

    public int GetNextIndex(int curr)
    {
        int next = curr + 1;
        
        if(next == transform.childCount) 
        {
        next = 0;
        }

        return next;
    }

}
