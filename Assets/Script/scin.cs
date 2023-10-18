using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scin : MonoBehaviour
{
    public int ScinPlayer;
    private int money;
    public int price;
    //[SerializeField] 
    public void  Playerscin()
    {
        money = PlayerPrefs.GetInt("money");

        if (money >= price)
        {
            PlayerPrefs.SetInt("scin", ScinPlayer);
            money -= price;
            PlayerPrefs.SetInt("money", money);
            Debug.Log(money);
            SceneManager.LoadScene("menu");
            
        }
        else
        {
            Debug.Log(money);
            SceneManager.LoadScene("menu");
            
        }
    }
}