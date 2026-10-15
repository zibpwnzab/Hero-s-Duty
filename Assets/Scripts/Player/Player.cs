using UnityEngine;

public class Player : MonoBehaviour
{
    private const float MinSwipePixels = 20f;
    private const float StopEpsilon = 0.01f;
    private const float RaycastHeightOffset = 0.1f;
    private const float DefaultMaxRayDistance = 100f;

    [Header("VFX & Animation")]
    public GameObject vfxSlashPrefab;
    public Transform vfxSpawnPoint;
    public Animator _anim;
    public Rigidbody rb;
    public float speed = 10f;
    public TrailRenderer playerTrail;
    public TrailRenderer playerTrail2;

    [Header("Obstacle Filtering")]
    public LayerMask obstacleLayers = 0;
    public string[] obstacleTags = { "Obstacle", "graund" };

    [Header("Attack Settings")]
    public LayerMask enemyLayers;
    public float attackDistance = 1.5f;

    public static int last;
    public static bool cam;
    public static int MoveCounter;
    public static bool canGo;

    private GameObject _activeVfx;
    private Vector2 _fingerDownPosition;
    private Vector2 _fingerUpPosition;

    private bool _isMoving;
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _moveStartPos = Vector3.zero;
    private float _stopDistance = Mathf.Infinity;

    private void Start()
    {
        last = 0;
        cam = true;
        canGo = true;
        MoveCounter = 0;

        if (_anim == null) _anim = GetComponentInChildren<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();

        if (_anim != null)
        {
            _anim.applyRootMotion = false;
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        SetTrailsEmitting(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _fingerDownPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _fingerUpPosition = Input.mousePosition;
            TryProcessSwipe();
        }
    }

    private void FixedUpdate()
    {
        if (!_isMoving || rb == null) return;

        if (!float.IsInfinity(_stopDistance))
        {
            float travelled = Vector3.Project(transform.position - _moveStartPos, _moveDirection).magnitude;
            if (travelled + StopEpsilon >= _stopDistance)
            {
                StopMovement();
                return;
            }
        }

        if (rb.linearVelocity.magnitude < StopEpsilon && _isMoving)
        {
            StopMovement();
        }
    }

    public void up() => StartRun(0f, 1f, 1);
    public void down() => StartRun(0f, -1f, 2);
    public void left() => StartRun(-1f, 0f, 3);
    public void right() => StartRun(1f, 0f, 4);

    private void OnCollisionEnter(Collision collision)
    {
        if (IsSelfCollision(collision)) return;
        if (!IsObstacle(collision.gameObject)) return;

        StopMovement();
        last = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isEnemyLayer = ((1 << other.gameObject.layer) & enemyLayers.value) != 0;
        bool isEnemyTag = other.CompareTag("mob");

        if (isEnemyLayer || isEnemyTag)
        {
            StopMovement();
            PlayAnimSafe("attack", "Attack");

            Vector3 lookDir = other.transform.position - transform.position;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero && rb != null)
            {
                rb.MoveRotation(Quaternion.LookRotation(lookDir));
            }

            PlaySlashVFX();
        }
    }

    private bool IsSelfCollision(Collision collision)
    {
        if (collision.transform.IsChildOf(transform) || collision.gameObject == gameObject)
            return true;

        return collision.rigidbody != null && collision.rigidbody == rb;
    }

    private bool IsObstacle(GameObject go)
    {
        if (go.CompareTag("Player") || go.CompareTag("mob"))
            return false;

        if (obstacleLayers != 0 && ((1 << go.layer) & obstacleLayers.value) != 0)
            return true;

        if (obstacleTags != null)
        {
            foreach (string t in obstacleTags)
            {
                if (!string.IsNullOrEmpty(t) && go.tag == t)
                    return true;
            }
        }

        return go.CompareTag("graund") || go.CompareTag("Untagged");
    }

    public void PlaySlashVFX()
    {
        if (_activeVfx != null)
        {
            Destroy(_activeVfx);
        }

        if (vfxSlashPrefab != null && vfxSpawnPoint != null)
        {
            _activeVfx = Instantiate(vfxSlashPrefab, vfxSpawnPoint.position, vfxSpawnPoint.rotation, vfxSpawnPoint);
        }
    }

