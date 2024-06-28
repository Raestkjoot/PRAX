using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UIElements;

public class Prompt : MonoBehaviour
{
    [SerializeField] private int _promptNumber;
    [Header("UI to Control")]
    [SerializeField] GameObject _text;
    [SerializeField] GameObject _tutPanelUI;
    [SerializeField] GameObject _oldPrompt; 
    [SerializeField] Animator _tutAnimator;
    [SerializeField] GameObject _pressToDismiss;

    [Header("PollutionMachine related")]
    [SerializeField] PollutionMachine _pollumachine;

    private InputActions _inputActions;
    private InputAction _dismissPrompt;
  

    private void OnEnable()
    {
        if (_oldPrompt.activeInHierarchy && _promptNumber != 0)
        {
            _oldPrompt.SetActive(false);
            _oldPrompt.transform.parent.gameObject.SetActive(false); // Setting the parent to false in case of Another UI panel
        }

        gameObject.SetActive(true);
        _tutPanelUI.SetActive(true);
        _text.SetActive(true);

        _inputActions = new InputActions();
        // Needs to be set after the UI panel is active:
        _tutAnimator = _tutPanelUI.GetComponent<Animator>();
        _tutAnimator.SetBool("OpenTutUI", true);

        if (_promptNumber == 2)
        {
            _pressToDismiss.SetActive(false);
            if (_dismissPrompt != null)
            {
                DisableInput();
            }
        }
        else
        {
            EnableInput();
            _pressToDismiss.SetActive(true);
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {

        if (_promptNumber == 2 && _pollumachine.GetIsActive())
        {
            // DO NOT REMOVE PROMPT
            DisableInput();
        }
        if (_promptNumber == 2 && !_pollumachine.GetIsActive())
        {
            DisableUI();

        }
        else if (_promptNumber != 2)
        {
            DisableUI();
            DisableInput();
        }
    }

    private void Update()
    {
        if (!_pollumachine.GetIsActive() && _promptNumber == 2)
        {
            _pressToDismiss.SetActive(true);
            EnableInput();
        }

        // Don't show the prompt if GameOver is active:
        if (GameOver.Instance.GetIsGameOver())
        {
            DisableUI();
        }
    }

    private void EnableInput()
    {
        if (_promptNumber == 2)
        {
            _inputActions.Enable();
        }
        _dismissPrompt = _inputActions.IntroScene.NextParagOrLvl;
        _dismissPrompt.performed += Interact;
        _dismissPrompt.Enable();
    }

    private void DisableInput()
    {
        _dismissPrompt.performed -= Interact;
        _dismissPrompt.Disable();
        if (_promptNumber == 2)
        {
            _inputActions.Disable();
            _dismissPrompt = null;
        }
    }
    private void DisableUI()
    {
        //_tutAnimator.SetBool("OpenTutUI", false);

        _tutPanelUI.SetActive(false);
        _text.SetActive(false);
        _pressToDismiss.SetActive(false);
        enabled = false;
    }

    private void OnDisable()
    {
        _dismissPrompt?.Disable();
    }
}
