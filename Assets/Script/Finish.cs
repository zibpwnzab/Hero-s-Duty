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
    void Start()
    {
        //PlayerPrefs.DeleteAll();
        vrags = mobs;
        _mesh = GetComponent<MeshRenderer>();
        _mesh.enabled = false;
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
    void OnTriggerEnter(Collider other)
    {
        
        if (vrags == 0) 
        {
            if (other.tag == "Player")
            {
                win.SetActive(true);
                if(Player.MoveCounter <= NeedForOne)
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
