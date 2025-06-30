using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] private CanvasGroup _logo; //logo
    [SerializeField] private CanvasGroup _text; // Text
    [SerializeField] private float _logoShowDuration = 1.0f;
    [SerializeField] private float _fadeInOutDuration = 0.3f;

    private bool _skipRequested = false;
    private bool _introSequenceDone = false;
    private InputAction _nextAction;

    private AsyncOperation _asyncLevelLoad;

    private void Start()
    {
        _logo.alpha = 0.0f;
        _text.alpha = 0.0f;

        _nextAction = new InputActions().IntroScene.Next;
        _nextAction.Enable();
        _nextAction.performed += Next;

        StartCoroutine(HandleIntroSequence());

        _asyncLevelLoad = SceneManager.LoadSceneAsync("MainMenu");
        _asyncLevelLoad.allowSceneActivation = false;
    }

    private IEnumerator HandleIntroSequence()
    {
        float elapsedTime = 0.0f;

        // Fade in logo
        while (elapsedTime < _fadeInOutDuration)
        {
            elapsedTime += Time.deltaTime;
            _logo.alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / _fadeInOutDuration);

            if (_skipRequested) 
            { 
                break;
            }
            yield return null;
        }
        _logo.alpha = 1.0f;

        // Keep logo visible for a duration
        elapsedTime = 0.0f;
        while (elapsedTime < _logoShowDuration)
        {
            elapsedTime += Time.deltaTime;

            if (_skipRequested)
            {
                break;
            }
            yield return null;
        }

        // Fade out logo
        elapsedTime = 0.0f;
        while (elapsedTime < _fadeInOutDuration)
        {
            elapsedTime += Time.deltaTime;
            _logo.alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / _fadeInOutDuration);

            if (_skipRequested) 
            { 
                _skipRequested = false;
                break;
            }
            yield return null;
        }
        _logo.alpha = 0.0f;

        // Fade in intro text
        elapsedTime = 0.0f;
        _introSequenceDone = true;
        while (elapsedTime < _fadeInOutDuration)
        {
            elapsedTime += Time.deltaTime;
            _text.alpha = Mathf.Lerp(0.0f, 1.0f, elapsedTime / _fadeInOutDuration);

            if (_skipRequested) 
            {
                break;
            }
            yield return null;
        }
        _text.alpha = 1.0f;

    }

    private void Next(InputAction.CallbackContext context)
    {
        _skipRequested = true;

        // Check control scheme
        var control = context.control;
        var binding = context.action.GetBindingForControl(control).Value;
        if (binding.groups == "Keyboard")
        {
            InputDeviceChangeManager.Instance
                .SetControlScheme(ControlScheme.Keyboard);
        }
        else if (binding.groups == "Controller")
        {
            InputDeviceChangeManager.Instance
                .SetControlScheme(ControlScheme.Controller);
        }

        // If intro is done, then activate the menu scene
        if (_introSequenceDone)
        {
            _asyncLevelLoad.allowSceneActivation = true;
            enabled = false;
        }
    }

    private void OnDisable()
    {
        _nextAction.Disable();
    }
}