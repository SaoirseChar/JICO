using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TestNotification : MonoBehaviour
{
    private int systemHour = DateTime.Now.Hour;
    private DayOfWeek systemDay = DateTime.Now.DayOfWeek;
    [SerializeField] private GameObject optionPanel;
    
    // Start is called before the first frame update
    void Start()
    {
        //optionPanel = GameObject.Find("Input Options").gameObject;
        
        if (systemDay == DayOfWeek.Friday)
        {
            Debug.Log("Happy Friday!");
            systemDay = DayOfWeek.Friday;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (systemHour >= 13)
        {
            Debug.Log("Good Afternoon");
            systemHour = 13;
        }
        else if (systemHour >= 7)
        {
            Debug.Log("Good Morning!");
        }
        else if (systemHour >= 22)
        {
            Debug.Log("Good Night!");
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            DateTime leaveDate = new DateTime(2022, 4,8,10,0,0); //8th April 2022 at 10am is when Jico will leave.
            DateTime now = DateTime.Now;
            TimeSpan ts = leaveDate - now;

            Debug.Log("Hours between" + ts.TotalDays + ts.TotalHours);
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            systemDay++;
            Debug.Log("Today is: " + systemDay);
        }
    }

    public void SaveActivties()
    {
        
    }
}
