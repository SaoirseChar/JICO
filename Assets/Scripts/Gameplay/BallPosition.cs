using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPosition : MonoBehaviour
{
    private bool hasPet;
    private Transform parent;

    private void Start()
    {
        parent = gameObject.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {

        if(hasPet && transform.localPosition != Vector3.zero)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public void SetPet()
    {
        hasPet = true;
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }

    public void RemovePet()
    {
        hasPet = false;
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        gameObject.transform.SetParent(parent);
        gameObject.SetActive(false);
    }
}
