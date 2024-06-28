using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceChangeManager : PersistentSingleton<InputDeviceChangeManager>
{
    private ControlScheme _curControlScheme;

    private int _ControllersConnected = 0;
    private int _ghostControllersConntected = 0;

    private InputAction _inputActionEnableMouse;
    private InputAction _inputActionEnableController;

    protected override void Awake()
    {
        base.Awake();

        _inputActionEnableMouse = new InputAction(binding: "<Keyboard>/escape");
        _inputActionEnableController = new InputAction(binding: "<Gamepad>/start");
    }

    private void Start()
    {
        InputSystem.onDeviceChange += CheckDeviceChange;

        var allGamepads = Gamepad.all;

        if (allGamepads.Count == 0)
        {
            SetControlScheme(ControlScheme.Keyboard);
        }
        else
        {
            foreach (var gamepad in allGamepads)
            {
                GamepadAdded(gamepad);
            }
        }
    }

    public void SetControlScheme(ControlScheme controlScheme)
    {
        _curControlScheme = controlScheme;

        if (_curControlScheme == ControlScheme.Controller )
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (_curControlScheme == ControlScheme.Keyboard )
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public ControlScheme CheckControlScheme()
    {
        return _curControlScheme;
    }

    private void CheckDeviceChange(InputDevice device, InputDeviceChange change)
    {
        var gamepad = device as Gamepad;

        if ( gamepad != null )
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    GamepadAdded(gamepad);
                    break;
                case InputDeviceChange.Removed:
                    GamepadRemoved(gamepad);
                    break;
            }
        }
    }

    private void GamepadAdded(Gamepad gamepad)
    {
        // Some controllers (PS4) will add a second device which messes
        // with the input and gives us double inputs and false values.
        // So we have this check to disable that second "ghost" device.
        if (_ControllersConnected > 0 && 
            gamepad.layout == "XInputControllerWindows")
        {
            InputSystem.DisableDevice(gamepad);
            _ghostControllersConntected++;
        }
        else
        {
            _ControllersConnected++;
        }

        SetControlScheme(ControlScheme.Controller);
    }

    private void GamepadRemoved(Gamepad gamepad)
    {
        if (_ghostControllersConntected > 0 &&
            gamepad.layout == "XInputControllerWindows")
        {
            _ghostControllersConntected--;
        }
        else
        {
            _ControllersConnected--;
        }

        if (_ControllersConnected == 0)
        {
            SetControlScheme(ControlScheme.Keyboard);
        }
    }

    private void EnableMouse(InputAction.CallbackContext context) =>
        SetControlScheme(ControlScheme.Keyboard);

    private void EnableController(InputAction.CallbackContext context) =>
        SetControlScheme(ControlScheme.Controller);

    private void OnEnable()
    {
        _inputActionEnableMouse.Enable();
        _inputActionEnableMouse.performed += EnableMouse;

        _inputActionEnableController.Enable();
        _inputActionEnableController.performed += EnableController;
    }

    private void OnDisable()
    {
        _inputActionEnableMouse.Disable();
        _inputActionEnableController.Disable();
    }
}

public enum ControlScheme
{
    Keyboard,
    Controller
}