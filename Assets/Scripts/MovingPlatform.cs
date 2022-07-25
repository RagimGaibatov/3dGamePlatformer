using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour{
    [SerializeField] private WayPointPath _wayPointPath;

    [SerializeField] private float speed;

    private int _targetWaypointIndex;

    private Transform _previosWaypoint;
    private Transform _targetWaypoint;

    private float _timeToWaypoint;
    private float _elapsedTime;

    void Start(){
        TargetNextWayPoint();
    }

    void FixedUpdate(){
        _elapsedTime += Time.deltaTime;

        float elapsedPercentage = _elapsedTime / _timeToWaypoint;
        elapsedPercentage = Mathf.SmoothStep(0, 1, elapsedPercentage);
        transform.position = Vector3.Lerp(_previosWaypoint.position, _targetWaypoint.position, elapsedPercentage);
        transform.rotation = Quaternion.Lerp(_previosWaypoint.rotation, _targetWaypoint.rotation, elapsedPercentage);

        if (elapsedPercentage >= 1){
            TargetNextWayPoint();
        }
    }

    private void TargetNextWayPoint(){
        _previosWaypoint = _wayPointPath.GetWayPoint(_targetWaypointIndex);
        _targetWaypointIndex = _wayPointPath.GetNextWayPointIndex(_targetWaypointIndex);
        _targetWaypoint = _wayPointPath.GetWayPoint(_targetWaypointIndex);

        _elapsedTime = 0;

        float distanceTowayPoint = Vector3.Distance(_previosWaypoint.position, _targetWaypoint.position);
        _timeToWaypoint = distanceTowayPoint / speed;
    }

    private void OnTriggerEnter(Collider other){
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other){
        other.transform.SetParent(null);
    }
}