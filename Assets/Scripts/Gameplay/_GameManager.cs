using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class _GameManager : MonoBehaviour
{
    /*[Header("Throw Ball")]
    public GameObject ball;
    public Transform spawnPos;
    public GameObject ballObject;
    public float force = 5f;
    //Constant float to keep the maximum amount of force given to the object
    public const float maxForce = 100f;*/

    public static _GameManager instance = null;

    [Header("UI Stuff")]
    public Image _happiness;
    public Image _hunger;
    public Image _fun;
    public MasterPet pet;
    public Transform[] walkPoints;
    public int clickCount;

    #region UI Elements
    /*[Header("Quick Menu")]
    public TMP_Text nameText;
    public Button editName;
    public GameObject changeNamePanel;
    public GameObject nameInput;
    public GameObject explore;
    public GameObject quitButton;
    public GameObject feed;
    public GameObject playFetch;*/
    
    //Wishlist stuff (call/whistle to get pet to come to start position for petting)
    //public GameObject call;
    //public AudioSource whistleAudio;
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

        //call.SetActive(false);
        //Set all icons in the quick menu to false
        /*changeNamePanel.SetActive(false);
        editName.gameObject.SetActive(false);
        explore.SetActive(false);
        quitButton.SetActive(false);
        feed.SetActive(false);
        playFetch.SetActive(false);*/
    }

    private void Update()
    {
        _hunger.fillAmount = pet.Hunger;
        _happiness.fillAmount = pet.Happiness;
        _fun.fillAmount = pet.Fun;
        
        //nameText.text = pet.Name;

        /*if(Input.GetKeyDown(KeyCode.I))
        {
            changeNamePanel.SetActive(true);
            editName.enabled = true;
        }*/
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            pet.DecreaseHunger(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            pet.DecreaseHappiness(0.5f);
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
            //pet.Name = nameInput.GetComponent<InputField>().text; //Connect Name to Input field object
            //PlayerPrefs.SetString("name", pet.Name); //Set the string name to the Pet name 
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
