using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PlaySession : MonoBehaviour
{
    public TMP_Text playTimeText;
    public TMP_Text counterText;
    private int counter = 0;
    
    public static PlaySession instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        counter = PlayerPrefs.GetInt("Counter", counter);
        string dateQuitString = PlayerPrefs.GetString("dateQuit", "");

        if (!dateQuitString.Equals(""))
        {
            DateTime dateQuit = DateTime.Parse(dateQuitString);
            DateTime now = DateTime.Now;

            if (now > dateQuit)
            {
                TimeSpan ts = now - dateQuit;
                int seconds = (int)ts.TotalSeconds;
                counter += seconds;
                Debug.Log("Quit for " + seconds + " seconds");

                if (seconds > 60)
                {
                    //1 minute
                    playTimeText.text = seconds + "Seconds";
                }
            }
            
            PlayerPrefs.SetString("dateQuit", ""); //Delete last stored DateTime in playerprefs
        }

        StartCoroutine("Counter");
    }

    private IEnumerator Counter()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            counter++;
            counterText.text = counter.ToString();
        }
    }

    private void OnApplicationQuit()
    {
        DateTime dateQuit = DateTime.Now;
        PlayerPrefs.SetString("dateQuit", dateQuit.ToString());
        PlayerPrefs.SetInt("Counter", counter);
        Debug.Log("Quit at: " + dateQuit);
    }
}
