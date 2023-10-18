using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class summon : MonoBehaviour
{
    public GameObject[] player;
    private int scin;

    void Start()
        
    {
        scin = PlayerPrefs.GetInt("scin");
        Instantiate(player[scin], gameObject.transform);
        
    }
}
