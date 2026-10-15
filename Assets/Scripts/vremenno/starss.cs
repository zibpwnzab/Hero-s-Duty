using UnityEngine;
using UnityEngine.UI;

public class starss : MonoBehaviour
{
    private const string TotalStarsPrefsKey = "money";

    public Text stars;
    private int _cachedStars = -1;

    private void Start()
    {
        RefreshStarsText();
    }

    public void RefreshStarsText()
    {
        if (stars == null) return;

        int currentStars = PlayerPrefs.GetInt(TotalStarsPrefsKey, 0);
        if (currentStars != _cachedStars)
        {
            _cachedStars = currentStars;
            stars.text = $"звезд : {_cachedStars}";
        }
    }
}