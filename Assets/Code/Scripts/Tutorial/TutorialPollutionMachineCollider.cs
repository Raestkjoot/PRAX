using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPollutionMachineCollider : MonoBehaviour
{
    [SerializeField] private Material _orignalMaterial;
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private List<Renderer> _polluterParts;
    [SerializeField] private GameObject _polluter;
    [SerializeField] private bool _isDeactivated = false;
    // to check whether the pollusionMachine is still active or not
    [SerializeField] private PollutionMachine _pollutionMachine;
    [SerializeField] private Prompt _prompt;



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _pollutionMachine.GetIsActive())
        {
            foreach (var renderer in _polluterParts)
            {
                renderer.material = _orignalMaterial;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _pollutionMachine.GetIsActive())
        {
            foreach (Renderer renderer in _polluterParts)
            {
                renderer.material = _highlightMaterial;
            }
            if (_prompt != null) 
            {
                _prompt.enabled = true;
            }
        }
    }

    private void Update()
    {
        if (!_pollutionMachine.GetIsActive() && !_isDeactivated) 
        {
            // to make sure we don't run this every update after pollution machine has been turned off
            _isDeactivated = true;
            foreach (var renderer in _polluterParts)
            {
                renderer.material = _orignalMaterial;
            }
        }
        if (_isDeactivated) { gameObject.SetActive(false); }
    }
}
