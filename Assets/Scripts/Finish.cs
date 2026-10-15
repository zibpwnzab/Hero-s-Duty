using UnityEngine;
using UnityEngine.Serialization;

public class Finish : MonoBehaviour
{
    private const string TotalStarsPrefsKey = "money";

    public int NeedForOne;
    public int NeedForTwo;
    public int NeedForThree;

    public GameObject s1;
    public GameObject s2;
    public GameObject s3;

    public GameObject win;
    public static int vrags;
    public int mobs;
    public string LvL;

    private MeshRenderer _mesh;
    private bool _isWinTriggered;

    private void Start()
    {
        vrags = mobs;
        _mesh = GetComponent<MeshRenderer>();
        if (_mesh != null) _mesh.enabled = false;
        _isWinTriggered = false;
    }

    private void Update()
    {
        if (vrags <= 0 && _mesh != null && !_mesh.enabled)
        {
            _mesh.enabled = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isWinTriggered || vrags > 0 || !other.CompareTag("Player"))
        {
            return;
        }

        if (Player.canGo)
        {
            _isWinTriggered = true;
            TriggerVictory();
        }
    }

    private void TriggerVictory()
    {
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

        SaveProgress(earnedStars);
        Player.cam = false;
    }

    private void SaveProgress(int earnedStars)
    {
        if (string.IsNullOrEmpty(LvL)) return;

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
}