using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSceneScript : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
