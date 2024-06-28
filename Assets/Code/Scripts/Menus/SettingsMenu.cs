using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    private FMOD.Studio.Bus _masterBus;
    private FMOD.Studio.Bus _sfxBus;
    private FMOD.Studio.Bus _musicBus;
    [SerializeField] private GameObject _backButton;

    public void SetMasterVolume(float volume)
    {
        _masterBus.setVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxBus.setVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        _musicBus.setVolume(volume);
    }

    public void SetQuality(int qualityLevelIndex)
    {
        QualitySettings.SetQualityLevel(qualityLevelIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void OnEnable()
    {
        // Using OnEnable instead of awake so the gameobject
        // can be disabled in the start-up scene. This adds a
        // little unecessary computation when opening the settings
        // menu, but it should be insignificant.
        _masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        _sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
        _musicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");

        EventSystem.current.SetSelectedGameObject(_backButton);
    }
}