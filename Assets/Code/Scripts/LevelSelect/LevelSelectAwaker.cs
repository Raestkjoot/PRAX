using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectAwaker : MonoBehaviour
{
    [SerializeField] private Transform _grassLand;
    [SerializeField] private Transform _cityLand;

    private void Awake()
    {
        LevelSelect.Instance.StartUp(_grassLand, _cityLand);
    }
}
