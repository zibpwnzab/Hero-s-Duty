using UnityEngine;

public class zeroStars : MonoBehaviour
{
    public void Zero()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}