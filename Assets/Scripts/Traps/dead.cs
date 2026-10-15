using UnityEngine;

public class dead : MonoBehaviour
{
    public GameObject lose;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (lose != null) lose.SetActive(true);
            Player.cam = false;

            if (other.TryGetComponent(out Player playerScript))
            {
                playerScript.StopMovement();
            }
        }
    }
}