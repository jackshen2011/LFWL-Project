using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class UGuiScrollRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    RectTransform RectTr;
    ScrollRect ScrollRt;
    void Start()
    {
        ScrollRt = GetComponent<ScrollRect>();
        RectTr = GetComponent<RectTransform>();
    }
    #region IEndDragHandler implementation
    public void OnEndDrag (PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
    #endregion

    #region IBeginDragHandler implementation
    public void OnBeginDrag (PointerEventData eventData)
    {
        //SetDraggedPosition(eventData);
    }
    #endregion

    /// <summary>
    /// set position of the dragged game object
    /// </summary>
    /// <param name="eventData"></param>
    private void SetDraggedPosition(PointerEventData eventData)
    {
        // transform the screen point to world point int rectangle
        Vector3 globalMousePos = Vector3.zero;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(RectTr, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
            globalMousePos.z = RectTr.position.z;
            if (ScrollRt.horizontal) {
                globalMousePos.y = RectTr.position.y;
            }
            if (ScrollRt.vertical) {
                globalMousePos.x = RectTr.position.x;
            }
            RectTr.position = globalMousePos;
        }
    }
}