using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class _GameManager : MonoBehaviour
{
    //[Header("Throw Ball")]
    //public GameObject ball;
    //public Transform spawnPos;
    //public GameObject ballObject;
    //public float force = 5f;
    //Constant float to keep the maximum amount of force given to the object
    //public const float maxForce = 100f;

    public static _GameManager instance = null;
    public int clickCount;

    [Header("UI Stuff")]
    public Slider happinessSlider;
    public Slider hungerSlider;
    public MasterPet pet;

    #region UI Elements
    [Header("Quick Menu")]
    public TMP_Text nameText;
    public Button editName;
    public GameObject changeNamePanel;
    public GameObject nameInput;
    
    /*public Button menuButton;
    public GameObject explore;
    public GameObject quitButton;
    public GameObject feed;
    //public GameObject call;
    public GameObject playFetch;
    //public AudioSource whistleAudio;*/
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        /*//Set all icons in the quick menu to false
        changeNamePanel.SetActive(false);
        editName.gameObject.SetActive(false);
        explore.SetActive(false);
        quitButton.SetActive(false);
        feed.SetActive(false);
        //call.SetActive(false);
        playFetch.SetActive(false);*/
    }

    private void Update()
    {
        hungerSlider.value = pet.Hunger;
        happinessSlider.value = pet.Happiness;
        nameText.text = pet.Name;

        if(Input.GetKeyDown(KeyCode.I))
        {
            changeNamePanel.SetActive(true);
            editName.enabled = true;
        }
    }

    /*public void ThrowBall()
    {
        GameObject ball = ObjectPool.instance.GetPooledToyObject("Toy");
        if (ball != null)
        {
            ball.SetActive(true);
            ball.transform.position = spawnPos.transform.position;
            ball.GetComponent<Rigidbody>().AddForce(transform.forward * 5000);
        }
    }*/

    /// <summary>
    /// Function to set the nickname of your pet.
    /// </summary>
    /// <param name="b">Bool to check if is true then change name.</param>
    public void ChangeNickname(bool b)
    {
        if(b)
        {
            pet.Name = nameInput.GetComponent<InputField>().text; //Connect Name to Input field object
            PlayerPrefs.SetString("name", pet.Name); //Set the string name to the Pet name 
        }
    }

    /// <summary>
    /// Call pet with a whistle.
    /// </summary>
    public void Call()
    {
        //whistleAudio.Play();
        //Move pet to original starting position
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        #if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        #endif
        Application.Quit();
    }

    public void FeedPet()
    {
        pet.UpdateHunger(5);
    }

}
