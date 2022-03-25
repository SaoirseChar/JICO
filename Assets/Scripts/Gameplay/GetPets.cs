using UnityEngine;

public class GetPets : MonoBehaviour
{
    public GameObject[] petPrefabs;
    public Transform spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        int selectedPet = PlayerPrefs.GetInt("selectedPet"); //Connect selectedPet int to the PlayerPrefs one
        GameObject prefab = petPrefabs[selectedPet]; //Connect prefab GO to the array
        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity); //Spawn the clones into next scene
    }
}
