using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(ThrowDistraction))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private ThrowDistraction _throwDistraction;

    private Vector2 _dir = Vector2.zero;
    private Vector2 _aimDir = Vector2.zero;
    private Vector2 _lastAimDir = Vector2.zero;

    public delegate void InteractReady();
    public static event InteractReady OnInteractReady;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _throwDistraction = GetComponent<ThrowDistraction>();
    }

    private void OnMove(InputValue value)
    {
        _dir = value.Get<Vector2>();
    }

    private void OnAim(InputValue value)
    {
        _aimDir = value.Get<Vector2>();
        _throwDistraction.Aim(_aimDir);

        // avoid stuttering aim movement when thumbstick does not change direction
        if (_aimDir.magnitude > 0)
        {
            _lastAimDir = _aimDir;
        }
        else
        {
            _lastAimDir = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        _playerMovement.Move(_dir, _throwDistraction.IsAiming());

        // updating aim here as OnMove is only called when the move input changes
        // we need to update the aim constantly
        // we are passing in a zero vector
        // because we don't want to update the aiming twice
        // which would double the aim speed
        _throwDistraction.Aim(_lastAimDir);
    }

    private void OnDash()
    {
        if (PauseMenu.Instance.GetIsGamePaused() || GameOver.Instance.GetIsGameOver()) { return; }
        _playerMovement.Dash(_playerMovement.GetRotation());

        _throwDistraction.ExitAim();
    }

    private void OnThrowTrigger(InputValue value)
    {
        if (value.isPressed)
        {
            _throwDistraction.EnterAim(_playerMovement.GetRotation());
        }
        else
        {
            _throwDistraction.TryThrow();
        }
    }

    private void OnInteract()
    {
        OnInteractReady?.Invoke();
    }

    private void OnEscape(InputValue value)
    {
        PauseMenu.Instance.ToggleGamePaused();
        InputDeviceChangeManager.Instance.CheckControlScheme();
    }
}
