using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    // The target marker.
    private Transform target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Jico").transform;
    }

    // Update is called once per frame
    void Update()
    {
        #region Rotation
        
        var lookDir = target.position - transform.position;
        lookDir.y = 0; // keep only the horizontal direction
        transform.rotation = Quaternion.LookRotation(lookDir);
        #endregion
    }
}