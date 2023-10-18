using UnityEngine;
using System.Collections;

public class reaction : MonoBehaviour
{
    public GameObject player;
    public Rigidbody rb;
    public Animator anim;
    private Collider vrag;
    private int can = 2;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "graund")
        {
            
            anim.Play("stop");
            Player.canGo = true;
        }
        else if (other.tag == "mob")
        {
            anim.Play("attack");
            StartCoroutine(at());
            Player.last = 0;
        }
        else if (other.tag == "dead")
        {
            anim.Play("dead");
            rb.velocity = new Vector3(0, 0, 0);
        }
        else if (other.tag == "boss" && killboss.rast == true)
        {
            can -= 1;
            Debug.Log("‡‡‡");
            if (Player.last == 1 && can == 0 )
            {
                rb.velocity = new Vector3(0, 0, -5) ;  
                Player.last = 2;
                can = 2;
            }
            else if (Player.last == 2 && can == 0)
            {
                rb.velocity = new Vector3(0, 0, 5);
                Player.last = 1;
                can = 2;
            }
            else if (Player.last == 3 && can == 0)
            {
                rb.velocity = new Vector3(5, 0, 0);
                Player.last = 4;
                can = 2;
            }
            else if (Player.last == 4 && can == 0)
            {
                rb.velocity = new Vector3(-5, 0, 0);
                Player.last = 3;
                can = 2;
            }





            player.transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }
    private IEnumerator at()
    {
        Player.cam = false;
        yield return new WaitForSeconds(0.42f);
        Player.cam = true;
    }
}