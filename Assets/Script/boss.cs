using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss : MonoBehaviour
{
    private Rigidbody rb;
    public GameObject player;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb = other.GetComponent<Rigidbody>();
            player = other.gameObject;

        }

 

    }
} 
