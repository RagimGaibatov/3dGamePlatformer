using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointPath : MonoBehaviour{
    public Transform GetWayPoint(int wayPointIndex){
        return transform.GetChild(wayPointIndex);
    }

    public int GetNextWayPointIndex(int currentWaypointIndex){
        int nextWapointIndex = currentWaypointIndex + 1;
        if (nextWapointIndex == transform.childCount){
            nextWapointIndex = 0;
        }

        return nextWapointIndex;
    }
}