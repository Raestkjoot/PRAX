using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoundButton : Button, ISelectHandler, IDeselectHandler
{
    // OnSelect is called right at the beginning of the scene
    // because the button is somewhere (stribe [<- that name is so cool] prob knows) set as selected
    // (which it also should be)
    // that's why I made this last minute hotfix
    private float _startTime;
    private float _muteTimer = 0.5f;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        if (Time.time - _startTime < _muteTimer)
        {
            return;
        }

        // play sound of switching button in menu
        FMODUnity.RuntimeManager.PlayOneShot("event:/menuMove");
    }

    private void PlayConfirmSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/menuConfirm");
    }

    protected override void Start()
    {
        base.Start();
        base.onClick.AddListener(this.PlayConfirmSound);
    }

    protected override void Awake()
    {
        _startTime = Time.time;
    }
}
