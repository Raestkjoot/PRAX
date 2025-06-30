using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelectedButton;
    [SerializeField] private GameObject _settingsButton;
    [SerializeField] private GameObject _settingsBackButton;

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

    public void SelectBackButton()
    {
        EventSystem.current.SetSelectedGameObject(_settingsBackButton);
    }

    public void SelectSettingsButton()
    {
        EventSystem.current.SetSelectedGameObject(_settingsButton);
    }
}
