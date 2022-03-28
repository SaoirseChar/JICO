using System;
using TMPro;
using UnityEngine;

public class SelectUI : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        text.text = "";
    }

    private void OnMouseEnter ()
    {
        text.text = name;
    }
 
    private void OnMouseExit ()
    {
        text.text = "";
    }
}
