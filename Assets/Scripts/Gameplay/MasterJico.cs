using UnityEngine;
using System;
using UnityEngine.UI;

public class MasterJico : MonoBehaviour
{
    #region Pet Stats Variables
    [Header("Pet Stats")]
    private float hunger;
    private float happiness;
    private float fun;
    private string petName;

    #region Animations
    [Header("Animations")]
    public Animator anim;
    public ParticleSystem hearts;
    public ParticleSystem sleep;
    public AudioSource petNoise;
    #endregion

    [Header("Other")]
    private Vector3 screenBounds;

    //Where you release your finger when up or down
    private Vector2 fingerDownPos;
    private Vector2 fingerUpPos;
    private bool detectSwipeAfterRelease = false;

    //[SerializeField] private float minSwipeDistance;

    //public static event Action<SwipeData> OnSwipe = delegate { };

    //This will be used to measure how much time has passed since game has been played
    //for updating the hunger, happiness and fun bars
    private bool serverTime;
    private static readonly int CanJump = Animator.StringToHash("canJump");

    #endregion

    #region PROPERTIES
    public float Hunger
    {
        get { return hunger; }
        set { hunger = value; }
    }

    public float Happiness
    {
        get { return happiness; }
        set { happiness = value; }
    }

    public float Fun
    {
        get { return fun; }
        set { fun = value; }
    }

    public string Name
    {
        get { return petName; }
        set { petName = value; }
    }
    #endregion

    #region INITIALISE
    // Start is called before the first frame update
    private void Start()
    {
        hearts.Stop();
        anim = GetComponent<Animator>();
        UpdateStats();

        if(!PlayerPrefs.HasKey("petName"))
        {
            PlayerPrefs.SetString("petName", "Pet");
            petName = PlayerPrefs.GetString("petName");
        }
    }
    #endregion

    #region UPDATE
    private void Update()
    {  
        #region PC Input Controls - Click to increase happiness
        if (Input.GetMouseButtonDown(0))
        {
            Camera mainCam = Camera.main;
            Vector3 mousePos = Input.mousePosition;
            RaycastHit hitInfo;

            if(Physics.Raycast(mainCam.ScreenPointToRay(mousePos), out hitInfo))
            {
                if(hitInfo.transform.gameObject.CompareTag("Jico"))
                {
                    _GameManager.instance.clickCount++;
                    
                    if(_GameManager.instance.clickCount >= 2)
                    {
                        //Play cute noise
                        PetNoises(petNoise);
                        UpdateHappiness(5); //Increase happiness
                        hearts.Play(); //Play hearts particles
                        _GameManager.instance.clickCount = 0; //Reset click count
                        //Make Pet jump when happy
                        //anim.SetBool(CanJump, true);
                    }
                }
            }
        }
        #endregion
    }
    #endregion

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
            hunger = 100;
            PlayerPrefs.SetFloat("hunger", hunger);
        }
        else
        {
            hunger = PlayerPrefs.GetFloat("hunger");
        }
        #endregion

        #region Happiness Stat
        if (!PlayerPrefs.HasKey("happiness"))
        {
            happiness = 100;
            PlayerPrefs.SetFloat("happiness", happiness);
        }
        else
        {
            happiness = PlayerPrefs.GetFloat("happiness");
        }
        #endregion

        #region Fun Stat

        if (!PlayerPrefs.HasKey("fun"))
        {
            fun = 100;
            PlayerPrefs.SetFloat("fun", fun);
        }
        else
        {
            fun = PlayerPrefs.GetFloat("fun");
        }
        #endregion

        #region Using TimeSpan to alter hunger and happiness value 
        TimeSpan ts = GetTimeSpan();
        hunger -= (int)(ts.TotalHours * 2); //Every hour will subtract 2 points from hunger
        if (hunger < 0)
        {
            hunger = 0;
        }

        happiness -= (int)((100 - hunger) * (ts.TotalHours / 5));
        if(happiness < 0)
        {
            happiness = 0;
        }
        #endregion

        if (!PlayerPrefs.HasKey("then"))
        {
            PlayerPrefs.SetString("then", GetStringTime());
        }
        
        //Debug.Log(getTimeSpan().ToString()); TESTING

        if (serverTime)
        {
            
        }
        else
        {
            InvokeRepeating(nameof(UpdateDevice), 0f, 60f); //Every 60 sec will save the time when close game. Then when player opens again, time will be based on 60 secs before game was closed.
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

    #region INCREASE PET STATS METHODS
    /// <summary>
    /// Function to update happiness.
    /// </summary>
    /// <param name="happyIndex">happiness index for determining how much happiness is increased.</param>
    public void UpdateHappiness(float happyIndex)
    {
        happiness += happyIndex;
        _GameManager.instance.lifeSlider.value = happiness;
        happiness++;

        if(happiness >= 100)
        {
            happiness = 100;
        }
    }

    /// <summary>
    /// Function to update hunger.
    /// </summary>
    /// <param name="hungerIndex">hunger index for determining how much hunger is increased.</param>
    public void UpdateHunger(float hungerIndex)
    {
        hunger += hungerIndex;
        _GameManager.instance.lifeSlider.value = hunger;
        hunger++;

        if(hunger >= 100)
        {
            hunger = 100;
        }
    }
    
    public void DecreaseHappiness(float sadIndex)
    {
        happiness -= sadIndex;
        _GameManager.instance.lifeSlider.value = happiness;
        happiness--;

        if(happiness <= 0)
        {
            happiness = 0;
        }
    }
    
    public void DecreaseHunger(float hungerIndex)
    {
        hunger -= hungerIndex;
        _GameManager.instance.lifeSlider.value = hunger;
        hunger--;

        if(hunger <= 0)
        {
            hunger = 0;
        }
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
            PlayerPrefs.SetFloat("fun", Fun);
            PlayerPrefs.SetString("name", Name);
        }
    }
    #endregion
    
    
}
