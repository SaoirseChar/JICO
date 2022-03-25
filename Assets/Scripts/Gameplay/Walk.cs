using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Walk : MonoBehaviour
{
    private NavMeshAgent nav;
    private Animator anim;
    private int destination = 0;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        nav.autoBraking = false; //No breaking at all
    }

    // Update is called once per frame
    void Update()
    {
        if (_GameManager.instance != null)
        {
            if (_GameManager.instance.clickCount == 0) //If not being clicked on, move around the floor
            {
                //Walk animation
                anim.SetInteger("Walk", 1);
                //Debug.Log("Pet is walking");
                GoToNextPoint();
            }
            else
            {
                //Stop walking
                anim.SetInteger("Walk", 0);
            }
        }
    }

    public void GoToNextPoint()
    {
        Debug.Log("Pet is moving to next point");
        //If no more points to travel to, return
        if (_GameManager.instance != null)
        {
            if (Vector3.Distance(_GameManager.instance.walkPoints[destination].transform.position, transform.position) < 0.5f)
            {
                destination++;

                if (destination >= _GameManager.instance.walkPoints.Length)
                {
                    destination = 0;
                }
            }
            nav.destination = Vector3.MoveTowards(transform.position, _GameManager.instance.walkPoints[destination].transform.position, Time.deltaTime * nav.speed);
        }
    }
}
