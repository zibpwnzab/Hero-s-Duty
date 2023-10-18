using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Collider[] col;
    public Animator anim;
    void Start()
    {
        col = GetComponents<Collider>();
    }
    private IEnumerator Dead()
    {
        Destroy(col[0]);
        Destroy(col[1]);
       
        yield return new WaitForSeconds(0.42f);
        
        anim.Play("dead");
        Finish.vrags -= 1;
        killboss.vrags -= 1;

        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(Dead());
        }
    }
}
