using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSoundManager : MonoBehaviour
{
    // Singleton
    private static LevelSoundManager _instance = null;
    public static LevelSoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
            }
            return _instance;
        }
    }

    private FMOD.Studio.EventInstance _levelMusicInstance;
    [SerializeField] private FMODUnity.EventReference _levelMusicEvent;
    private FMOD.Studio.PARAMETER_ID _stateParameterID;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _levelMusicInstance = FMODUnity.RuntimeManager.CreateInstance(_levelMusicEvent);
        _levelMusicInstance.start();
    }

    public void ChangeMusicTo(string parameterValue)
    {
        _levelMusicInstance.setParameterByNameWithLabel("parameter:/enemyState", parameterValue);
    }

    public void PlayOneShotEvent(FMODUnity.EventReference FMOD_event, GameObject source)
    {
        FMOD.Studio.EventInstance tempInstance = FMODUnity.RuntimeManager.CreateInstance(FMOD_event);
        tempInstance.start();
        tempInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(source));
    }

    void OnDestroy()
    {
        _levelMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopAll()
    {
    }
}
