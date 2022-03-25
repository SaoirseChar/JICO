using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectPet : MonoBehaviour
{
    //Array of Pets
    [SerializeField] private GameObject[] pets;
    //Pet selection index 
    [SerializeField] private int choiceIndex;

    private void Start()
    {
        choiceIndex = PlayerPrefs.GetInt("Pet Selected");

        pets = new GameObject[transform.childCount];

        //Fill array with pet models
        for (int i = 0; i < transform.childCount; i++)
            pets[i] = transform.GetChild(i).gameObject;
        
        //Turn off renderer
        foreach (GameObject go in pets)
            go.SetActive(false);
        
        //Turn on selected pet model
        if (pets[choiceIndex])
            pets[choiceIndex].SetActive(true);

        if(_GameManager.instance != null)
            _GameManager.instance.pet = pets[choiceIndex].GetComponent<MasterPet>();
    }


    public void NextPet() //Right
    {
        //Turn off current model
        pets[choiceIndex].SetActive(false);

        choiceIndex++;

        if (choiceIndex == pets.Length)
            choiceIndex = 0;

        //Turn on new model
        pets[choiceIndex].SetActive(true);
    }

    public void PreviousPet() //Left
    {
        //Turn off current model
        pets[choiceIndex].SetActive(false);

        choiceIndex--;
        if (choiceIndex < 0)
            choiceIndex = pets.Length - 1;

        //Turn on new model
        pets[choiceIndex].SetActive(true);
    }

    public void ChoosePet()
    {
        PlayerPrefs.SetInt("Pet Selected", choiceIndex);
        SceneManager.LoadScene(2);
    }
}
