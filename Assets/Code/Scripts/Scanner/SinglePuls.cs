using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePuls : MonoBehaviour
{
    [SerializeField] private float _growSpeed = 1.5f;
    [SerializeField] private float _liveTime = 0.3f;

    private float _growAcceleration = 0.0f;
    private float _alphaDecreaseSum = 0.0f;

    private float _alphaDecreaseSpeed = 0.45f;
    private float _growIncreaseSpeed = 0.9f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimedDestruction());
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(1, 1, 1) * (_growSpeed + _growAcceleration) * Time.deltaTime;
        _growAcceleration += _growIncreaseSpeed * Time.deltaTime;
        _alphaDecreaseSum += _alphaDecreaseSpeed * Time.deltaTime;

        GetComponent<Renderer>().material.SetFloat("_Alpha", 4 - _alphaDecreaseSum);
    }

    private IEnumerator TimedDestruction()
    {
        yield return new WaitForSeconds(_liveTime);
        Destroy(gameObject);
    }
}
