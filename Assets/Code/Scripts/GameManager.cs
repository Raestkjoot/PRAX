using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    private float _distractionRange = 8f;

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetDistractionRange()
    {
        return _distractionRange;
    }
}