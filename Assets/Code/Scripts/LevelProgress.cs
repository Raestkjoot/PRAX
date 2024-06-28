using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class LevelProgress : Singleton<LevelProgress>
{
    [SerializeField, Range(0.0f, 1.0f)] private float _requiredScrapPercentage = 0.8f;
    [SerializeField, Range(0.0f, 1.0f)] private float _pollutionFadeOutSpeed = 0.35f;

    [SerializeField] private float _gameOverFogFadeInDuration = 2f;
    [SerializeField] private float _gameOverFadeInTargetMaskSizeValue = -1;

    [SerializeField] private Image _polluterProgressCircle;
    [SerializeField] private Image _scrapProgressBar;
    [SerializeField] private RectTransform _scrapRequiredIndicator;
    [SerializeField] private Image _winStatusBackground;
    [SerializeField] private TMP_Text _winStatusText;

    [SerializeField] private Volume _postProcessingVolume;
    [SerializeField] private ParticleSystem _pollutionParticles1;
    [SerializeField] private ParticleSystem _pollutionParticles2;

    private int _scrapAmount = 0;
    private int _scrapCollected = 0;
    private int _totalPollutionMachines = 0;
    private int _deactivatedPollutionMachines = 0;
    private bool _smogWinCondition = false;
    private bool _scrapWinCondition = false;

    private float _fogStartValue = 25.0f;
    private float _fogFadeoutTargetValue = 60.0f;

    private Color _winColor = new Color(0.32f, 0.9f, 0.3f, 0.8f);
    private Color _fogColor = new Color(0.176f, 0.156f, 0.18f, 1.0f);

    // TODO: Maybe have a prefab with all level things and
    // add the particles and the progress bar in start function

    private void Start()
    {
        SetScrapRequiredIndicatorPosition();

        // Make sure fog is set up properly.
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = _fogColor;
        RenderSettings.fogEndDistance = _fogStartValue;
    }

    public void RegisterScrap()
    {
        _scrapAmount++;
    }

    public void RegisterPollutionMachine()
    {
        _totalPollutionMachines++;
    }

    public void CollectedScrap()
    {
        _scrapCollected++;

        // Update progress bar and win condition
        float percentageCollected = (float)_scrapCollected / _scrapAmount;

        _scrapProgressBar.fillAmount = percentageCollected;
        _scrapWinCondition = percentageCollected >= _requiredScrapPercentage;

        if (_scrapWinCondition)
        {
            _scrapProgressBar.color = _winColor;
            CheckWinConditions();
        }
    }

    public void DeactivatePollutionMachine()
    {
        _deactivatedPollutionMachines++;

        // Update progress bar and win condition
        float percentageDeactivated = (float)_deactivatedPollutionMachines /
            _totalPollutionMachines;
        _polluterProgressCircle.fillAmount = percentageDeactivated;

        _smogWinCondition = _deactivatedPollutionMachines == _totalPollutionMachines;

        if (_smogWinCondition)
        {
            _polluterProgressCircle.color = _winColor;
            SmogFadeOut();
            CheckWinConditions();
        }
    }

    public bool CheckWinConditions()
    {
        bool win = _smogWinCondition && _scrapWinCondition;

        if (win)
        {
            _winStatusText.text = "CLEAR";
            _winStatusBackground.color = new Color(0.32f, 0.9f, 0.3f, 0.2f);

            if (_scrapCollected == _scrapAmount)
            {
                _winStatusText.text = "FULL CLEAR";
            }
        }

        return win;
    }

    /// <summary>
    /// Enemy has touched the player: we call on the GameOverUI and make the fog close in.
    /// </summary>
    public void ShowGameOver()
    {
        GameOver.Instance.OpenGameOverUI();
        SetGameOverFog();
    }

    /// <summary> Check if the player has collected at least one scrap. </summary>
    /// <remark> Used in tutorial level </remark>
    public bool HasCollectedScrap()
    {
        return _scrapCollected == 0;
    }

    private async void SmogFadeOut()
    {
        _postProcessingVolume.profile.TryGet<EdgeParticles_VolumeComponent>
            (out var edgeParticles);
        edgeParticles.intensity.overrideState = true;

        float fogFadeSpeed = _pollutionFadeOutSpeed *
            (_fogFadeoutTargetValue - RenderSettings.fogEndDistance);
        float edgeParticlesFadeSpeed = _pollutionFadeOutSpeed * edgeParticles.intensity.value;

        // We can only change start values of particles, so we turn off looping and
        // wait for them to fade out on their own. If we want them to fade out faster,
        // we can change the start lifetime property on the particle system itself.
        ParticleSystem.MainModule main;
        main = _pollutionParticles1.main; main.loop = false;
        main = _pollutionParticles2.main; main.loop = false;

        while (RenderSettings.fogEndDistance < _fogFadeoutTargetValue)
        {
            // this should prevent the particles from continue to fade when GameOver
            if (GameOver.Instance.GetIsGameOver()) { return; }
            // Linear fog does not have density, so we change the end distance instead
            RenderSettings.fogEndDistance += fogFadeSpeed * Time.deltaTime;

            edgeParticles.intensity.value -= edgeParticlesFadeSpeed * Time.deltaTime;

            await Task.Yield();
        }

        //RenderSettings.fogEndDistance = _fogFadeoutTargetValue;
        //edgeParticles.active = false;
    }

    private void SetScrapRequiredIndicatorPosition()
    {
        // Find the corners of the progress bar and use a left corner and
        // a right corner to find min and max
        var trans = _scrapProgressBar.gameObject.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        trans.GetWorldCorners(corners);
        float min = corners[0].x;
        float max = corners[2].x;

        // Set the indicator position based on required scrap percentage
        Vector3 pos = _scrapRequiredIndicator.position;
        pos.x = Mathf.Lerp(min, max, _requiredScrapPercentage);
        _scrapRequiredIndicator.position = pos;
    }

    private async void SetGameOverFog()
    {
        // Same as when disabling the SmogOutFog but we decrease the maskSize to make the fog close in towards Player/center instead.
        // Get the particle volume:
        _postProcessingVolume.profile.TryGet<EdgeParticles_VolumeComponent>
    (out var edgeParticles);
        // if the polluters have been turned off - reenable the particles
        edgeParticles.active = true;
        edgeParticles.intensity.value = 1f;

        edgeParticles.maskSize.overrideState = true;
        float fogFadeInSpeed = (edgeParticles.maskSize.value - _gameOverFadeInTargetMaskSizeValue) / _gameOverFogFadeInDuration;

        while (_gameOverFogFadeInDuration > _gameOverFadeInTargetMaskSizeValue)
        {
            _gameOverFogFadeInDuration -= fogFadeInSpeed * Time.deltaTime;
            if (edgeParticles.maskSize.value >= _gameOverFadeInTargetMaskSizeValue)
            {
                edgeParticles.maskSize.value -= fogFadeInSpeed * Time.deltaTime;
            }
            await Task.Yield();
        }
    }
}
