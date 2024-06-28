using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;

public class GameOver : Singleton<GameOver>
{
    [Header("UI's to control")]
    [SerializeField] GameObject _gameOverUI;
    [SerializeField] GameObject _confirmQuitUI;
    [Header("Buttons to be selected first")]
    [SerializeField] GameObject _firstButtonGOSelect;
    [SerializeField] GameObject _firstButtonConSelect;

    [Header("Scripts to Disable when GameOver")]
    [SerializeField] PlayerController _playerController;

    private GameObject _lastSelectedButton;

    public void OpenGameOverUI()
    {
        if (_gameOverUI.activeInHierarchy) { return; }
        _gameOverUI.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_firstButtonGOSelect);
        _playerController.enabled = false;

    }

    public void RestartLevel()
    {
        EnemyManager.Instance.UnsubscribeAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMenu()
    {
        EnemyManager.Instance.UnsubscribeAll();
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenConfirmUI()
    {
        _confirmQuitUI.SetActive(true);
        _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(_firstButtonConSelect);
    }

    public void CloseConfirmUI()
    {
        _confirmQuitUI.SetActive(false);
        EventSystem.current.SetSelectedGameObject(_lastSelectedButton);
    }

    public bool GetIsGameOver()
    {
        if (_gameOverUI != null && _gameOverUI.activeInHierarchy)
        {
            return true;
        }
        else return false;
    }
}