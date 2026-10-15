using UnityEngine;

public class killboss : MonoBehaviour
{
    private const string TotalStarsPrefsKey = "money";

    public static bool rast;
    public static int vrags;

    public int NeedForOne;
    public int NeedForTwo;
    public int NeedForThree;

    public GameObject s1;
    public GameObject s2;
    public GameObject s3;

    public GameObject win;
    public int mobs;
    public string LvL;

    private bool _isDefeated;

    private void Start()
    {
        vrags = mobs;
        rast = true;
        _isDefeated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDefeated || vrags > 0 || !other.CompareTag("Player"))
        {
            return;
        }

        _isDefeated = true;
        rast = false;

        if (win != null) win.SetActive(true);

        int earnedStars = 0;

        if (Player.MoveCounter <= NeedForOne)
        {
            if (s1 != null) s1.SetActive(true);
            earnedStars = 1;
        }
        if (Player.MoveCounter <= NeedForTwo)
        {
            if (s2 != null) s2.SetActive(true);
            earnedStars = 2;
        }
        if (Player.MoveCounter <= NeedForThree)
        {
            if (s3 != null) s3.SetActive(true);
            earnedStars = 3;
        }

        if (!string.IsNullOrEmpty(LvL))
        {
            int previousBest = PlayerPrefs.GetInt(LvL, 0);
            if (earnedStars > previousBest)
            {
                int delta = earnedStars - previousBest;
                int totalStars = PlayerPrefs.GetInt(TotalStarsPrefsKey, 0) + delta;

                PlayerPrefs.SetInt(TotalStarsPrefsKey, totalStars);
                PlayerPrefs.SetInt(LvL, earnedStars);
                PlayerPrefs.Save();
            }
        }

        Player.cam = false;
    }
}