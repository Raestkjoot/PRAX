using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    protected ActionState _actionState = ActionState.Idle;

    [SerializeField] float _alertedActivationTime, _alertedCalmDownTime, _searchTime;
    [SerializeField] float _idleVisionLen, _cautiousVisionLen, _alertVisionLen;
    [SerializeField] float _idleFovDegrees, _cautiousFovDegrees, _alertFovDegrees;

    [SerializeField] private GameObject _cautiounIndicator;
    [SerializeField] private GameObject _alertedIndicator;

    [SerializeField] private int _idleRotationRange, _searchSpeed;

    [SerializeField] private float _idleSpeed, _cautiousSpeed, _alertSpeed;

    protected FieldOfView _fov;
    protected NavMeshAgent _navAgent;

    private float _searchTimerStart, _alertTimerStart, _escapeTimerStart;
    private bool _hasLostEyeContact = false;


    private float _idleRotationSpeed = 5f;
    private float _startRotationValue, _nextRotationValue;
    private int _directionSwitcher = -1;
    private Vector3 _startPosition;
    private float _distToTargetThreshold;


    protected virtual void Start()
    {
        EnemyManager.Instance.RegisterEnemy(this);

        DistractorScript.OnActive += Distracted;

        _navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        _startRotationValue = transform.rotation.eulerAngles.y;
        _startPosition = transform.position;
        _nextRotationValue = _startRotationValue + _idleRotationRange / 2;

        // instantiate fov prefab and set FOV
        _fov = Instantiate(Resources.Load<GameObject>("Prefabs/FOV"), Vector3.zero, Quaternion.identity).GetComponent<FieldOfView>();
        SetFOV();
    }

    protected virtual void Update()
    {
        if (_fov.DetectedPlayer)
        {
            if (_actionState != ActionState.Cautious && _actionState != ActionState.Alerted)
            {
                // Start alert timer
                _alertTimerStart = Time.time;

                // change state and inform EnemyManager
                EnemyManager.Instance.EnemyChangedState(_actionState, ActionState.Cautious, gameObject);
                _actionState = ActionState.Cautious;
                
                // set speed and activate indicator
                _navAgent.speed = _cautiousSpeed;
                _cautiounIndicator.SetActive(true);
            }
        }

        SetFOV();
        DecideOnAction();
    }

    private void DecideOnAction()
    {
        switch (_actionState)
        {
            case ActionState.Alerted:
                AlertedChase();
                break;
            case ActionState.Cautious: // player is detected (triggers the vision detection)
                Chase();
                _distToTargetThreshold = 0.0f; // we want the enemy to touch the player
                break;
            case ActionState.Distracted:
                _cautiounIndicator.SetActive(true);
                _distToTargetThreshold = 2f; // enemy just searches the area
                _actionState = ActionState.Searching; // logic in searching takes care of correct behavior
                break;
            case ActionState.Searching:
                Search();
                break;
            case ActionState.Idle:
                Rotate();
                break;
            case ActionState.GoingBack:
                _distToTargetThreshold = 0.5f; // we want the enemy to always return to approx. the same position
                GoBack();
                break;
            default:
                break;
        }
    }

    protected void SetFOV()
    {
        _fov.SetOrigin(transform.position);
        _fov.SetDirection(transform.forward);

        switch (_actionState)
        {
            // distracted, cautious and searching are all the same as of now
            case ActionState.Distracted:
            case ActionState.Cautious:
            case ActionState.Searching:
                _fov.SetFovDegrees(_cautiousFovDegrees);
                _fov.SetVisionDistance(_cautiousVisionLen);
                _fov.SetVisionState(VisionState.Yellow);
                break;
            case ActionState.Alerted:
                _fov.SetFovDegrees(_alertFovDegrees);
                _fov.SetVisionDistance(_alertVisionLen);
                _fov.SetVisionState(VisionState.Red);
                break;
            default:
                _fov.SetFovDegrees(_idleFovDegrees);
                _fov.SetVisionDistance(_idleVisionLen);
                _fov.SetVisionState(VisionState.White);
                break;
        }
    }

    private void GoBack()
    {
        _navAgent.SetDestination(_startPosition);
        
        Vector2 vecToStart = new Vector2(_startPosition.x - transform.position.x, _startPosition.z - transform.position.z);

        if (vecToStart.magnitude < _distToTargetThreshold)
        {
            _actionState = ActionState.Idle;
        }
    }

    private void AlertedChase()
    {
        // player escaped enemy vision for long enough
        if (_hasLostEyeContact && Time.time - _escapeTimerStart > _alertedCalmDownTime)
        {
            _alertedIndicator.SetActive(false);
            _cautiounIndicator.SetActive(true);

            EnemyManager.Instance.EnemyChangedState(_actionState, ActionState.Cautious, gameObject);
            _actionState = ActionState.Cautious;

            _navAgent.speed = _cautiousSpeed;

            return;
        }

        _navAgent.SetDestination(_fov.GetTargetPosition());

        // hasn't previously lost eye contact but now lost sight of player
        if (!_hasLostEyeContact && !_fov.DetectedPlayer)
        {
            _hasLostEyeContact = true;
            _escapeTimerStart = Time.time;
        }
        // player was detected again
        else if (_hasLostEyeContact && _fov.DetectedPlayer)
        {
            _hasLostEyeContact = false;
        }
    }

    private void Chase()
    {
        // player was in sight for long enough -> alerted
        if (_fov.DetectedPlayer && Time.time - _alertTimerStart > _alertedActivationTime)
        {
            _cautiounIndicator.SetActive(false);
            _alertedIndicator.SetActive(true);

            EnemyManager.Instance.EnemyChangedState(_actionState, ActionState.Alerted, gameObject);
            _actionState = ActionState.Alerted;

            _navAgent.speed = _alertSpeed;

            _hasLostEyeContact = false;
            return;
        }

        if (!_fov.DetectedPlayer)
        {
            EnemyManager.Instance.EnemyChangedState(_actionState, ActionState.Searching, gameObject);
            _actionState = ActionState.Searching;
            _navAgent.speed = _idleSpeed;
        }
        else
        {
            _navAgent.SetDestination(_fov.GetTargetPosition());
        }
    }

    private void Search()
    {
        // go to position were player was last seen
        if (_navAgent.remainingDistance > _distToTargetThreshold)
        {
            _searchTimerStart = Time.time;
            return;
        }

        if (_searchTime > Time.time - _searchTimerStart)
        {
            transform.RotateAround(transform.position, Vector3.up, _searchSpeed * Time.deltaTime);
        }
        else
        {
            _cautiounIndicator.SetActive(false);
            _navAgent.speed = _idleSpeed;
            _actionState = ActionState.GoingBack;
        }
    }

    protected void Rotate()
    {
        // the Quaternion.Slerp slows down significantly if we want to go to the exact rotation
        // making it stop a bit earlier creates more natural rotation behavior
        float rotationTolerance = 1.5f;
        float rotationDiff = Mathf.Abs(transform.rotation.eulerAngles.y - _nextRotationValue) % 360;

        if (rotationDiff > rotationTolerance)
        {
            Quaternion targetRot = Quaternion.Euler(new Vector3(0, _nextRotationValue, 0));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _idleRotationSpeed);
        }
        else
        {
            _nextRotationValue = _startRotationValue + (_directionSwitcher * _idleRotationRange / 2);
            _directionSwitcher = _directionSwitcher * -1;
        }
    }

    public ActionState GetState()
    {
        return _actionState;
    }

    private void Distracted()
    {
        // enemy cannot be distracted when enemy is cautious or alerted
        if (_actionState == ActionState.Cautious || _actionState == ActionState.Alerted) return;

        Vector3 distractionPoint = DistractorScript.DistractionPoint;

        if (Vector3.Distance(transform.position, distractionPoint) <
            GameManager.Instance.GetDistractionRange())
        {
            _searchTimerStart = Time.time;
            _actionState = ActionState.Distracted;
            _navAgent.speed = _cautiousSpeed;

            bool destination = _navAgent.SetDestination(distractionPoint);
        }
    }

    // When enemy catches player -> restart level
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LevelProgress.Instance.ShowGameOver();
            // disable enemy that touches the player
            this.enabled = false;
        }
    }

    public void UnsubscribeDistractor()
    {
        DistractorScript.OnActive -= Distracted;
    }
}