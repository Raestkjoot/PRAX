using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

// Rigidbody: Freeze rotations and disable gravity
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10.0f;
    [SerializeField] private float _speedWhileAiming = 4.0f;
    [SerializeField] private float _meshRotationSpeed = 800.0f;
    [SerializeField] float _meshTiltSpeed = 5.0f;
    [SerializeField] float _maxMeshTiltAngle = 20.0f;

    [SerializeField] private float _dashSpeed = 25.0f;
    [SerializeField] private float _dashDuration = 0.15f;
    [SerializeField] private float _dashCooldown = 0.5f;

    [SerializeField] private Transform _rotatingMesh;
    [SerializeField] Transform _tiltingMesh;
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _dashCooldownBar;

    private CharacterController _controller;

    private Vector2 _lastDir = Vector2.zero;
    private bool _isDashing = false;
    private bool _canDash = true;

    float _curTiltAngle = 0.0f;
    bool _isTilting = false;


    private FMOD.Studio.EventInstance _moveInstance;
    [SerializeField] private FMODUnity.EventReference _moveEvent;

    private FMOD.Studio.EventInstance _dashInstance;
    [SerializeField] private FMODUnity.EventReference _dashEvent;

    /// <summary>
    /// Handles player movement.
    /// Attempts to move the player in the given direction using the normal "walking" speed.
    /// If the player is dashing or aiming, it applies the relevant speed instead.
    /// </summary>
    /// <param name="dir"> Direction of movement input. </param>
    /// <param name="isAiming"> Is the player aiming? 
    /// This will make the player move slower. </param>
    public void Move(Vector2 dir, bool isAiming)
    {
        if (_isDashing) { return; }

        FMOD.Studio.PLAYBACK_STATE state;
        _moveInstance.getPlaybackState(out state);
        bool moveSoundPlaying = state != FMOD.Studio.PLAYBACK_STATE.STOPPED;

        _moveInstance.setParameterByName("RobotPitch", dir.magnitude);

        if (!moveSoundPlaying)
        {
            _moveInstance.start();
        }

        if (dir == Vector2.zero)
        {
            Idle();
            return;
        }

        _lastDir = dir;

        if (isAiming)
        {
            MoveCharacter(_speedWhileAiming, dir);
        }
        else
        {
            MoveCharacter(_speed, dir); 
        }
    }
    
    /// <summary>
    /// Plays idle sound. Should be called when the player is not moving.
    /// </summary>
    public void Idle()
    {
        if (_isDashing) { return; }
        //_moveInstance.start();
    }

    /// <summary>
    /// Attempts to make the player to dash in the given direction.
    /// Fails if the dash is on cooldown.
    /// </summary>
    /// <param name="dir"> Direction. </param>
    public async void Dash(Vector2 dir)
    {
        if (!_canDash) { return; }

        _isDashing = true;
        _canDash = false;

        _animator.SetBool("Dash", true);

        // stop moving audio
        _moveInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        // dash audio
        _dashInstance = FMODUnity.RuntimeManager.CreateInstance(_dashEvent);
        _dashInstance.start();
        _dashInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        
        if (_dashCooldownBar != null)
        {
            _dashCooldownBar.fillAmount = 0.0f;
        }


        // We use a while loop so we can hold onto the same direction value.
        float dashTimer = 0.0f;
        while (dashTimer < _dashDuration)
        {
            MoveCharacter(_dashSpeed, dir);
            dashTimer += Time.deltaTime;
            await Task.Yield();
        }

        _isDashing = false;
        _animator.SetBool("Dash", false);
        float dashCooldownTimer = 0.0f;

        while (dashCooldownTimer < _dashCooldown)
        {
            dashCooldownTimer += Time.deltaTime;

            if (_dashCooldownBar != null)
            {
                float dashCooldownProgressPercentage =
                    Mathf.Clamp(dashCooldownTimer / _dashCooldown, 0.0f, 1.0f);

                _dashCooldownBar.fillAmount = dashCooldownProgressPercentage;
            }
            await Task.Yield();
        }

        if (_dashCooldownBar != null)
        {
            _dashCooldownBar.fillAmount = 1.0f;
        }

        _canDash = true;
    }

    public Vector2 GetRotation()
    {
        float rot = Mathf.Deg2Rad * _rotatingMesh.rotation.eulerAngles.y;

        Vector2 forwardVec;
        forwardVec.y = Mathf.Cos(rot);
        forwardVec.x = Mathf.Sin(rot);

        return forwardVec;
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();

        _moveInstance = FMODUnity.RuntimeManager.CreateInstance(_moveEvent);
        _moveInstance.start();
        _moveInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void FixedUpdate()
    {

        UpdateMeshRotation();
        UpdateMeshTilt();
    }

    /// <summary>
    /// Move character.
    /// </summary>
    /// <remarks>
    /// Looks like CharacterController.Move works independently of FixedUpdate, 
    /// so it can be called from basically anywhere. 
    /// </remarks>
    /// <param name="speed"> How fast the character should move. </param>
    /// <param name="dir"> The direction of movement. </param>
    private void MoveCharacter(float speed, Vector2 dir)
    {
        Vector3 dirV3 = new Vector3(
            dir.x, 0.0f, dir.y);

        _controller.Move(
            speed * Time.deltaTime * dirV3);

        TiltTowards(dir.magnitude);
    }

    private void UpdateMeshRotation()
    {
        Vector3 dir = new Vector3(_lastDir.x, 0.0f, _lastDir.y);
        if (dir == Vector3.zero) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

        _rotatingMesh.rotation = Quaternion.RotateTowards(
            _rotatingMesh.rotation,
            targetRotation,
            _meshRotationSpeed * Time.deltaTime);
    }

    private void TiltTowards(float percentOfAngle)
    {
        _isTilting = true;

        float targetAngle = (percentOfAngle * _maxMeshTiltAngle);
        _curTiltAngle = Mathf.Lerp(_curTiltAngle, targetAngle, 
            _meshTiltSpeed * Time.deltaTime);
    }

    private void UpdateMeshTilt()
    {
        if (_isTilting)
        {
            _isTilting = false;
        }
        else if (_curTiltAngle > 0.0f)
        {
            _curTiltAngle = Mathf.Lerp(_curTiltAngle, 0.0f,
            _meshTiltSpeed * Time.deltaTime);
        }
        else
        {
            return;
        }

        _tiltingMesh.localEulerAngles = new Vector3(-_curTiltAngle, 180.0f, 0.0f);
    }

    private void OnDestroy()
    {
        _moveInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _moveInstance.release();
        _dashInstance.release();
    }
}
