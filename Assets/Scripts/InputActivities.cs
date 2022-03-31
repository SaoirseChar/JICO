using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputActivities : MonoBehaviour
{
    private List<string> weekdays = new List<string>
    {
        DayOfWeek.Monday.ToString(), DayOfWeek.Tuesday.ToString(), DayOfWeek.Wednesday.ToString(), DayOfWeek.Thursday.ToString(), DayOfWeek.Friday.ToString()
    };

    private TMP_Dropdown weekdayDropdown;
    public TMP_InputField input;
    public TMP_InputField[] toDoList;

    private void Start()
    {
        
        
        //Get component
        weekdayDropdown = GetComponent<TMP_Dropdown>();

        //Clear old dropdown options
        weekdayDropdown.ClearOptions();
        
        //Add weekdays
        weekdayDropdown.AddOptions(weekdays);

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            input.text = toDoList[toDoList.Length].text;
            PlayerPrefs.Save();
            gameObject.SetActive(false);
        }
    }
    
    public void SetString(string KeyName, string Value)
    {
        PlayerPrefs.SetString(KeyName, Value);
    }

    public string GetString(string KeyName)
    {
        return PlayerPrefs.GetString(KeyName);
    }
}
