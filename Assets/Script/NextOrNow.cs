using UnityEngine;
using UnityEngine.SceneManagement;

public class NextOrNow : MonoBehaviour
{

    private int mobs;
    public string nextLvl;
    public void Start()
    {
        
    }
    public void Next()
    {
        SceneManager.LoadScene(nextLvl);
    }
    public void Reload()
    {
        Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
        //SceneManager.LoadScene(ThisLvl);
    }
}
