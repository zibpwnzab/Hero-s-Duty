using UnityEngine;
using UnityEngine.UI;

public class dead : MonoBehaviour
{
    public GameObject lose;
    private GameObject des;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            lose.SetActive(true);
            //des = other.gameObject;
            Player.cam = false;
        }    
            
    }
}
    

