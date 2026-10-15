using UnityEngine;

public class LevelMusicStart : MonoBehaviour
{
    public AudioClip chapterMusic; // Сюда кидай музыку этой главы

    void Start()
    {
        // Обращаемся к нашему вечному менеджеру
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMusic(chapterMusic);
            // На всякий случай возвращаем громкость в норму при старте уровня
            BackgroundMusicManager.Instance.SetDucked(false);
        }
        else
        {
            Debug.LogError("Эй! Ты забыла поместить BackgroundMusicManager в стартовую сцену!");
        }
    }
}