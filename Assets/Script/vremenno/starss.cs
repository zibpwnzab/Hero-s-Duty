using UnityEngine;
using UnityEngine.UI;

public class starss : MonoBehaviour
{
    public Text stars;
    private int st;
    void Update()
    {
        st = PlayerPrefs.GetInt("money");
        stars.text = "звезд : " + st;
    }
}
