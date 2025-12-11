using UnityEngine;
using System.Collections; // Обязательно для работы корутин (IEnumerator)

public class LightningTrap : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject lightningVFXPrefab;
    public GameObject loseScreen;
    public float vfxOffset_Y = 0.5f;

    [Header("Тайминги")]
    [Tooltip("Задержка перед ударом в секундах. Подбирай под скорость игрока.")]
    public float strikeDelay = 0.5f;

    // Служебные
    private int _lastProcessedMove = -1;
    private bool _isPlayerInside = false;
    private GameObject _playerRef;

    private void Start()
    {
        // Чтобы при старте игры сразу не шарахнуло, если мы начали с четного
        _lastProcessedMove = Player.MoveCounter;
    }

    private void Update()
    {
        // Если счетчик изменился
        if (Player.MoveCounter != _lastProcessedMove)
        {
            _lastProcessedMove = Player.MoveCounter;

            // Если ход четный — запускаем процедуру удара с задержкой
            if (Player.MoveCounter % 2 == 0)
            {
                StartCoroutine(StrikeWithDelay());
            }
        }
    }

    // Это Корутина. Она умеет ставить выполнение на паузу.
    private IEnumerator StrikeWithDelay()
    {
        // 1. Ждем, пока игрок доедет (или почти доедет)
        yield return new WaitForSeconds(strikeDelay);

        // 2. Теперь (спустя полсекунды) играем эффект
        PlayVFX();

        // 3. И проверяем, стоит ли кто-то в ловушке ИМЕННО СЕЙЧАС
        if (_isPlayerInside && _playerRef != null)
        {
            KillPlayer(_playerRef);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
            _playerRef = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            _playerRef = null;
        }
    }

    private void PlayVFX()
    {
        if (lightningVFXPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y += vfxOffset_Y;
            GameObject vfx = Instantiate(lightningVFXPrefab, spawnPos, Quaternion.identity);
            Destroy(vfx, 2f);
        }
    }

    private void KillPlayer(GameObject playerObj)
    {
        Player playerScript = playerObj.GetComponent<Player>();

        if (playerScript != null)
        {
            // 1. Выключаем скрипт, чтобы он не мешал (не включал Idle)
            playerScript.enabled = false;

            // 2. Останавливаем физику
            if (playerScript.rb != null)
            {
                playerScript.rb.linearVelocity = Vector3.zero;
                playerScript.rb.isKinematic = true;
            }

            // 3. ПРИНУДИТЕЛЬНЫЙ ЗАПУСК АНИМАЦИИ
            if (playerScript._anim != null)
            {
                // CrossFadeInFixedTime работает надежнее, чем Play.
                // 0.1f — это время смешивания (чтобы переход был плавным, а не дерганым)
                playerScript._anim.CrossFadeInFixedTime("dead", 0.1f);
            }
        }

        // 4. Отключаем управление
        Player.cam = false;

        // 5. Включаем экран проигрыша
        if (loseScreen != null)
            loseScreen.SetActive(true);

        Debug.Log("Игрок убит, анимация dead запущена");
    }
}