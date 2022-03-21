using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// To use this script, attach it to the GameObject that you would like to rotate towards another game object.
/// After attaching it, go to the inspector and drag the GameObject you would like to rotate towards into the target field.
/// Move the target around in the scene view to see the GameObject continuously rotate towards it.
/// </summary>
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
        #region Quaternion Rotation

        // The step size is equal to speed times frame time.
        float step = speed * Time.deltaTime;

        // Rotate our transform a step closer to the target's.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, step);

        #endregion
    }

    /*private void FixedUpdate()
    {
        #region Vector Rotation
        
        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = speed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);
        
        #endregion
    }*/
}