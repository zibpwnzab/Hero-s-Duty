using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    private const float TeleportCooldownSeconds = 0.5f;

    [Header("Portal Configuration")]
    public Portal otherPortal;
    public Transform spawnPoint;

    private bool _canTeleport = true;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_canTeleport || otherPortal == null) return;

        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject playerObject)
    {
        if (!playerObject.TryGetComponent(out Rigidbody playerRb) ||
            !playerObject.TryGetComponent(out Player playerScript))
        {
            return;
        }

        _canTeleport = false;
        otherPortal._canTeleport = false;

        Transform targetSpawn = otherPortal.spawnPoint != null ? otherPortal.spawnPoint : otherPortal.transform;
        Quaternion rotationDifference = targetSpawn.rotation * Quaternion.Inverse(transform.rotation);

        Vector3 newVelocity = rotationDifference * playerRb.linearVelocity;

        playerObject.transform.SetPositionAndRotation(targetSpawn.position, targetSpawn.rotation);
        playerRb.linearVelocity = newVelocity;

        playerScript.OnTeleport();
        StartCoroutine(ResetCooldownRoutine());
    }

    private IEnumerator ResetCooldownRoutine()
    {
        yield return new WaitForSeconds(TeleportCooldownSeconds);
        _canTeleport = true;
        if (otherPortal != null)
        {
            otherPortal._canTeleport = true;
        }
    }
}