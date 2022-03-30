using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectUI : MonoBehaviour
{
    public TMP_Text text;
    public GameObject uiObject;

    private void Start()
    {
        text.text = "";
        uiObject.SetActive(false);
    }

    private void OnMouseEnter ()
    {
        text.text = name;
        uiObject.SetActive(true);
    }
 
    private void OnMouseExit ()
    {
        text.text = "";
        uiObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (gameObject.tag.Equals("Jico"))
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("Room");
        }
    }
}
