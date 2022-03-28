using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    private static readonly int CanTransition = Animator.StringToHash("CanTransition");
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    private void Start()
    {
        transition.SetBool(CanTransition, false);
    }

    void Update()
    {
        //Go to bathroom (Testing)
        if (Input.GetKeyDown(KeyCode.B))
        {
            transition.SetBool(CanTransition, true);
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }

        //Return to bedroom (Testing)
        if (Input.GetKeyDown(KeyCode.R))
        {
            transition.SetBool(CanTransition, true);
            StartCoroutine(ReturnLevel(SceneManager.GetActiveScene().buildIndex - 1));
        }
    }

    private IEnumerator LoadLevel(int _levelIndex)
    {
        //Play anim
        transition.Play("Closed");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Load scene
        SceneManager.LoadScene(_levelIndex);
    }
    
    private IEnumerator ReturnLevel(int _levelIndex)
    {
        //Play anim
        transition.SetBool(IsOpen, true);

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Load scene
        SceneManager.LoadScene(_levelIndex);
    }
}