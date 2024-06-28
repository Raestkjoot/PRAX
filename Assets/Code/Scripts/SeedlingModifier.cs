using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedlingModifier : MonoBehaviour
{
    private float _targetY;
    [SerializeField] private float _growSpeed = 1f;
    [SerializeField] private float _rotationSpeed = 1f;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        transform.localScale *= Random.Range(0.8f, 1.5f);

        _targetY = transform.position.y;
        Vector3 underGroundPos = transform.position;
        underGroundPos.y -= 2.7f; // cannot find a quick way to get the actual size of the plant yet 
        transform.position = underGroundPos;
    }

    void Update()
    {
        if (transform.position.y < _targetY)
        {
            transform.position += new Vector3(0, 1, 0) * _growSpeed * Time.deltaTime;
            transform.RotateAround(transform.position, Vector3.up, _rotationSpeed * Time.deltaTime);
        }
    }
}
