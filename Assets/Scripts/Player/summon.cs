using UnityEngine;

public class summon : MonoBehaviour
{
    private const string SkinPrefsKey = "scin";

    public GameObject[] player;

    private void Awake()
    {
        if (player == null || player.Length == 0) return;

        int skinIndex = Mathf.Clamp(PlayerPrefs.GetInt(SkinPrefsKey, 0), 0, player.Length - 1);
        if (player[skinIndex] != null)
        {
            Instantiate(player[skinIndex], transform);
        }
    }
}