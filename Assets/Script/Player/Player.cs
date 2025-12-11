using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject vfxSlashPrefab; // префаб VFX (желательно без коллайдера)
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
    [Tooltip("Установить слой(и) препятствий (например 'Obstacle' или 'Default'). Если не хотите фильтровать — поставьте 'Everything' или включите нужные слои.")]
    public LayerMask obstacleLayers = 0; // выставь в инспекторе слой(и) стен/препятствий
    [Tooltip("Дополнительные теги объектов-препятствий (опционально)")]
    public string[] obstacleTags = new string[] { "Obstacle" };

    [Header("Attack Settings")]
    [Tooltip("Установите слой 'Enemy', который вы создали для врагов")]
    public LayerMask enemyLayers; // Сюда нужно будет выставить слой "Enemy"
    [Tooltip("На каком расстоянии игрок может атаковать врага")]
    public float attackDistance = 1.5f; // Дистанция атаки (подберите под размер вашей клетки)

    // для предсказательной остановки
    private bool isMoving = false;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 moveStartPos = Vector3.zero;
    private float stopDistance = Mathf.Infinity; // расстояние до препятствия (если найдено)
    private float raycastMaxDistance = 100f; // можно увеличить по карте

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
            // рекомендуется ставить continuous в случае быстрого движения, чтобы избежать туннелирования
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

    // Физика — проверка окончания движения по расстоянию
    private void FixedUpdate()
    {
        if (!isMoving) return;

        // если мы заранее посчитали расстояние до препятствия — останавливаемся при достижении
        if (!float.IsInfinity(stopDistance))
        {
            float travelled = Vector3.Project(transform.position - moveStartPos, moveDirection).magnitude;
            // небольшая дельта чтобы не застрять
            if (travelled + 0.01f >= stopDistance)
            {
                StopMovement();
            }
        }

        // дополнительная защита: если скорость почти нулевая — считаем движение завершённым
        if (rb.linearVelocity.magnitude < 0.01f && isMoving)
        {
            StopMovement();
        }
    }

    private void up() => Run(0, 1, 1);    // ИЗМЕНЕНО: убрал speed, направление должно быть нормализованным (0, 1, 0)
    public void down() => Run(0, -1, 2);  // ИЗМЕНЕНО
    public void left() => Run(-1, 0, 3); // ИЗМЕНЕНО
    public void right() => Run(1, 0, 4); // ИЗМЕНЕНО

    private void OnCollisionEnter(Collision collision)
    {
        // Игнорируем коллизии с самими дочерними объектами игрока (например VFX, если он привязан как child)
        if (IsSelfCollision(collision))
        {
            //Debug.Log($"Игнорируем самоколлизию с {collision.gameObject.name}");
            return;
        }

        // Останавливаемся ТОЛЬКО если столкнулись с действительным препятствием
        if (!IsObstacle(collision.gameObject))
        {
            //Debug.Log($"Игнорируем коллизию с не-препятствием: {collision.gameObject.name} (layer {collision.gameObject.layer})");
            return;
        }

        // Если это реальное препятствие — останавливаемся
        StopMovement();

        // явный сброс анимации
        if (_anim != null) _anim.Play("Idle");

        last = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что мы вошли в триггер врага по слою "Enemy"
        if (((1 << other.gameObject.layer) & enemyLayers.value) != 0)
        {
            // Тут логика.
            // Предполагаем Enemy.cs или аналогичный скрипт на враге для OnTriggerEnter.
            // Если нужно - доработать.
            StopMovement();
            // (Опционально) Анимация атаки персонажа и VFX на спавн поинте
            if (_anim != null) _anim.Play("attack"); // Убедитесь, что есть анимация "attack"
            // Поворот лицом к врагу, используя MoveRotation для совместимости с физикой
            Vector3 lookDir = other.transform.position - transform.position;
            if (lookDir != Vector3.zero)
                rb.MoveRotation(Quaternion.LookRotation(lookDir));
            PlaySlashVFX();
        }
    }

    private bool IsSelfCollision(Collision collision)
    {
        // если объект столкновения — сам игрок или дочерний объект игрока
        if (collision.transform.IsChildOf(transform) || collision.gameObject == gameObject)
            return true;

        // иногда у VFX есть Rigidbody с тем же корнем — ещё проверка на rb
        if (collision.rigidbody != null && collision.rigidbody == rb)
            return true;

        return false;
    }

    private bool IsObstacle(GameObject go)
    {
        // 1) фильтрация по слоям (если установлен obstacleLayers)
        if (obstacleLayers != 0)
        {
            if (((1 << go.layer) & obstacleLayers.value) != 0)
                return true;
        }

        // 2) фильтрация по тегам (если заданы)
        if (obstacleTags != null && obstacleTags.Length > 0)
        {
            foreach (var t in obstacleTags)
                if (!string.IsNullOrEmpty(t) && go.CompareTag(t))
                    return true;
        }

        // если ни слой ни тэг не совпал — считаем НЕпрепятствием (избежим ложных остановок)
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
            // РЕКОМЕНДАЦИЯ: убрать коллайдеры/rigidbody у префаба VFX или поместить VFX на слой, который игрок игнорирует.
        }
    }

    private void MovePlayer()
    {
        if (cam == false) return;
        if (rb.linearVelocity.magnitude > 0.01f) return;

        Vector2 delta = fingerUpPosition - fingerDownPosition;
        // Добавим минимальный порог для свайпа, чтобы игнорировать тапы или мелкие движения (настройте значение)
        if (delta.magnitude < 20f) return; // Порог в пикселях, можно увеличить/уменьшить

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

        // Нормализуем направление и ставим скорость через rb
        Vector3 vel = dir.normalized * speed;

        // --- Raycast вперед чтобы узнать расстояние до ближайшего препятствия (опционально)
        stopDistance = Mathf.Infinity;
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, dir.normalized); // чуть выше пола
        RaycastHit hit;
        if (obstacleLayers != 0)
        {
            if (Physics.Raycast(ray, out hit, raycastMaxDistance, obstacleLayers))
            {
                stopDistance = hit.distance;
                // если hit.target — это сам игрок или его child, игнорируем (защита)
                if (hit.collider != null && hit.collider.transform.IsChildOf(transform))
                {
                    stopDistance = Mathf.Infinity;
                }
            }
        }
        else
        {
            // если obstacleLayers == 0 — не делаем raycast (пользователь не настроил слои)
            stopDistance = Mathf.Infinity;
        }

        // включаем режим движения
        isMoving = true;
        moveDirection = dir.normalized;
        moveStartPos = transform.position;

        // Непосредственно даём скорость
        rb.linearVelocity = vel;

        // Поворот с использованием MoveRotation для лучшей совместимости с физикой в новых версиях Unity
        if (dir != Vector3.zero)
            rb.MoveRotation(Quaternion.LookRotation(dir));

        MoveCounter++;
        last = lastValue;
    }

    private void StopMovement()
    {
        // выключаем шлейф и движение
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

    // блокируем root motion
    private void OnAnimatorMove() { }

    // ... после метода OnAnimatorMove()

    /// <summary>
    /// Этот метод вызывается извне (например, порталом) после телепортации,
    /// чтобы сбросить и пересчитать логику движения из новой точки.
    /// </summary>
    public void OnTeleport()
    {
        // 1. Сбрасываем стартовую позицию для расчета пройденного пути
        moveStartPos = transform.position;

        // 2. Выполняем новый Raycast, чтобы найти новое препятствие в текущем направлении
        stopDistance = Mathf.Infinity;
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, rb.linearVelocity.normalized);
        RaycastHit hit;

        if (obstacleLayers != 0 && Physics.Raycast(ray, out hit, raycastMaxDistance, obstacleLayers))
        {
            // Проверяем, что луч не попал в самого себя или свои дочерние объекты
            if (hit.collider == null || !hit.collider.transform.IsChildOf(transform))
            {
                stopDistance = hit.distance;
            }
        } 
    } // <-- последняя закрывающая скобка класса
}