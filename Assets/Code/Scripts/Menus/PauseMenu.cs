using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PauseMenu : Singleton<PauseMenu>
{
    [SerializeField] GameObject _pauseMenuUI;
    [SerializeField] GameObject _confirmQuitUI;

    [Header("First Buttons in the Menus")]
    [SerializeField] GameObject _firstPauseButtonSelect;
    [SerializeField] GameObject _firstConfirmQuitSelect;
    private GameObject _lastSelectedButton;
    [SerializeField] GameObject _restartLevelButton;

    private bool _isGamePaused;

    public void ToggleGamePaused()
    {
        if (CloseMultipleUIs()) { return; }
        if (GameOver.Instance.GetIsGameOver()) { return; }
        _isGamePaused = !_isGamePaused;

        _pauseMenuUI.SetActive(_isGamePaused);
        if (_pauseMenuUI.activeInHierarchy)
        {
            SetSelectedButton(_firstPauseButtonSelect);
            _firstPauseButtonSelect.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
        Time.timeScale = _isGamePaused ? 0.0f : 1.0f;
    }

    public bool GetIsGamePaused()
    {
        return _isGamePaused;
    }

    public void RestartLevel()
    {
        // un-pause time and Unsubscribe Enemies
        EnemyManager.Instance.UnsubscribeAll();
        ToggleGamePaused();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenConfirmQuitUI()
    {
        _lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        _confirmQuitUI.SetActive(true);
        SetSelectedButton(_firstConfirmQuitSelect);
    }
    public void CloseConfirmQuitUI()
    {
        _confirmQuitUI.SetActive(false);
        SetSelectedButton(_lastSelectedButton);
    }

    private bool CloseMultipleUIs()
    {
        if (_confirmQuitUI.activeInHierarchy && _pauseMenuUI.activeInHierarchy)
        {
            CloseConfirmQuitUI();
            return true;
        }
        else return false;
    }
    public void QuitToMenu()
    {
        // unpause time
        Time.timeScale = 1.0f;
        // If enemyManager is active in the Scene: unsubscribe Enemies
        if (EnemyManager.Instance != null && EnemyManager.Instance.isActiveAndEnabled)
        {
            EnemyManager.Instance.UnsubscribeAll();
        }
        SceneManager.LoadScene("MainMenu");
    }

    private void SetSelectedButton(GameObject uiButton)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(uiButton);
    }

    private void Start()
    {
        if (_pauseMenuUI == null)
        {
            Debug.LogWarning("Paused Menu UI not assigned. Remember to also " +
                "assign functions in this class to buttons in the Pause Menu UI");
        }
    }
}