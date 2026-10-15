using UnityEngine;
using UnityEngine.SceneManagement;

public class NextOrNow : MonoBehaviour
{
    public string nextLvl;

    public void Next()
    {
        if (!string.IsNullOrEmpty(nextLvl))
        {
            SceneManager.LoadScene(nextLvl);
        }
    }

    public void Reload()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}