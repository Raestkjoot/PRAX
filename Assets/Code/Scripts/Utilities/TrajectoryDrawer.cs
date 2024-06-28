using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryDrawer : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    
    private GameObject _aimIndicator;

    private LineRenderer _lineRenderer;
    private int _lineSegmentCount = 50;

    private List<Vector3> _linePoints = new List<Vector3>();

    public static TrajectoryDrawer Instance;

    private void Awake()
    {
        Instance = this;
        _lineRenderer = GetComponent<LineRenderer>();

        // aim indicator is now loaded as a prefab
        // having the indicator as a child object to the player
        // messes up the aiming
        _aimIndicator = Instantiate(Resources.Load<GameObject>("Prefabs/Scanner/MainAimPoint"));
        _aimIndicator.SetActive(false);
    }

    public void UpdateTrajectory(Vector3 forceVector, Rigidbody rigidbody, Vector3 startingPoint)
    {
        bool placedIndicator = false;
        _aimIndicator.SetActive(true);
        _lineRenderer.enabled = true;

        Vector3 velocity = (forceVector / rigidbody.mass) * Time.fixedDeltaTime;

        // todo: remove magic number (might be kinda hard because it get's very physical here)
        float flightDuration = (4 * velocity.y) / Physics.gravity.y;

        float stepTime = flightDuration / _lineSegmentCount;

        _linePoints.Clear();

        // calculate flight curve
        for (int i = 0; i < _lineSegmentCount; i++)
        {
            float stepTimePassed = stepTime * i;

            Vector3 movementVector = new Vector3
                (
                velocity.x * stepTimePassed,
                velocity.y * stepTimePassed - 0.5f * Physics.gravity.y * stepTimePassed * stepTimePassed,
                velocity.z * stepTimePassed
                );

            Vector3 position = -movementVector + startingPoint;

            _linePoints.Add(position);

            // see if the line hit an obstacle (or ground)
            RaycastHit hit;

            float nextStepTime = stepTime * (i + 1);

            Vector3 nextPosition = startingPoint + -new Vector3
                (
                velocity.x * nextStepTime,
                velocity.y * nextStepTime - 0.5f * Physics.gravity.y * nextStepTime * nextStepTime,
                velocity.z * nextStepTime
                );

            // raycast from current position in direction of next position
            if (Physics.Raycast(position, - position + nextPosition, out hit, 1f, _layerMask))
            {
                if (hit.collider != null)
                {
                    placedIndicator = true;
                    _aimIndicator.transform.position = hit.point;
                    break;
                }
            }
        }
        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(_linePoints.ToArray());

        if (!placedIndicator)
        {
            _aimIndicator.transform.position = _linePoints[_linePoints.Count - 1];
        }
    }

    public void DeleteTrajectory()
    {
        StartCoroutine(DelayedAimRemove());
        _linePoints.Clear();
        _lineRenderer.SetPositions(_linePoints.ToArray());
        _lineRenderer.enabled = false;
        Time.timeScale = 1f;
    }

    private IEnumerator DelayedAimRemove()
    {
        yield return new WaitForSeconds(0.3f);
        _aimIndicator.SetActive(false);
    }
}