using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    [Header("Settings")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeDuration = 1.0f;
    [SerializeField] private float _normalVolume = 1.0f;
    [SerializeField] private float _duckedVolume = 0.3f;

    private void Awake()
    {
        // ЖЕСТКАЯ ЛОГИКА СИНГЛТОНА
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Живи вечно

            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.loop = true;
        }
        else
        {
            // Если мы загрузились во второй сцене, и там уже есть менеджер с первой...
            // Убиваем самозванца из новой сцены!
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        // ДИАГНОСТИКА (Посмотри в консоль!)
        string currentClipName = _audioSource.clip != null ? _audioSource.clip.name : "Тишина";
        string newClipName = clip.name;

        // Проверка: Если играет ТОТ ЖЕ трек (сверяем имена) И он вообще играет
        if (currentClipName == newClipName && _audioSource.isPlaying)
        {
            Debug.Log($"<color=green>[MusicManager] Трек {newClipName} уже играет. Продолжаем кайфовать.</color>");
            return; // ВЫХОДИМ, ничего не меняя
        }

        Debug.Log($"<color=yellow>[MusicManager] Меняем музыку! Было: {currentClipName}, Стало: {newClipName}</color>");

        // Если трек другой или была тишина — запускаем
        _audioSource.clip = clip;
        _audioSource.volume = _normalVolume;
        _audioSource.Play();
    }

    public void SetDucked(bool isDucked)
    {
        float targetVol = isDucked ? _duckedVolume : _normalVolume;
        StopAllCoroutines();
        StartCoroutine(FadeVolume(targetVol));
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = _audioSource.volume;
        float time = 0;
        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, targetVolume, time / _fadeDuration);
            yield return null;
        }
        _audioSource.volume = targetVolume;
    }
}