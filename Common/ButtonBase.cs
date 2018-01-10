using UnityEngine;
using UnityEngine.EventSystems;
class ButtonBase : UnityEngine.UI.Button
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if (eventData.button == PointerEventData.InputButton.Left || eventData.pointerId > 0)
        {
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.XiTong01, true);
        }
    }
}
