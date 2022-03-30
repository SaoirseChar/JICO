using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

public class JicoManager : MonoBehaviour
{
    public int clickCount = 0;
    public ParticleSystem hearts, evolve;
    public AudioSource petNoise;
    public ParticleSystem sleep;
    public string petName;

    [Header("Pet Patrol State")] 
    public int _happiness, _hunger;
    public Transform sickPosition;
    public Transform[] patrolSpots;
    public Transform lookPoint;
    private int randomSpot; //Choose a random position for the blob to go to
    private float waitTime;

    [Tooltip("How long blob pauses on each waypoint")]
    public float[] startWaitTime;

    [Header("Rotation & Movement")] [HideInInspector]
    public float targetAngle;

    [HideInInspector] public float angle;
    public float moveSpeed;
    private float turnSmoothVelocity;
    public float turnSpeed;

    private Animator anim;
    public Material sickMat;
    public Material hungryMat;
    public Material newMat;
    private Mesh mesh;

    [Header("Jico Evolution")] public GameObject evolvePanel;
    [SerializeField] private GameObject jicoAdult;

    //This will be used to measure how much time has passed since game has been played
    //for updating the hunger & happiness bars
    private bool serverTime;

    #region PROPERTIES

    public int Hunger
    {
        get { return _hunger; }
        set { _hunger = value; }
    }

    public int Happiness
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
        mesh = GetComponent<Mesh>();
        PlayerPrefs.SetString("then", "30/04/2022 5:04:00");
        UpdateStatus();

        //anim = GetComponent<Animator>();

        waitTime = Random.Range(0, startWaitTime.Length);

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(EvolveJico());
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Health.instance.Damage(25);
        }

        if (Health.instance.health <= Health.instance.maxHealth / 2)
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
        transform.position = Vector3.MoveTowards(transform.position, sickPosition.transform.position,
            moveSpeed * Time.deltaTime);

        //Do sick animation (shiver eg) OR Green material
        gameObject.GetComponent<MeshRenderer>().material = sickMat;
        //anim.Play("Sick");

        //Wait until health is above 50%
        yield return new WaitUntil((() => Health.instance.health >= 50f));

        gameObject.GetComponent<MeshRenderer>().material = newMat;

        //Go back to patrolling
        Patrol();
    }

    public void PetNoises(AudioSource _clip)
    {
        _clip.Play();
    }

    private void UpdateStatus()
    {
        if (!PlayerPrefs.HasKey("_hunger"))
        {
            _hunger = 100;
            PlayerPrefs.SetInt("_hunger", _hunger);
        }
        else
        {
            _hunger = PlayerPrefs.GetInt("_hunger");
        }

        if (!PlayerPrefs.HasKey("_happiness"))
        {
            _happiness = 100;
            PlayerPrefs.SetInt("_happiness", _happiness);
        }
        else
        {
            _happiness = PlayerPrefs.GetInt("_happiness");
        }

        if (!PlayerPrefs.HasKey("then"))
        {
            PlayerPrefs.SetString("then", GetStringTime());
        }

        TimeSpan ts = GetTimeSpan();

        //For every hour you are not in game, subtract 2 points from hunger
        _hunger -= (int)(ts.TotalHours * 2);
        if (_hunger < 0)
            _hunger = 0;
        //Compared to how hungry the Jico is, the happiness will also decrease as well
        _happiness -= (int)((100 - _hunger) * (ts.TotalHours / 5));
        if (_happiness < 0)
            _happiness = 0;

        //Debug.Log(GetTimeSpan().ToString());

        if (serverTime)
            UpdateServer();
        else
            InvokeRepeating(nameof(UpdateDevice), 0f, 30f);
    }

    public void UpdateServer()
    {
    }

    public void UpdateDevice()
    {
        PlayerPrefs.SetString("then", GetStringTime());
    }

    /// <summary>
    /// Subtract two DateTimes from each other to calculate
    /// </summary>
    /// <returns></returns>
    TimeSpan GetTimeSpan()
    {
        if (serverTime)
            return new TimeSpan();
        else
            return DateTime.Now - Convert.ToDateTime(PlayerPrefs.GetString("then"));
    }

    /// <summary>
    /// Convert string time to DateTime
    /// </summary>
    /// <returns></returns>
    private string GetStringTime()
    {
        DateTime now = DateTime.Now;
        return now.Month + "/" + now.Day + "/" + now.Year + " " + now.Hour + ":" + now.Minute + ":" + now.Second;
    }

    public void FeedJico()
    {
    }

    private IEnumerator EvolveJico()
    {
        //Move Jico to 4th spot
        transform.position = Vector3.MoveTowards(transform.position, patrolSpots[4].position,
            moveSpeed * Time.deltaTime);

        evolve.Play();

        MeshRenderer myMesh = gameObject.GetComponent<MeshRenderer>();
        myMesh = jicoAdult.GetComponent<MeshRenderer>();

        yield return new WaitUntil(() => (evolve.isStopped));

        evolvePanel.SetActive(true);

        yield return new WaitForSeconds(10);

        //Go back to patrolling
        Patrol();
    }
}

