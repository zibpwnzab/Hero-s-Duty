using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    private const float TargetFallAngle = 90f;

    [Header("Fall Settings")]
    public float fallSpeed = 180f;
    public string walkableLayerName = "Default";

    [Header("Damage Settings")]
    public Collider damageCollider;
    public LayerMask enemyLayers;
    public GameObject enemyDeathVFX;

    private bool _isFallen;
    private Rigidbody _rb;
    private Collider _mainCollider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider col in colliders)
        {
            if (!col.isTrigger)
            {
                _mainCollider = col;
            }
            else if (damageCollider == null)
            {
                damageCollider = col;
            }
        }

        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFallen) return;

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            Vector3 directionToObject = transform.position - player.transform.position;
            Vector3 fallDirection = GetCardinalDirection(directionToObject);
            StartCoroutine(FallRoutine(fallDirection));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isFallen) return;

        bool isEnemyLayer = ((1 << other.gameObject.layer) & enemyLayers.value) != 0;
        if (isEnemyLayer || other.CompareTag("mob"))
        {
            KillEnemy(other.gameObject);
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        if (enemyDeathVFX != null)
        {
            Instantiate(enemyDeathVFX, enemy.transform.position, Quaternion.identity);
        }

        Finish.vrags = Mathf.Max(0, Finish.vrags - 1);
        killboss.vrags = Mathf.Max(0, killboss.vrags - 1);
        Destroy(enemy);
    }

    private IEnumerator FallRoutine(Vector3 fallDirection)
    {
        _isFallen = true;
        if (damageCollider != null) damageCollider.enabled = true;

        Vector3 rotationAxis = Vector3.Cross(Vector3.up, fallDirection);
        float pivotY = _mainCollider != null ? _mainCollider.bounds.min.y : transform.position.y;
        Vector3 pivotPoint = new Vector3(transform.position.x, pivotY, transform.position.z);

        float totalRotation = 0f;

        while (totalRotation < TargetFallAngle)
        {
            float step = fallSpeed * Time.deltaTime;
            if (totalRotation + step > TargetFallAngle)
            {
                step = TargetFallAngle - totalRotation;
            }

            transform.RotateAround(pivotPoint, rotationAxis, step);
            totalRotation += step;
            yield return null;
        }

        if (damageCollider != null) damageCollider.enabled = false;

        int newLayer = LayerMask.NameToLayer(walkableLayerName);
        if (newLayer != -1) gameObject.layer = newLayer;
    }

    private static Vector3 GetCardinalDirection(Vector3 diff)
    {
        diff.y = 0f;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            return diff.x > 0f ? Vector3.right : Vector3.left;
        }
        return diff.z > 0f ? Vector3.forward : Vector3.back;
    }
}