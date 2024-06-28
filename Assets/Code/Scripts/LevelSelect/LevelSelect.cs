using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : PersistentSingleton<LevelSelect>
{
    private int _maxUnlockedLevel = 0;
    private int _levelAmount = 6;

    private int _selectedLevel;

    private GameObject _player;
    private Vector3 _playerSpawnPos;

    private float _distanceBetweenLevels = 0.5f;

    // set to false when player replays levels so that _maxUnlockedLevel does not get increased
    private bool _canUnlock;

    public bool SetSelectedLevel(int level) 
    {
        if (CheckSelectedLevel(level))
        {
            _selectedLevel = level;
            return true;
        }
        return false;
    }

    public bool CheckSelectedLevel(int level)
    {
        return (_maxUnlockedLevel >= level);
    }

    public void EnterSelectedLevel() 
    {
        if (_maxUnlockedLevel >= _selectedLevel)
        {
            if (_maxUnlockedLevel == _selectedLevel)
            {
                _canUnlock = true;
            }
            else
            {
                _canUnlock = false;
            }

            // go to level
            _playerSpawnPos = _player.transform.position;
            SceneManager.LoadScene("Level" + _selectedLevel);
        }
    }

    public void CompleteLevel() 
    {
        if (_canUnlock) _maxUnlockedLevel++;
    }

    public int GetMaxUnlockedLevel() 
    {
        return _maxUnlockedLevel;
    }

    public void StartUp(Transform grassLand, Transform cityLand)
    {
        _player = Instantiate(Resources.Load<GameObject>("Prefabs/Player"),
            _playerSpawnPos, Quaternion.identity);

        ScaleGrounds(grassLand, cityLand);
    }

    private void ScaleGrounds(Transform grassLand, Transform cityLand)
    {
        float scaleAmount = _maxUnlockedLevel;

        if (_maxUnlockedLevel == _levelAmount)
        {
            _distanceBetweenLevels *= 2f;
        }

        scaleAmount *= _distanceBetweenLevels;

        Vector3 scale = grassLand.localScale;
        scale.z += scaleAmount;
        grassLand.localScale = scale;

        scale = cityLand.localScale;
        scale.z -= scaleAmount;
        cityLand.localScale = scale;
    }
}
