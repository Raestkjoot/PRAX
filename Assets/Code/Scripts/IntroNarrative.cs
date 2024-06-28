using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroNarrative : MonoBehaviour
{
    [SerializeField] private CanvasGroup _logoCGroup; //logo
    [SerializeField] private CanvasGroup _controllerCGroup; //Controller
    [SerializeField] private CanvasGroup _TextCGroup; // Text
    [SerializeField] private float _duration;
    private bool _textActive = false;


    private InputActions _inputActions;
    private InputAction _nextParagOrLvl;

    private AsyncOperation _asyncLevelLoad;

    private void Start()
    {
        HandleFadeIns();

        _asyncLevelLoad = SceneManager.LoadSceneAsync("MainMenu");
        _asyncLevelLoad.allowSceneActivation = false;
    }
    private void Awake()
    {
        _inputActions = new InputActions();
    }

    IEnumerator FadeIn(CanvasGroup canvasGroup)
    {
        float timeElasted = 0;

        while (timeElasted < _duration)
        {
            timeElasted += Time.deltaTime;
            // Use linear interpolation: t = counter / duration
            float alpha = Mathf.Lerp(0, 1, timeElasted / _duration);

            canvasGroup.alpha = alpha;
            //Wait for a frame before continue
            yield return null;
        }
        // Start FadeOut:
        HandleFadeOuts();
    }

    IEnumerator FadeOut(CanvasGroup canvasGroup)
    {
        float timeElasted = 0;

        while (timeElasted < _duration)
        {
            timeElasted += Time.deltaTime;
            // Same as above but fade out
            float alpha = Mathf.Lerp(1, 0, timeElasted / _duration);

            canvasGroup.alpha = alpha;
            //Wait for a frame before continue
            yield return null;
        }
        canvasGroup.gameObject.SetActive(false);
        // start new fade in when old fade out has stopped:
        HandleFadeIns();
    }

    private void HandleFadeIns()
    {
        if (_logoCGroup.gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeIn(_logoCGroup));
        }
        else if (_controllerCGroup.gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeIn(_controllerCGroup));
        }
        else if (_TextCGroup.gameObject.activeInHierarchy)
        {
            // Fade the text in, but not out.
            StartCoroutine(FadeIn(_TextCGroup));
            // here we active the NextParagrah thingie
            _textActive = true;
        }
    }

    private void HandleFadeOuts()
    {
        if (_logoCGroup.gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOut(_logoCGroup));
        }
        else if (_controllerCGroup.gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOut(_controllerCGroup));
        }
    }

    private void NextParagOrLvl(InputAction.CallbackContext context)
    {
        // Check control scheme
        var control = context.control;
        var binding = context.action.GetBindingForControl(control).Value;
        if (binding.groups == "Keyboard")
        {
            InputDeviceChangeManager.Instance
                .SetControlScheme(ControlScheme.Keyboard);
        }
        if (_textActive)
        {
            _asyncLevelLoad.allowSceneActivation = true;
            // make sure we cannot call this input again.
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _nextParagOrLvl = _inputActions.IntroScene.NextParagOrLvl;
        _nextParagOrLvl.Enable();
        _nextParagOrLvl.performed += NextParagOrLvl;
    }

    private void OnDisable()
    {
        _nextParagOrLvl.Disable();
    }
}