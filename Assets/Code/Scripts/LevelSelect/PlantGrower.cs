using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrower : MonoBehaviour
{
    private float _growSpeed = 0.075f;
    private Vector3 _targetScale;

    private bool _grow;

    // Start is called before the first frame update
    void Start()
    {
        if (!_grow)
        {
            return;
        }

        // add some randomness to the growtime so it appears more natural
        _growSpeed += Random.Range(-_growSpeed / 10, _growSpeed / 10);

        // set random y rotation
        _targetScale = transform.localScale;// * Random.Range(0.8f, 1.5f);

        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
    }

    void Update()
    {
        if (!_grow)
        {
            return;
        }

        Vector3 newLocalScale = transform.localScale + new Vector3(0, 1, 0) * _growSpeed * Time.deltaTime;

        if (newLocalScale.magnitude < _targetScale.magnitude)
        {
            transform.localScale = newLocalScale;
        }
        else
        {
            transform.localScale = _targetScale;
        }
    }

    public void SetGrow()
    {
        _grow = true;
    }

}
