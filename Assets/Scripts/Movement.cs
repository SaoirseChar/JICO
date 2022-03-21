using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;
    private float turnSmoothVelocity;
    public float turnSpeed;

    private Animator animalAnimator;

    [Header("Pet Patrol State")] public Transform[] patrolSpots;
    public Transform lookPoint;
    private int randomSpot; //Choose a random position for the blob to go to
    private float waitTime;

    [Tooltip("How long blob pauses on each waypoint")]
    public float[] startWaitTime;

    [Header("Angle")] 
    [HideInInspector] public float targetAngle;
    [HideInInspector] public float angle;

    // Start is called before the first frame update
    void Start()
    {
        waitTime = Random.Range(0, startWaitTime.Length);

        //animalAnimator = GetComponent<Animator>();

        //Set up the random points for the blob to walk to
        randomSpot = Random.Range(0, patrolSpots.Length);
    }

    // Update is called once per frame
    void Update()
    {
        //Smooth rotation between walk points
        targetAngle = Mathf.Atan2(patrolSpots[randomSpot].transform.position.x - transform.position.x,
            patrolSpots[randomSpot].transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Patrol();
    }

    private void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, patrolSpots[randomSpot].position,
            moveSpeed * Time.deltaTime);

        //If blob has moved to a random spot = wait a few seconds
        if (Vector3.Distance(transform.position, patrolSpots[randomSpot].position) < 0.2f)
        {
            //If wait time has passed
            if (waitTime <= 0)
            {
                //Walk animation between points
                //animalAnimator.SetBool("IsWalking", true);

                //Move to another point
                randomSpot = Random.Range(0, patrolSpots.Length);

                //Randomise length of time spent on each point
                waitTime = Random.Range(0, startWaitTime.Length);
            }
            else
            {
                //Make blob idle
                //animalAnimator.SetBool("IsWalking", false);

                //Look at point when idle
                transform.LookAt(lookPoint);

                //Decrease countdown
                waitTime -= Time.deltaTime;
            }
        }
    }
}