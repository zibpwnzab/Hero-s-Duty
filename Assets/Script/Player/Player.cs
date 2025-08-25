using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject vfxSlashPrefab; // Сюда перетащим префаб эффекта
    public Transform vfxSpawnPoint;   // Сюда перетащим точку спавна
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

    private void Start()
    {
        last = 0;
        cam = true;
        canGo = true;
        MoveCounter = 0;

        // На всякий случай явно отключаем применение root motion
        if (_anim != null)
            _anim.applyRootMotion = false;

        // Убедимся, что Rigidbody не кинематический
        if (rb != null)
            rb.isKinematic = false;
        if (playerTrail != null)
            playerTrail.emitting = false; // Выключаем на старте
        if (playerTrail2 != null)
            playerTrail2.emitting = false; // Выключаем на старте


    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            fingerDownPosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            fingerUpPosition = Input.mousePosition;
            MovePlayer();
        }
    }

    // Синхронизация позиции/ротации после анимации:
    // это блокирует визуальные смещения, которые анимация может попытаться применить.
    private void LateUpdate()
    {
        // Защита от null
        if (rb == null) return;

        // Принудительно ставим корень объекта туда, где Rigidbody (физика)
        transform.position = rb.position;

        // Синхронизируем вращение с rb (если нужно) — иначе оставим текущее
        // Если ты вращаешь через LookRotation при старте движения, это сохранит корректный вид
        // Только вращаем по Y, чтобы избежать кренов
        if (rb.velocity.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(new Vector3(rb.velocity.x, 0f, rb.velocity.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 1f); // немедленная установка
        }
    }

    private void up() => Run(0, speed, 1);
    public void down() => Run(0, -speed, 2);

    public void left() => Run(-speed, 0, 3);
    public void right() => Run(speed, 0, 4);

    // ... после всего кода добавь этот метод
    private void OnCollisionEnter(Collision collision)
    {
        // Выключаем шлейф при столкновении
        if (playerTrail != null)
            playerTrail.emitting = false;
        if (playerTrail2 != null)
            playerTrail2.emitting = false;
        // 1. Немедленно останавливаем движение игрока
        rb.velocity = Vector3.zero;

        // 2. Проверяем, есть ли сейчас активный эффект
        if (_activeVFX != null)
        {
            // 3. Если есть - уничтожаем его
            Destroy(_activeVFX);
            _activeVFX = null; // Очищаем ссылку на всякий случай
        }

        // 4. (Рекомендация) Сбрасываем анимацию на Idle, чтобы персонаж не бежал в стену
        _anim.Play("Idle");

        // 5. (Рекомендация) Сбрасываем 'last', чтобы можно было сразу двигаться в другую сторону
        last = 0;
    }
    public void PlaySlashVFX()
    {
        // Если старый эффект еще существует, уничтожаем его перед созданием нового
        if (_activeVFX != null)
        {
            Destroy(_activeVFX);
        }

        if (vfxSlashPrefab != null && vfxSpawnPoint != null)
        {
            // Создаем И СОХРАНЯЕМ ссылку на новый эффект в нашу переменную
            _activeVFX = Instantiate(vfxSlashPrefab, vfxSpawnPoint.position, vfxSpawnPoint.rotation);

            // Делаем VFX дочерним к точке спавна
            _activeVFX.transform.parent = vfxSpawnPoint;
        }
    }

    private void MovePlayer()
    {
        if (cam == false) return;

        if (rb.velocity.magnitude > 0.01f) return;

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
        if (playerTrail != null)
            playerTrail.emitting = true;
        if (playerTrail2 != null)
            playerTrail2.emitting = true;
        canGo = false;

        // Вместо "run" лучше проигрывать анимацию атаки.
        // Если ее нет, можно оставить "run", но эффект будет не так синхронизирован.
        _anim.Play("run"); // Предположим, у тебя есть анимация с таким названием

        // движение только через физику
        rb.velocity = new Vector3(x, 0, z);

        // поворачиваем игрока по направлению (если скорость есть)
        if (rb.velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rb.velocity);

        MoveCounter++;
        last = lastValue;
    }

    // --- опционально: если хочешь явно перехватывать root motion (иногда вызывается) ---
    // Этот метод будет вызван Animator'ом, если root motion задействован.
    // Оставляем его пустым — чтобы не применять root motion к transform.
    private void OnAnimatorMove()
    {
        // НИЧЕГО не делаем: не применяем animator.deltaPosition / deltaRotation
        // если вдруг root-motion пытается отработать, мы игнорируем его.
    }
}
