using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Finish : MonoBehaviour
{
    public int NeedForOne;
    public int NeedForTwo;
    public int NeedForThree;

    public GameObject s1;
    public GameObject s2;
    public GameObject s3;

    private MeshRenderer _mesh;
    public GameObject win;
    public static int vrags;
    public int mobs;

    // ++ НОВАЯ ПЕРЕМЕННАЯ:
    // Нужна, чтобы победа не срабатывала 100 раз подряд, пока игрок стоит на люке
    private bool isWinTriggered = false;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        vrags = mobs;
        _mesh = GetComponent<MeshRenderer>();
        _mesh.enabled = false;

        // ++ Сбрасываем флаг при старте уровня
        isWinTriggered = false;
    }

    public string LvL;
    private int stars;
    private int Allstars;
    private int Lstars;
    private int delta;

    void Update()
    {
        if (vrags == 0)
        {
            _mesh.enabled = true;
        }
    }

    // !! ИЗМЕНЕНИЕ: Меняем OnTriggerEnter на OnTriggerStay
    // Этот метод срабатывает каждый физический кадр, пока игрок ВНУТРИ триггера.
    void OnTriggerStay(Collider other)
    {
        // 1. Проверяем, что это игрок, враги убиты, и мы еще не засчитали победу
        if (other.tag != "Player" || vrags != 0 || isWinTriggered)
        {
            return;
        }

        // 2. !! ГЛАВНАЯ ПРОВЕРКА:
        // Теперь мы проверяем, остановлен ли игрок.
        // (Используем твою статическую переменную `Player.canGo` из скрипта Player)
        if (Player.canGo == true)
        {
            // -- Игрок ОСТАНОВИЛСЯ на люке! --

            // 3. Немедленно ставим флаг, чтобы этот код не выполнился на следующем кадре
            isWinTriggered = true;

            // 4. Весь твой код победы
            win.SetActive(true);
            if (Player.MoveCounter <= NeedForOne)
            {
                s1.SetActive(true);
                stars = 1;
            }
            if (Player.MoveCounter <= NeedForTwo)
            {
                s2.SetActive(true);
                stars = 2;
            }
            if (Player.MoveCounter <= NeedForThree)
            {
                s3.SetActive(true);
                stars = 3;
            }
            Lstars = PlayerPrefs.GetInt(LvL);
            if (stars > Lstars)
            {
                delta = stars - Lstars;

                Allstars = PlayerPrefs.GetInt("money");
                Allstars += delta;
                PlayerPrefs.SetInt("money", Allstars);
                Lstars = stars;
                PlayerPrefs.SetInt(LvL, Lstars);
            }

            Player.cam = false;
        }
    }
}