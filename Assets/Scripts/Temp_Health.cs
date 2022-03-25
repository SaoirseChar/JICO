using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Temp_Health : MonoBehaviour
{
    public Image _happiness;
    public Image _hunger;
    public Image _fun;

    public TMP_Text happyText, funText, hungerText;

    public float happy, hungry, play;

    public int clickCount = 0;
    public ParticleSystem hearts;
    public ParticleSystem sleep;
    public AudioSource petNoise;

    
    [Header("Pet Patrol State")] 
    public Transform[] patrolSpots;
    public Transform lookPoint;
    private int randomSpot; //Choose a random position for the blob to go to
    private float waitTime;
    [Tooltip("How long blob pauses on each waypoint")]
    public float[] startWaitTime;

    [Header("Rotation & Movement")] 
    [HideInInspector] public float targetAngle;
    [HideInInspector] public float angle;
    public float moveSpeed;
    private float turnSmoothVelocity;
    public float turnSpeed;
    
    private Animator anim;
    public Material deadMat; 
    
    private void Start()
    {
        waitTime = Random.Range(0, startWaitTime.Length);

        anim = GetComponent<Animator>();

        //Set up the random points for the blob to walk to
        randomSpot = Random.Range(0, patrolSpots.Length);
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
        
        /*_happiness.fillAmount = happy;
        _hunger.fillAmount = hungry;
        _fun.fillAmount = play;*/

        happyText.text = _happiness.fillAmount.ToString();
        funText.text = _fun.fillAmount.ToString();
        hungerText.text = _hunger.fillAmount.ToString();
        
        //Smooth rotation between walk points
        targetAngle = Mathf.Atan2(patrolSpots[randomSpot].transform.position.x - transform.position.x,
            patrolSpots[randomSpot].transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (Input.GetKeyDown(KeyCode.H))
        {
            DecreaseHunger(0.01f);
            print(happy);
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            DecreaseHappiness(0.01f);
            print(hungry);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpdateHunger(0.01f);
            print(happy);
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            UpdateHappiness(0.01f);
            print(hungry);
        }

        #region MyRegion

        if (Input.GetMouseButtonDown(0))
        {
            Camera mainCam = Camera.main;
            Vector3 mousePos = Input.mousePosition;
            RaycastHit hitInfo;

            if(Physics.Raycast(mainCam.ScreenPointToRay(mousePos), out hitInfo))
            {
                if(hitInfo.transform.gameObject.CompareTag("Player"))
                {
                    clickCount++;
                    
                    if(clickCount >= 3)
                    {
                        //Play cute noise
                        PetNoises(petNoise);
                        UpdateHappiness(1); //Increase happiness
                        hearts.Play(); //Play hearts particles
                        clickCount = 0; //Reset click count
                        //Make Pet jump when happy
                        anim.Play("Test_Jump");
                    }
                }
            }
        }
        #endregion
    }
    
    private void Patrol()
    {
        transform.position = Vector3.MoveTowards(transform.position, patrolSpots[randomSpot].position,
            moveSpeed * Time.deltaTime);

        //If blob has moved to a random spot = wait a few seconds
        if (Vector3.Distance(transform.position, patrolSpots[randomSpot].position) < 0.1f)
        {
            //If wait time has passed
            if (waitTime <= 0)
            {
                //Move to another point
                randomSpot = Random.Range(0, patrolSpots.Length);

                //Randomise length of time spent on each point
                waitTime = Random.Range(0, startWaitTime.Length);
            }
            else if (waitTime >= 0)
            {
                //Make blob idle
                //anim.SetBool("canJump", false);

                anim.Play("Test_Idle");
                
                //Look at point when idle
                transform.LookAt(lookPoint);

                //Decrease countdown
                waitTime -= Time.deltaTime;
            }
        }
    }

    
    
    public void DecreaseHappiness(float sadIndex)
    {
        happy -= sadIndex;
        _happiness.fillAmount = happy;
        happy--;

        if(happy <= 0)
        {
            happy = 0;
            gameObject.GetComponent<MeshRenderer>().material = deadMat;
            transform.position = Vector3.zero; 
        }
    }
    
    public void DecreaseHunger(float hungerIndex)
    {
        hungry -= hungerIndex;
        _hunger.fillAmount = hungry;
        hungry--;

        if(hungry <= 0)
        {
            hungry = 0;
        }
    }
    
    public void UpdateHappiness(float happyIndex)
    {
        happy += happyIndex;
        _happiness.fillAmount = happy;
        happy++;

        if(happy >= 1)
        {
            happy = 1;
        }
    }
    
    public void UpdateHunger(float hungerIndex)
    {
        hungry += hungerIndex;
        _hunger.fillAmount = hungry;
        hungry++;

        if(hungry >= 1)
        {
            hungry = 1;
        }
    }
    
    public void PetNoises(AudioSource _clip)
    {
        _clip.Play();
    }
}
