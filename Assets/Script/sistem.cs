using UnityEngine;
using UnityEngine.UI;

public class sistem : MonoBehaviour
{

    public Text Svipe;
    void Update()
    {
        Svipe.text = "шагов : " + Player.MoveCounter;
    }
}
