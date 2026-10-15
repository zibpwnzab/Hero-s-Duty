using UnityEngine;

public class boss : MonoBehaviour
{
    public GameObject player;
    private Rigidbody _playerRb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerRb = other.GetComponent<Rigidbody>();
            player = other.gameObject;
        }
    }
}