    private void TryProcessSwipe()
    {
        if (!cam || rb == null) return;
        if (rb.linearVelocity.magnitude > StopEpsilon) return;

        Vector2 delta = _fingerUpPosition - _fingerDownPosition;
        if (delta.magnitude < MinSwipePixels) return;

        if (delta.x > 0f && delta.y > 0f) up();
        else if (delta.x < 0f && delta.y < 0f) down();
        else if (delta.x < 0f && delta.y > 0f) left();
        else if (delta.x > 0f && delta.y < 0f) right();
    }

    private void StartRun(float x, float z, int directionCode)
    {
        if (last == directionCode || rb == null) return;

        Vector3 dir = new Vector3(x, 0f, z).normalized;
        if (dir == Vector3.zero) return;

        SetTrailsEmitting(true);
        canGo = false;

        PlayAnimSafe("run", "Run");

        _isMoving = true;
        _moveDirection = dir;
        _moveStartPos = transform.position;
        RecalculateStopDistance(dir);

        rb.linearVelocity = dir * speed;
        rb.MoveRotation(Quaternion.LookRotation(dir));

        MoveCounter++;
        last = directionCode;
    }

    public void StopMovement()
    {
        SetTrailsEmitting(false);

        if (rb != null && !rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
        }

        _isMoving = false;
        _stopDistance = Mathf.Infinity;
        canGo = true;

        if (_activeVfx != null)
        {
            Destroy(_activeVfx);
            _activeVfx = null;
        }

        PlayAnimSafe("Idle", "stop");
    }

    public void OnTeleport()
    {
        if (rb == null) return;

        _moveStartPos = transform.position;
        Vector3 currentDir = rb.linearVelocity.sqrMagnitude > StopEpsilon
            ? rb.linearVelocity.normalized
            : _moveDirection;

        _moveDirection = currentDir;
        RecalculateStopDistance(currentDir);
    }

    public void OnRedirected(Vector3 newVelocity, int newDirectionCode)
    {
        if (rb == null) return;

        rb.linearVelocity = newVelocity;
        last = newDirectionCode;

        if (newVelocity.sqrMagnitude > StopEpsilon)
        {
            _moveDirection = newVelocity.normalized;
            _moveStartPos = transform.position;
            _isMoving = true;
            canGo = false;
            rb.MoveRotation(Quaternion.LookRotation(_moveDirection));
            RecalculateStopDistance(_moveDirection);
        }
    }

    private void RecalculateStopDistance(Vector3 direction)
    {
        _stopDistance = Mathf.Infinity;
        if (obstacleLayers == 0 || direction == Vector3.zero) return;

        Ray ray = new Ray(transform.position + Vector3.up * RaycastHeightOffset, direction.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, DefaultMaxRayDistance, obstacleLayers))
        {
            if (hit.collider == null || !hit.collider.transform.IsChildOf(transform))
            {
                float colliderRadius = 0.4f;
                if (TryGetComponent(out Collider col))
                {
                    colliderRadius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.z);
                }
                _stopDistance = Mathf.Max(0f, hit.distance - colliderRadius);
            }
        }
    }

    private void PlayAnimSafe(string primaryState, string fallbackState = null)
    {
        if (_anim == null || _anim.runtimeAnimatorController == null) return;

        int primaryHash = Animator.StringToHash(primaryState);
        if (_anim.HasState(0, primaryHash))
        {
            _anim.Play(primaryHash, 0);
            return;
        }

        if (!string.IsNullOrEmpty(fallbackState))
        {
            int fallbackHash = Animator.StringToHash(fallbackState);
            if (_anim.HasState(0, fallbackHash))
            {
                _anim.Play(fallbackHash, 0);
            }
        }
    }

    private void SetTrailsEmitting(bool emitting)
    {
        if (playerTrail != null) playerTrail.emitting = emitting;
        if (playerTrail2 != null) playerTrail2.emitting = emitting;
    }

    private void OnAnimatorMove() { }
}