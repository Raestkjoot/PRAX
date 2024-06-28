using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCollider : MonoBehaviour
{
    [SerializeField] private int _thisLevel;
    [SerializeField] private GameObject _levelDigit;

    [SerializeField] private GameObject _selectedVisuals;
    [SerializeField] private GameObject _lockedVisuals;
    [SerializeField] private GameObject _completedVisuals;

    [SerializeField] private GameObject _levelPlato;
    [SerializeField] private Material _completedMaterial;

    [SerializeField] private GameObject _bushes;

    private LevelState _state;
    private LevelState _previousState;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) 
        {
            return;
        }

        LevelSelect.Instance.SetSelectedLevel(_thisLevel);
        PlayerController.OnInteractReady += LevelSelect.Instance.EnterSelectedLevel;

        _previousState = _state;
        _state = LevelState.Selected;
        
        _selectedVisuals.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        LevelSelect.Instance.SetSelectedLevel(-1);
        PlayerController.OnInteractReady -= LevelSelect.Instance.EnterSelectedLevel;

        _state = _previousState;
        _selectedVisuals.SetActive(false);
    }

    private void OnDisable()
    {
        PlayerController.OnInteractReady -= LevelSelect.Instance.EnterSelectedLevel;
    }

    private void Start()
    {
        int maxUnlocked = LevelSelect.Instance.GetMaxUnlockedLevel();

        if (_thisLevel < maxUnlocked)
        {
            _state = LevelState.Completed;
            _completedVisuals.SetActive(true);
            _levelPlato.GetComponent<MeshRenderer>().material = _completedMaterial;

            // let grow plants if this level was just finished
            if (maxUnlocked-1 == _thisLevel)
            {
                _bushes.GetComponent<PlantGrower>().SetGrow();
            }
        }
        else if (_thisLevel > maxUnlocked)
        {
            _state = LevelState.Locked;
            _lockedVisuals.SetActive(true);
            _levelDigit.SetActive(false);
        }
        else
        {
            _state = LevelState.Unlocked;
        }

        this.enabled = LevelSelect.Instance.CheckSelectedLevel(_thisLevel);
        gameObject.GetComponent<BoxCollider>().enabled = this.enabled;
    }
}
