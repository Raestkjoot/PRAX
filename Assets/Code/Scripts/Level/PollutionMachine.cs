using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionMachine : MonoBehaviour
{
    [SerializeField] private ParticleSystem _pollutionParticles;
    [SerializeField] private Renderer _onOffIndicator;
    [SerializeField] private Material _materialMachineOff;
    [SerializeField] private GameObject _interactPrompt;

    private FMOD.Studio.EventInstance _polluterSoundInstance;
    [SerializeField] private FMODUnity.EventReference _polluterSoundEvent;

    private bool _isActive = true;

    public void Deactivate()
    {
        if (_isActive == false) { return; }
        _isActive = false;

        _onOffIndicator.material = _materialMachineOff;
        ParticleSystem.MainModule main = _pollutionParticles.main;
        main.loop = false;

        LevelProgress.Instance.DeactivatePollutionMachine();
        _polluterSoundInstance.setParameterByNameWithLabel("parameter:/polluterState", "polluterOff");

        _interactPrompt.SetActive(false);
        Destroy(this);
    }

    public void Start()
    {
        LevelProgress.Instance.RegisterPollutionMachine();
        _interactPrompt.SetActive(false);

        _polluterSoundInstance = FMODUnity.RuntimeManager.CreateInstance(_polluterSoundEvent);
        _polluterSoundInstance.start();
        _polluterSoundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.OnInteractReady -= Deactivate;
            _interactPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.OnInteractReady += Deactivate;
            _interactPrompt.SetActive(true);
        }
    }

    private void OnDisable()
    {
        PlayerController.OnInteractReady -= Deactivate;
    }

    public bool GetIsActive() 
    {
        return _isActive;
    }

    public void OnDestroy()
    {
        _polluterSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _polluterSoundInstance.release();
    }
}
