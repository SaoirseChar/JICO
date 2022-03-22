using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // The target marker.
    public Transform target;

    // Angular speed in radians per sec.
    public float speed = 1.0f;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        #region Rotation

        float step = speed * Time.deltaTime;
        
        //Rotation angle of target
        Quaternion rotationTarget = Quaternion.LookRotation(target.position - transform.position);

        //Rotate toward target
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationTarget, step);
        
        #endregion
    }
}