using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushMossModifier : MonoBehaviour
{
    [SerializeField] private GameObject[] _parts;
    [SerializeField] private Material[] _materials;

    // Start is called before the first frame update
    void Start()
    {
        // set random y rotation
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        transform.localScale *= Random.Range(0.8f, 1.5f);

        foreach (var part in _parts)
        {
            // distribute materials randomly to make everything look more natural
            part.GetComponent<MeshRenderer>().material = _materials[Random.Range(0, _materials.Length)];
        }
    }
}
