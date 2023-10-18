using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ice : MonoBehaviour
{
    public Vector3 up;
    public int lastUp;
    public Vector3 down;
    public int lastDown;
    public Vector3 right;
    public int lastRight;
    public Vector3 left;
    public int lastLeft;
    private Rigidbody rb;
    private GameObject player;
    private float deltaZ;
    private float deltaX;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void FixedUpdate()
    { 
        deltaZ = player.transform.position.z - transform.position.z;
        deltaX = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(deltaZ) <= 0.0001f & Mathf.Abs(deltaX) <= 0.0001f)
        {
            if(Player.last == 2)
            {
                rb.velocity = new Vector3(up.x, 0, up.z);
                player.transform.rotation = Quaternion.LookRotation(rb.velocity);
                Player.last = lastUp;
            }
            else if (Player.last == 1)
            {
                rb.velocity = new Vector3(down.x, 0, down.z);
                player.transform.rotation = Quaternion.LookRotation(rb.velocity);
                Player.last = lastDown;
            }
            else if (Player.last == 3)
            {
                rb.velocity = new Vector3(right.x, 0, right.z);
                player.transform.rotation = Quaternion.LookRotation(rb.velocity);
                Player.last = lastRight;
            }
            else if (Player.last == 4)
            {
                rb.velocity = new Vector3(left.x, 0, left.z);
                player.transform.rotation = Quaternion.LookRotation(rb.velocity);
                Player.last = lastLeft;
            }

        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            rb = other.GetComponent<Rigidbody>();
            
        }

    }
    
}
