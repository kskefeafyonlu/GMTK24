using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    private Camera mainCam;
    public float rotationSpeed = 35f; 

    private void Awake()
    {
        mainCam = Camera.main;
    }
    private void FixedUpdate()
    {
        LookAtMouse();
    }
    
    //Gun rotation logic
    private void LookAtMouse()
    {
        //get mouse position
        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        //get angle between pivot point and mouse
        float targetAngle = Mathf.Atan2(mouseWorldPos.y - transform.position.y, mouseWorldPos.x - transform.position.x) * Mathf.Rad2Deg ;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        //rotate towards mouse
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
