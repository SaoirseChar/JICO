using UnityEngine.UI;
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
    public Material hungryMat;
    public Material newMat; 
    
    private void Start()
    {
        //anim = GetComponent<Animator>();

        waitTime = Random.Range(0, startWaitTime.Length);

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

        happyText.text = "Love: " + happy;
        funText.text = "Fun: " + play;
        hungerText.text = "Hunger: " + hungry;
        
        //Smooth rotation between walk points
        targetAngle = Mathf.Atan2(patrolSpots[randomSpot].transform.position.x - transform.position.x,
            patrolSpots[randomSpot].transform.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (Input.GetKeyDown(KeyCode.H))
        {
            DecreaseHunger(10f);
            print(happy);
        }
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            DecreaseHappiness(10f);
            print(hungry);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpdateHunger(10f);
            print(happy);
        }
        
        if (Input.GetKeyDown(KeyCode.O))
        {
            UpdateHappiness(10f);
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
                    
                    if(clickCount >= 2)
                    {
                        //Play cute noise
                        PetNoises(petNoise);
                        UpdateHappiness(5); //Increase happiness
                        hearts.Play(); //Play hearts particles
                        clickCount = 0; //Reset click count
                        //Make Pet jump when happy
                        //anim.SetTrigger("jump");
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
        if (Vector3.Distance(transform.position, patrolSpots[randomSpot].position) < 0.2f)
        {
            //If wait time has passed
            if (waitTime <= 0)
            {
                //anim.SetInteger("Walk", 1);
                
                //Move to another point
                randomSpot = Random.Range(0, patrolSpots.Length);

                //Randomise length of time spent on each point
                waitTime = Random.Range(0, startWaitTime.Length);
            }
            else if (waitTime >= 0)
            {
                //Make blob idle
                //anim.SetTrigger("Idle");
                
                //Stop walking
                //anim.SetInteger("Walk", 0);

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
            transform.position = transform.position; 
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
            gameObject.GetComponent<MeshRenderer>().material = hungryMat;
        }
    }
    
    public void UpdateHappiness(float happyIndex)
    {
        happy += happyIndex;
        _happiness.fillAmount = happy;
        happy++;

        if(happy >= 100)
        {
            happy = 100;
            gameObject.GetComponent<MeshRenderer>().material = newMat;
        }
    }
    
    public void UpdateHunger(float hungerIndex)
    {
        hungry += hungerIndex;
        _hunger.fillAmount = hungry;
        hungry++;

        if(hungry >= 100)
        {
            hungry = 100;
            gameObject.GetComponent<MeshRenderer>().material = newMat;
        }
    }
    
    public void PetNoises(AudioSource _clip)
    {
        _clip.Play();
    }
}
