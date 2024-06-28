using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScanPoint : MonoBehaviour
{
    [SerializeField] private float _spawnScannerIntervall = 0.3f;

    private float _distractionRange;
    private bool _readyToSpawn = true;

    private List<GameObject> _scanners = new List<GameObject>();

    // we don't want the distraction range indicator to be exact
    // we want it to be a tiny bit smaller than the actual range
    // (testers were getting frustrated when half of the enemy was out of range
    // and the distraction didn't trigger)
    // 2 is the exact range
    private float _underSellingParameter = 1.75f;

    // have a main scanner circle and only do the pulse when aim is stationary for some time
    void Start()
    {
        _distractionRange = GameManager.Instance.GetDistractionRange();
        transform.localScale = new Vector3(_distractionRange * _underSellingParameter, _distractionRange * _underSellingParameter, _distractionRange * _underSellingParameter);
        GetComponent<Renderer>().material.SetFloat("_Alpha", 0.15f);
    }

    void Update()
    {
        if (_readyToSpawn)
        {
            // spawn new scanner in interval
            StartCoroutine(SpawnScanner());
        }
    }

    private IEnumerator SpawnScanner()
    {
        _readyToSpawn = false;

        GameObject newScanner = Instantiate(Resources.Load<GameObject>("Prefabs/Scanner/SinglePuls"), transform.position, Quaternion.identity);
        newScanner.transform.parent = transform;

        _scanners.Add(newScanner);

        yield return new WaitForSeconds(_spawnScannerIntervall);

        _readyToSpawn = true;
    }

    void OnDisable()
    {
        foreach (var scanner in _scanners)
        {
            Destroy(scanner?.gameObject);
        }

        _readyToSpawn = true;
    }
}
