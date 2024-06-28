using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelectedButton;
    [SerializeField] private GameObject _settingsButton;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(_firstSelectedButton);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void BackFromSettings()
    {
        EventSystem.current.SetSelectedGameObject(_settingsButton);
    }
}
