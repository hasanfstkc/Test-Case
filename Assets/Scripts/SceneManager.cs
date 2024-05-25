using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void NextScene()
    {
        var index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (index == UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index + 1);
        }
    }
}
