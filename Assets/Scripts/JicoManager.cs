using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class JicoManager : MonoBehaviour
{
    public int clickCount = 0;
    public ParticleSystem hearts;
    public AudioSource petNoise;
    public ParticleSystem sleep;
    public string petName;

    [Header("Pet Patrol State")] 
    public float _happiness;
    public float _hunger;
    public Transform sickPosition;
    public Transform[] patrolSpots;
    public Transform lookPoint;
    private int randomSpot; //Choose a random position for the blob to go to
    private float waitTime;

    [Tooltip("How long blob pauses on each waypoint")]
    public float[] startWaitTime;

    [Header("Rotation & Movement")] 
    [HideInInspector]
    public float targetAngle;

    [HideInInspector] public float angle;
    public float moveSpeed;
    private float turnSmoothVelocity;
    public float turnSpeed;

    private Animator anim;
    public Material sickMat;
    public Material hungryMat;
    public Material newMat;
    
    //This will be used to measure how much time has passed since game has been played
    //for updating the hunger & happiness bars
    private bool serverTime;
    private static readonly int CanJump = Animator.StringToHash("canJump");

    #region PROPERTIES
    public float Hunger
    {
        get { return _hunger; }
        set { _hunger = value; }
    }

    public float Happiness
    {
        get { return _happiness; }
        set { _happiness = value; }
    }

    public string Name
    {
        get { return petName; }
        set { petName = value; }
    }
    #endregion
    
    private void Start()
    {
        //anim = GetComponent<Animator>();

        waitTime = Random.Range(0, startWaitTime.Length);

        //Set up the random points for the blob to walk to
        randomSpot = Random.Range(0, patrolSpots.Length);
        
        UpdateStats();

        if(!PlayerPrefs.HasKey("petName"))
        {
            PlayerPrefs.SetString("petName", "Pet");
            petName = PlayerPrefs.GetString("petName");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Smooth rotation between walk points
        targetAngle = Mathf.Atan2(patrolSpots[randomSpot].transform.position.x - transform.position.x,
            patrolSpots[randomSpot].transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        #region MyRegion

        if (Input.GetMouseButtonDown(0))
        {
            Camera mainCam = Camera.main;
            Vector3 mousePos = Input.mousePosition;
            RaycastHit hitInfo;

            if (Physics.Raycast(mainCam.ScreenPointToRay(mousePos), out hitInfo))
            {
                if (hitInfo.transform.gameObject.CompareTag("Jico"))
                {
                    clickCount++;

                    if (clickCount >= 2)
                    {
                        //Play cute noise
                        PetNoises(petNoise);
                        Health.instance.Heal(10f); //Increase happiness
                        hearts.Play(); //Play hearts particles
                        clickCount = 0; //Reset click count
                        //Make Pet jump when happy
                    }
                }
            }
        }

        #endregion
        
        if (Health.instance.health < 50)
        {
            StartCoroutine(LowHealthJico());
        }
        else
        {
            Patrol();
            StopCoroutine(LowHealthJico());
        }
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
                //Walk animation

                //Move to another point
                randomSpot = Random.Range(0, patrolSpots.Length);

                //Randomise length of time spent on each point
                waitTime = Random.Range(0, startWaitTime.Length);
            }
            else if (waitTime >= 0)
            {
                //Make blob idle

                //Look at point when idle
                transform.LookAt(lookPoint);

                //Decrease countdown
                waitTime -= Time.deltaTime;
            }
        }
    }

    private IEnumerator LowHealthJico()
    {
        //Move Jico to sad spot
        transform.position = Vector3.MoveTowards(transform.position, sickPosition.transform.position, moveSpeed * Time.deltaTime);

        //Do sick animation (shiver eg) OR Green material
        gameObject.GetComponent<MeshRenderer>().material = sickMat;
        //anim.Play("Sick");

        //Wait until health is above 50%
        yield return new WaitUntil((() => Health.instance.health >= 50f));

        yield return new WaitForSeconds(15f);

        //Go back to patrolling
        Patrol();
    }

    public void PetNoises(AudioSource _clip)
    {
        _clip.Play();
    }
    
    #region MANAGES PET STATS OVER TIME - HAPPINESS, HUNGER AND FUN
    //Use PlayerPrefs to save stats
    private void UpdateStats()
    {
        #region Hunger Stat
        if (!PlayerPrefs.HasKey("hunger"))
        {
            _hunger = 100;
            PlayerPrefs.SetFloat("hunger", _hunger);
        }
        else
        {
            _hunger = PlayerPrefs.GetFloat("hunger");
        }
        #endregion

        #region Happiness Stat
        if (!PlayerPrefs.HasKey("happiness"))
        {
            _happiness = 100;
            PlayerPrefs.SetFloat("happiness", _happiness);
        }
        else
        {
            _happiness = PlayerPrefs.GetFloat("happiness");
        }
        #endregion

        #region Using TimeSpan to alter hunger and happiness value 
        TimeSpan ts = GetTimeSpan();
        _hunger -= (int)(ts.TotalHours * 2); //Every hour will subtract 2 points from hunger
        if (_hunger < 0)
        {
            _hunger = 0;
        }

        _happiness -= (int)((100 - _hunger) * (ts.TotalHours / 5));
        if(_happiness < 0)
        {
            _happiness = 0;
        }
        #endregion

        if (!PlayerPrefs.HasKey("then"))
        {
            PlayerPrefs.SetString("then", GetStringTime());
        }
        
        Debug.Log(GetTimeSpan().ToString()); //TESTING

        if (serverTime)
        {
            
        }
        else
        {
            InvokeRepeating(nameof(UpdateDevice), 0f, 130f); //Every 60 sec will save the time when close game. Then when player opens again, time will be based on 60 secs before game was closed.
        }
    }

    /// <summary>
    /// This function will allow access of devices time in order to alter hunger and happiness based
    /// on how much time has passed in the game
    /// </summary>
    public void UpdateDevice()
    {
        PlayerPrefs.SetString("then", GetStringTime());
    }

    /// <summary>
    /// Object that is the result of two time subtractions
    /// </summary>
    /// <returns></returns>
    TimeSpan GetTimeSpan()
    {
        if(serverTime)
        {
            return new TimeSpan();
        }
        else
        {
            return DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("then"));
        }
    }

    private string GetStringTime()
    {
        DateTime now = DateTime.Now; //Accessing current time on device
        return now.Day + "/" + now.Month + "/" + now.Year + " " + now.Hour + ":" + now.Minute;
    }
    #endregion


    #region SAVE PET INFO
    /// <summary>
    /// Update Device Time and save all info (hunger and happiness)
    /// </summary>
    public void SavePetInfo()
    {
        if(!serverTime)
        {
            UpdateDevice();
            PlayerPrefs.SetFloat("hunger", Hunger);
            PlayerPrefs.SetFloat("happiness", Happiness);
            PlayerPrefs.SetString("name", Name);
        }
    }
    #endregion
    
}