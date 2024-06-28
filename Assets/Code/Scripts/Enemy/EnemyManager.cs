using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private FMODUnity.EventReference FMOD_onAlertEvent;
    [SerializeField] private FMODUnity.EventReference FMOD_onCautiousEvent;

    // Singleton
    private static EnemyManager _instance = null;
    public static EnemyManager Instance
    {
        get 
        {
            if (_instance == null)
            {
                // something like this but different, Unity doesnt like MonoBehaviour scripts being intitialized with new
                // sol: create gameobject and add EnemyManager component?
                
                //_instance = new EnemyManager();
            }
            
            return _instance;
        }
    }

    private List<Enemy> _registeredEnemies = new List<Enemy>();

    private int _alertedCounter, _cautiousCounter;

    private Dictionary<ActionState, int> _stateDict = new Dictionary<ActionState, int>();

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

    private void Start()
    {
        _stateDict.Add(ActionState.Alerted, 0);
        _stateDict.Add(ActionState.Cautious, 0);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        _registeredEnemies.Add(enemy);
    }

    public void UnsubscribeAll()
    {
        foreach (var enemy in _registeredEnemies)
        {
            enemy.UnsubscribeDistractor();
        }
    }

    public void EnemyChangedState(ActionState fromState, ActionState toState, GameObject source)
    {
        if (_stateDict.ContainsKey(toState)) _stateDict[toState]++;

        if (_stateDict.ContainsKey(fromState)) _stateDict[fromState]--;

        // fire one shots if toState is alerted or cautious
        if (toState == ActionState.Alerted)
        {
            LevelSoundManager.Instance.PlayOneShotEvent(FMOD_onAlertEvent, source);
        }
        else if (toState == ActionState.Cautious)
        {
            if (fromState != ActionState.Alerted)
            {
                // play cautious state unless we calmed down from the alerted state
                LevelSoundManager.Instance.PlayOneShotEvent(FMOD_onCautiousEvent, source);
            }
        }

        if (_stateDict[ActionState.Alerted] > 0)
        {
            // change level music to alerted
            LevelSoundManager.Instance.ChangeMusicTo("enemyAlert");
        }
        else if (_stateDict[ActionState.Cautious] > 0)
        {
            // change level music to cautious
            LevelSoundManager.Instance.ChangeMusicTo("enemyCautious");
        }
        else
        {
            LevelSoundManager.Instance.ChangeMusicTo("enemyIdle");
        }
    }
}
