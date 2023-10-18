using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static int last;
    private int now;
    public static bool cam;
    public static int MoveCounter;
    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;
    public Animator _anim;
    public static bool canGo;
    public Rigidbody rb;
    public float speed;
    private void Start()
    {
        last = 0;
        cam = true;
        canGo = true;
        MoveCounter = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerDownPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            fingerUpPosition = Input.mousePosition;
            MovePlayer();
        }

    }



    private void up() => Run(0, speed, 1);
    public void down() => Run(0, -speed, 2);

    public void left() => Run(-speed, 0, 3);
    public void right() => Run(speed, 0, 4);


    private void MovePlayer()
    {
        
        if (cam == false) return;


        if (rb.velocity.magnitude > 0) return;

        float deltaX = fingerDownPosition.x - fingerUpPosition.x;
        float deltaY = fingerDownPosition.y - fingerUpPosition.y;

        

        if (fingerUpPosition.x > fingerDownPosition.x && fingerUpPosition.y > fingerDownPosition.y)
        {
            up();
            
        }
        else if (fingerUpPosition.x < fingerDownPosition.x && fingerUpPosition.y < fingerDownPosition.y )
        {
            down();
        }
        else if (fingerUpPosition.x < fingerDownPosition.x && fingerUpPosition.y > fingerDownPosition.y )
        {
            left();
        }
        else if (fingerUpPosition.x > fingerDownPosition.x && fingerUpPosition.y < fingerDownPosition.y )
        {
            right();
        }

    }

    private void Run(float x, float z, int lastValue)
    {
        if (last == lastValue) return;
        canGo = false;
        _anim.Play("run");
        rb.velocity = new Vector3(x, 0, z);
        transform.rotation = Quaternion.LookRotation(rb.velocity);
        MoveCounter++;
        last = lastValue;
       
    }
}