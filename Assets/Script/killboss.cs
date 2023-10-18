using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killboss : MonoBehaviour
{
    public static bool rast;
    public int NeedForOne;
    public int NeedForTwo;
    public int NeedForThree;


    public GameObject s1;
    public GameObject s2;
    public GameObject s3;

    public GameObject win;
    public static int vrags;
    public int mobs;
    void Start()
    {
        vrags = mobs;
        rast = true; 
    }

    public string LvL;
    private int stars;
    private int Allstars;
    private int Lstars;
    private int delta;
    void OnTriggerEnter(Collider other)
    {

        if (vrags == 0)
        {
            rast = false;
            if (other.tag == "Player")
            {
                
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

}
