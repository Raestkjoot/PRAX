using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This has been made so we can change the text on the button on Select/ Deselect
/// </summary>
public class ButtonColorText : Button, ISelectHandler, IDeselectHandler
{
    private TextMeshProUGUI _textObject;
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        _textObject = GetComponentInChildren<TextMeshProUGUI>();
        _textObject.color = Color.white;

        // play sound of switching button in menu
        FMODUnity.RuntimeManager.PlayOneShot("event:/menuMove");
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _textObject.color = Color.black;
    }

    private void PlayConfirmSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/menuConfirm");
    }

    protected override void Start()
    {
        base.Start();
        base.onClick.AddListener(this.PlayConfirmSound);
        _textObject = GetComponentInChildren<TextMeshProUGUI>();
    }
}
