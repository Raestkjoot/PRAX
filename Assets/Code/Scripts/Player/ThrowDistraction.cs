using UnityEngine;

public class ThrowDistraction : MonoBehaviour
{
    [SerializeField] private int _amountOfProjectiles = 4;

    [SerializeField] private Transform _throwingPoint;
    [SerializeField] private GameObject _projectileGO;
    [SerializeField] private ProjectilesUISystem _projectileUISystem;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _canonMesh;

    [SerializeField] private float _force;
    [SerializeField] private float _angle;
    [SerializeField] private float _aimSpeed;
    [SerializeField] private float _maxThrowDist = 10.0f;
    [SerializeField] private float _throwCooldown = 2.0f;

    private Rigidbody _projectile;

    private Vector2 _aimPoint;
    private Vector3 _throwDir;

    private Vector3 _positionDelta;
    private Vector3 _lastPosition;

    private float _cooldownTimerStart = 0.0f;
    private float _forceMultiplyer;

    private bool _isAiming = false;
    private bool _canThrow = true;

    public bool IsAiming()
    {
        return _isAiming;
    }

    public bool CanThrow()
    {
        bool offCooldown = Time.time - _cooldownTimerStart > _throwCooldown;
        _canThrow = offCooldown && _amountOfProjectiles > 0;

        return _canThrow;
    }

    public void Start()
    {
        _projectile = _projectileGO.GetComponent<Rigidbody>();

        if (_projectileUISystem != null)
        {
            _projectileUISystem.InitializeProjectiles(_amountOfProjectiles);
        }

        _lastPosition = transform.position;
    }

    void LateUpdate()
    {
        // update _lastPosition at the end of every update, otherwise the initial aim can be very off
        _lastPosition = transform.position;
    }

    public bool EnterAim(Vector2 preAimDir)
    {
        if (_projectileUISystem == null) return false;
        if (!CanThrow()) return false;
        if (_isAiming) { return true; }

        Time.timeScale = 0.3f;

        _isAiming = true;
        
        float x = transform.position.x + preAimDir.x * _maxThrowDist / 2f;
        float y = transform.position.z + preAimDir.y * _maxThrowDist / 2f;
        _aimPoint = new Vector2(x, y);
        VisualizeAim(preAimDir);
     
        _animator.SetBool("Aim", true);

        return true;
    }

    public void ExitAim()
    {
        if (!_isAiming) return;

        _isAiming = false;
        TrajectoryDrawer.Instance.DeleteTrajectory();
        _animator.SetBool("Aim", false);
    }

    public void Aim(Vector2 aimInput)
    {
        if (_projectileUISystem == null) return;
        if (!_isAiming) return;

        VisualizeAim(aimInput);
    }

    private void VisualizeAim(Vector2 aimInput)
    {
        // calculate new aimpoint
        _aimPoint += aimInput * Time.fixedDeltaTime * _aimSpeed;

        // now that the player can move during aiming
        // add half the player delta position to aim point
        // this way we keep the aiming point at the same relativ position
        _positionDelta = transform.position - _lastPosition;
        _lastPosition = transform.position;
        _aimPoint += new Vector2(_positionDelta.x, _positionDelta.z)/2.0f;

        Vector2 vecToAimPoint = _aimPoint - new Vector2(transform.position.x, transform.position.z);

        // keep point on circle if aim goes to far
        if (vecToAimPoint.magnitude > _maxThrowDist)
        {
            vecToAimPoint.Normalize();
            vecToAimPoint *= _maxThrowDist;
        }

        _aimPoint = vecToAimPoint + new Vector2(transform.position.x, transform.position.z);

        // calculate force multiplier
        _forceMultiplyer = (Mathf.Abs((new Vector2(transform.position.x, transform.position.z) - _aimPoint).magnitude)/10f);

        // TODO: For Mouse/Keyboard: Check that vecToAimPoint.magnitude > minThrowDist
        // If the player throws the distractor on top of themselves they might get stuck
        // Haven't tested it thoroughly though.
        // With controller it isn't a problem due to deadzone.

        // couldn't find a smarter way to do this
        _throwDir = new Vector3(_aimPoint.x - transform.position.x, 0, _aimPoint.y-transform.position.z);
        _throwDir.Normalize();
        _throwDir.y = _angle / 90f;
        _throwDir.Normalize();

        TrajectoryDrawer.Instance.UpdateTrajectory(
            _throwDir * _force * _forceMultiplyer, 
            _projectile, 
            _throwingPoint.position);

        
        _canonMesh.rotation = Quaternion.LookRotation(_canonMesh.forward, new Vector3(
            vecToAimPoint.x, 0.0f, vecToAimPoint.y));
    }

    public void TryThrow()
    {
        if (_projectileUISystem == null) return;
        if (!CanThrow()) { return; }
        if (!_isAiming) { return; }

        _canThrow = false;
        _cooldownTimerStart = Time.time;

        // instantiate distractor object and add force to it
        GameObject distractor = Instantiate(Resources.Load<GameObject>("Prefabs/Distractor"));
        distractor.transform.position = _throwingPoint.position;

        Rigidbody rb = distractor.GetComponent<Rigidbody>();
        rb.AddForce(_throwDir * _force * _forceMultiplyer);
        rb.angularVelocity = distractor.transform.forward * 7.0f;
        rb.angularDrag = 0.0f;
        
        TrajectoryDrawer.Instance.DeleteTrajectory();
        _isAiming = false;
        _animator.SetBool("Aim", false);
        _canonMesh.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

        _amountOfProjectiles--;
        _projectileUISystem.ConsumeProjectile();
    }
}
