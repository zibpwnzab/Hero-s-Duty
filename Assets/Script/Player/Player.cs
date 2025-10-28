using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject vfxSlashPrefab; // ������ VFX (���������� ��� ����������)
    public Transform vfxSpawnPoint;
    private GameObject _activeVFX;

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

    public TrailRenderer playerTrail;
    public TrailRenderer playerTrail2;

    [Header("Obstacle filtering")]
    [Tooltip("���������� ����(�) ����������� (�������� 'Obstacle' ��� 'Default'). ���� �� ������ ����������� � ��������� 'Everything' ��� �������� ������ ����.")]
    public LayerMask obstacleLayers = 0; // ������� � ���������� ����(�) ����/�����������
    [Tooltip("�������������� ���� ��������-����������� (�����������)")]
    public string[] obstacleTags = new string[] { "Obstacle" };
    
    [Header("Attack Settings")]
    [Tooltip("���������� ���� 'Enemy', ������� �� ������� ��� ������")]
    public LayerMask enemyLayers; // ���� ����� ����� ��������� ���� "Enemy"
    [Tooltip("�� ����� ���������� ����� ����� ��������� �����")]
    public float attackDistance = 1.5f; // ��������� ����� (��������� ��� ������ ����� ������)

    // ��� ���������������� ���������
    private bool isMoving = false;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveStartPos = Vector3.zero;
    private float stopDistance = Mathf.Infinity; // ���������� �� ����������� (���� �������)
    private float raycastMaxDistance = 100f; // ����� ��������� �� �����

    private void Start()
    {
        last = 0;
        cam = true;
        canGo = true;
        MoveCounter = 0;

        if (_anim != null)
            _anim.applyRootMotion = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            // ������������� ������� continuous � ������ �������� ��������, ����� �������� ��������������
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        if (playerTrail != null) playerTrail.emitting = false;
        if (playerTrail2 != null) playerTrail2.emitting = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            fingerDownPosition = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            fingerUpPosition = Input.mousePosition;
            MovePlayer();
        }
    }

    // ������ � �������� ��������� �������� �� ����������
    private void FixedUpdate()
    {
        if (!isMoving) return;

        // ���� �� ������� ��������� ���������� �� ����������� � ��������������� ��� ����������
        if (!float.IsInfinity(stopDistance))
        {
            float travelled = Vector3.Project(transform.position - moveStartPos, moveDirection).magnitude;
            // ��������� ������ ����� �� ��������
            if (travelled + 0.01f >= stopDistance)
            {
                StopMovement();
            }
        }

        // �������������� ������: ���� �������� ����� ������� � ������� �������� �����������
        if (rb.linearVelocity.magnitude < 0.01f && isMoving)
        {
            StopMovement();
        }
    }

    private void up() => Run(0, 1, 1);    // ��������: ����� speed, ����������� ������ ���� ��������������� (0, 1, 0)
    public void down() => Run(0, -1, 2);  // ��������
    public void left() => Run(-1, 0, 3); // ��������
    public void right() => Run(1, 0, 4); // ��������

    private void OnCollisionEnter(Collision collision)
    {
        // ���������� �������� � ������ ��������� ��������� ������ (�������� VFX, ���� �� �������� ��� child)
        if (IsSelfCollision(collision))
        {
            //Debug.Log($"���������� ������������ � {collision.gameObject.name}");
            return;
        }

        // ��������������� ������ ���� ����������� � �������������� ������������
        if (!IsObstacle(collision.gameObject))
        {
            //Debug.Log($"���������� �������� � ��-������������: {collision.gameObject.name} (layer {collision.gameObject.layer})");
            return;
        }

        // ���� ��� �������� ����������� � ���������������
        StopMovement();

        // ����� ����� ��������
        if (_anim != null) _anim.Play("Idle");

        last = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        // ���������, ��� �� ����� � ������� ������� �� ���� "Enemy"
        if (((1 << other.gameObject.layer) & enemyLayers.value) != 0)
        {
            // ��� ����.
            // ������ ����� `Enemy.cs` ��� ���������� ���� ������ ����� ���� `OnTriggerEnter`.

            // ���� ������ - ������������.
            StopMovement();

            // (�����������) ����� �������� �������� ����� � VFX � ������ �����
            if (_anim != null) _anim.Play("attack"); // �������, ��� ���� �������� "attack"

            // �������������� � �����, �������� �������
            transform.LookAt(other.transform.position);

            PlaySlashVFX();
        }
    }
    private bool IsSelfCollision(Collision collision)
    {
        // ���� ������ ������������ � ��� ����� ��� �������� ������ ������
        if (collision.transform.IsChildOf(transform) || collision.gameObject == gameObject)
            return true;

        // ������ � VFX ���� Rigidbody � ��� �� ������ � ��� �������� �� rb
        if (collision.rigidbody != null && collision.rigidbody == rb)
            return true;

        return false;
    }

    private bool IsObstacle(GameObject go)
    {
        // 1) ���������� �� ����� (���� ���������� obstacleLayers)
        if (obstacleLayers != 0)
        {
            if (((1 << go.layer) & obstacleLayers.value) != 0)
                return true;
        }

        // 2) ���������� �� ����� (���� ������)
        if (obstacleTags != null && obstacleTags.Length > 0)
        {
            foreach (var t in obstacleTags)
                if (!string.IsNullOrEmpty(t) && go.CompareTag(t))
                    return true;
        }

        // ���� �� ���� �� ��� �� ������ � ������� �������������� (������� ������ ���������)
        return false;
    }

    public void PlaySlashVFX()
    {
        if (_activeVFX != null)
            Destroy(_activeVFX);

        if (vfxSlashPrefab != null && vfxSpawnPoint != null)
        {
            _activeVFX = Instantiate(vfxSlashPrefab, vfxSpawnPoint.position, vfxSpawnPoint.rotation);
            _activeVFX.transform.parent = vfxSpawnPoint;
            // ������������: ������ ����������/rigidbody � ������� VFX ��� ��������� VFX �� ����, ������� ����� ����������.
        }
    }
    

    private void MovePlayer()
    {
        if (cam == false) return;
        if (rb.linearVelocity.magnitude > 0.01f) return;

        float deltaX = fingerDownPosition.x - fingerUpPosition.x;
        float deltaY = fingerDownPosition.y - fingerUpPosition.y;

        if (fingerUpPosition.x > fingerDownPosition.x && fingerUpPosition.y > fingerDownPosition.y)
        {
            up();
        }
        else if (fingerUpPosition.x < fingerDownPosition.x && fingerUpPosition.y < fingerDownPosition.y)
        {
            down();
        }
        else if (fingerUpPosition.x < fingerDownPosition.x && fingerUpPosition.y > fingerDownPosition.y)
        {
            left();
        }
        else if (fingerUpPosition.x > fingerDownPosition.x && fingerUpPosition.y < fingerDownPosition.y)
        {
            right();
        }
    }

    private void Run(float x, float z, int lastValue)
    {
        if (last == lastValue) return;

        if (playerTrail != null) playerTrail.emitting = true;
        if (playerTrail2 != null) playerTrail2.emitting = true;
        canGo = false;

        if (_anim != null) _anim.Play("run");

        Vector3 dir = new Vector3(x, 0f, z);
        if (dir == Vector3.zero) return;

        Vector3 vel = dir.normalized * speed; // (���� ������ ��������)

        // --- Raycast ---
        stopDistance = Mathf.Infinity;

        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, dir.normalized);
        RaycastHit hit;

        // !! ���������: �� ���� ������ ����������� (�����)
        // �� *�����*, ����� ����� ������� �� ����� � ����� � ��� �������.
        // ������� Raycast ������ "������" ������ 'obstacleLayers'.
        if (obstacleLayers != 0 && Physics.Raycast(ray, out hit, raycastMaxDistance, obstacleLayers))
        {
            // --- ��� ����������� (������ ������) ---
            stopDistance = hit.distance;

            // ������ �� ��������� � ����
            if (hit.collider != null && hit.collider.transform.IsChildOf(transform))
            {
                stopDistance = Mathf.Infinity;
            }
        }
        // !! ���� "if (��� ����)" ��������� ������.

        // �������� ����� ��������
        isMoving = true;
        moveDirection = dir.normalized;
        moveStartPos = transform.position;

        // ��������������� ��� ��������
        rb.linearVelocity = vel;

        if (rb.linearVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.linearVelocity);

        MoveCounter++;
        last = lastValue;
    }

    private void StopMovement()
    {
        // ��������� ����� � ��������
        if (playerTrail != null) playerTrail.emitting = false;
        if (playerTrail2 != null) playerTrail2.emitting = false;

        rb.linearVelocity = Vector3.zero;
        isMoving = false;
        stopDistance = Mathf.Infinity;
        canGo = true;

        if (_activeVFX != null)
        {
            Destroy(_activeVFX);
            _activeVFX = null;
        }

        if (_anim != null) _anim.Play("Idle");
    }

    // ��������� root motion
    private void OnAnimatorMove() { }

    // ... ����� ������ OnAnimatorMove()

    /// <summary>
    /// ���� ����� ���������� ����� (��������, ��������) ����� ������������,
    /// ����� �������� � ����������� ������ �������� �� ����� �����.
    /// </summary>
    public void OnTeleport()
    {
        // 1. ���������� ��������� ������� ��� ������� ����������� ����
        moveStartPos = transform.position;

        // 2. ��������� ����� Raycast, ����� ����� ����� ����������� � ������� �����������
        stopDistance = Mathf.Infinity;
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, rb.linearVelocity.normalized);
        RaycastHit hit;

        if (obstacleLayers != 0 && Physics.Raycast(ray, out hit, raycastMaxDistance, obstacleLayers))
        {
            // ���������, ��� ��� �� ����� � ������ ���� ��� ���� �������� �������
            if (hit.collider == null || !hit.collider.transform.IsChildOf(transform))
            {
                stopDistance = hit.distance;
            }
        }
    } // <-- ��������� ����������� ������ ������
}
