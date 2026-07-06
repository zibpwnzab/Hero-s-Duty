using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
  
    public float fallSpeed = 180f;
    public string walkableLayerName = "Default";

    [Header("Íàñòðîéêè Óðîíà")]
    public Collider damageCollider; 
    public LayerMask enemyLayers;
    public GameObject enemyDeathVFX;

    private bool _isFallen = false;
    private Rigidbody _rb;
    private Collider _mainCollider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

    
        var colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            if (!col.isTrigger) _mainCollider = col;
            else if (damageCollider == null) damageCollider = col; // Àâòî-ïîèñê òðèããåðà
        }

        if (damageCollider != null) damageCollider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFallen) return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
          
            Vector3 directionToTree = transform.position - player.transform.position;

           
            Vector3 fallDirection = GetCardinalDirection(directionToTree);

          
            StartCoroutine(FallRoutine(fallDirection));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (((1 << other.gameObject.layer) & enemyLayers.value) != 0)
        {
            KillEnemy(other.gameObject);
        }
    }

    private void KillEnemy(GameObject enemy)
    {
        if (enemyDeathVFX != null)
            Instantiate(enemyDeathVFX, enemy.transform.position, Quaternion.identity);
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
        float targetAngle = 90f;

        while (totalRotation < targetAngle)
        {
            float step = fallSpeed * Time.deltaTime;
            if (totalRotation + step > targetAngle) step = targetAngle - totalRotation;

            transform.RotateAround(pivotPoint, rotationAxis, step);
            totalRotation += step;
            yield return null;
        }

        OnFallComplete();
    }

    private void OnFallComplete()
    {
        if (damageCollider != null) damageCollider.enabled = false;

        int newLayer = LayerMask.NameToLayer(walkableLayerName);
        if (newLayer != -1) gameObject.layer = newLayer;
    }

    
    private Vector3 GetCardinalDirection(Vector3 diff)
    {
        diff.y = 0; 

      
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            return diff.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return diff.z > 0 ? Vector3.forward : Vector3.back;
        }
    }
}
