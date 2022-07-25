using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraRecentre : MonoBehaviour{
    private CinemachineFreeLook camera;

    private void Start(){
        camera = GetComponent<CinemachineFreeLook>();
    }

    private void Update(){
        if (Input.GetAxis("CameraResentre") == 1){
            camera.m_RecenterToTargetHeading.m_enabled = true;
        }
        else{
            camera.m_RecenterToTargetHeading.m_enabled = false;
        }
    }
}