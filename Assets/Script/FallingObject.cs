using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingObject : MonoBehaviour
{
    [Header("Настройки падения")]
    public float fallSpeed = 180f;
    public string walkableLayerName = "Default";

    [Header("Настройки Урона")]
    public Collider damageCollider; // Тот, что с галочкой IsTrigger
    public LayerMask enemyLayers;
    public GameObject enemyDeathVFX;

    private bool _isFallen = false;
    private Rigidbody _rb;
    private Collider _mainCollider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        // Ищем основной (твердый) коллайдер
        var colliders = GetComponents<Collider>();
        foreach (var col in colliders)
        {
            if (!col.isTrigger) _mainCollider = col;
            else if (damageCollider == null) damageCollider = col; // Авто-поиск триггера
        }

        if (damageCollider != null) damageCollider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isFallen) return;

        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            // ИЗМЕНЕНИЕ: Не смотрим на скорость. 
            // Вычисляем вектор ОТ игрока К дереву.
            Vector3 directionToTree = transform.position - player.transform.position;

            // Превращаем это в строгое направление (Вверх, Вниз, Влево, Вправо)
            Vector3 fallDirection = GetCardinalDirection(directionToTree);

            Debug.Log($"Удар! Игрок пришел со стороны: {-fallDirection}. Дерево падает в: {fallDirection}");

            StartCoroutine(FallRoutine(fallDirection));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Убиваем врагов триггером
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

        // Точка вращения - низ дерева
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

    // Логика определения направления на основе позиций
    private Vector3 GetCardinalDirection(Vector3 diff)
    {
        diff.y = 0; // Игнорируем высоту

        // Если разница по X больше, значит удар сбоку
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