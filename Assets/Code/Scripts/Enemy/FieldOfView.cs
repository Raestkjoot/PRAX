using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;

    [SerializeField] private Material _baseMaterial;

    [SerializeField] private Color _idleColor;
    [SerializeField] private Color _cautiousColor;
    [SerializeField] private Color _alertedColor;

    private float _currentFOV, _currentViewDistance;
    private float _fov = 90.0f;
    private float _viewDistance = 10f;
    private int _rayCount = 50; // increase this variable to get more precise vision shadow effect
    private float _angleIncrease;

    private Mesh _mesh;

    private Vector3 _origin;
    private Vector3[] _vertices;
    private Vector2[] _uv;
    private int[] _triangles;
    private float _startingAngle;

    private bool _detectedPlayer = false;
    public bool DetectedPlayer
    {
        get { return _detectedPlayer; }
    }
    private Transform _playerTransform;


    private void Start()
    {
        _mesh = new Mesh();
        _vertices = new Vector3[_rayCount + 1 + 1];
        _uv = new Vector2[_vertices.Length];
        _triangles = new int[_rayCount * 3];
        GetComponent<MeshFilter>().mesh = _mesh;
        GetComponent<Renderer>().material = _baseMaterial;
        GetComponent<Renderer>().material.color = _idleColor;
        _currentFOV = _fov;
        _currentViewDistance = _viewDistance;
    }

    private void LateUpdate()
    {
        _currentFOV = Mathf.Lerp(_currentFOV, _fov, 0.2f);
        _currentViewDistance = Mathf.Lerp(_currentViewDistance, _viewDistance, 0.2f);

        _angleIncrease = _currentFOV / _rayCount;

        _vertices[0] = _origin;

        float angle = _startingAngle;
        int vertexIndex = 1;
        int triangleIndex = 0;
        _detectedPlayer = false;
        for (int i = 0; i <= _rayCount; i++)
        {
            RaycastHit hit;

            // we send out a ray and if it hits something we will set the next mesh point at the hit location
            // else we just set it at vision distance in the current angle
            if (Physics.Raycast(_origin, GetVectorFromAngle(angle), out hit, _currentViewDistance, _layerMask))
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    _detectedPlayer = true;
                    _playerTransform = hit.collider.transform;
                }

                _vertices[vertexIndex] = hit.point;
            }
            else
            {
                Vector3 vertex = _origin + GetVectorFromAngle(angle) * _currentViewDistance;
                _vertices[vertexIndex] = vertex;
            }

            if (i > 0)
            {
                _triangles[triangleIndex + 0] = 0;
                _triangles[triangleIndex + 1] = vertexIndex - 1;
                _triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= _angleIncrease;
        }

        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;
        _mesh.RecalculateBounds();
    }

    public void SetOrigin(Vector3 origin)
    {
        _origin = origin;
    }

    public void SetDirection(Vector3 dir)
    {
        _startingAngle = GetAngleFromVector(dir) + _fov / 2;
    }

    public Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }

    public float GetAngleFromVector(Vector3 dir)
    {
        float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    public Vector3 GetTargetPosition()
    {
        return _playerTransform.position;
    }

    public void SetFovDegrees(float fovDegrees)
    {
        if (_currentFOV == 0)
        {
            _currentFOV = fovDegrees;
        }
        _fov = fovDegrees;
    }

    public void SetVisionDistance(float distance)
    {
        if (_viewDistance == 0)
        {
            _currentViewDistance = _viewDistance;
        }
        _viewDistance = distance;
    }

    public void SetVisionState(VisionState state)
    {
        Color currentColor = GetComponent<Renderer>().material.color;
        Color newColor;

        float lerpSpeed = 0.1f;

        switch (state)
        {
            case VisionState.Red:
                newColor = Color.Lerp(currentColor, _alertedColor, lerpSpeed);
                break;
            case VisionState.Yellow:
                newColor = Color.Lerp(currentColor, _cautiousColor, lerpSpeed);
                break;
            case VisionState.White:
                newColor = Color.Lerp(currentColor, _idleColor, lerpSpeed);
                break;
            default:
                newColor = currentColor;
                break;
        }

        GetComponent<Renderer>().material.color = newColor;
    }

}