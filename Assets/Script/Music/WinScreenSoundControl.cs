using UnityEngine;

public class WinScreenSoundControl : MonoBehaviour
{
    // Вызывается автоматически, когда объект включается (SetActive(true))
    private void OnEnable()
    {
        if (BackgroundMusicManager.Instance != null)
        {
            // Приглушаем музыку
            BackgroundMusicManager.Instance.SetDucked(true);
        }
    }

    // Вызывается автоматически, когда объект выключается или сцена меняется
    private void OnDisable()
    {
        if (BackgroundMusicManager.Instance != null)
        {
            // Возвращаем громкость (например, если игрок нажал "Рестарт" или закрыл меню)
            BackgroundMusicManager.Instance.SetDucked(false);
        }
    }
}