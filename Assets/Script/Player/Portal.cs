using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour
{
    [Header("��������� �������")]
    [Tooltip("���������� ���� ������ ������-����")]
    public Portal otherPortal;

    [Tooltip("�����, ������ ����� ���������� �����. ������ ��� �������� ������ ����� ��������.")]
    public Transform spawnPoint;

    // --- ��������� ���������� ---
    private bool canTeleport = true; // ���� ��� �������������� ������������ �����

    private void Awake()
    {
        // ���� ����� ������ �� �������, ���������� ������� ������ �������
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���������, ��� � ������ ����� ����� � ��� ������������ ������ ��������
        if (other.CompareTag("Player") && canTeleport)
        {
            // ���������, ��� ������ �� ������ ������ �����������
            if (otherPortal == null)
            {
                Debug.LogError($"������ '{gameObject.name}' �� ����� ������ �� ������ ������!", this);
                return;
            }

            // ��������� ������� ������������
            TeleportPlayer(other.gameObject);
        }
    }

    private void TeleportPlayer(GameObject playerObject)
    {
        // �������� ���������� ������
        Rigidbody playerRb = playerObject.GetComponent<Rigidbody>();
        Player playerScript = playerObject.GetComponent<Player>();

        if (playerRb == null || playerScript == null) return;

        // 1. ��������� ����������� ������������ �� ����� ��������, ����� �������� �����
        this.canTeleport = false;
        otherPortal.canTeleport = false;

        // 2. ������������ ������� ����� ���������
        // ��� �����, ����� ��������� ��������� �������� ������ �� ������
        Quaternion rotationDifference = otherPortal.spawnPoint.rotation * Quaternion.Inverse(this.transform.rotation);

        // 3. ��������� ������� �������� � ������������ �� � ������������ � �������
        Vector3 currentVelocity = playerRb.linearVelocity;
        Vector3 newVelocity = rotationDifference * currentVelocity;

        // 4. ��������� ���������� ������ � ����� ������ ������� �������
        playerObject.transform.position = otherPortal.spawnPoint.position;
        playerObject.transform.rotation = otherPortal.spawnPoint.rotation;

        // 5. ����������� ������ �����, ���������������� ��������
        playerRb.linearVelocity = newVelocity;

        // 6. �������� ������� ������, ��� ��������� ������������
        // ��� �����, ����� �� ���������� ���������� �� ��������� �����
        playerScript.OnTeleport();

        // 7. ��������� �������� ��� "�����������" ��������
        StartCoroutine(ResetTeleportCooldown());
    }

    private IEnumerator ResetTeleportCooldown()
    {
        // ���� ��������� �����, ����� ����� ����� �������� �� �������
        yield return new WaitForSeconds(0.5f);

        // ����� ��������� ������������
        this.canTeleport = true;
        otherPortal.canTeleport = true;
    }
}