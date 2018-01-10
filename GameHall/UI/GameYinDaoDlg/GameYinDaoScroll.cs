using UnityEngine;
using UnityEngine.EventSystems;

class GameYinDaoScroll : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public float SpeedMove = 1.5f;
    public int PageCount = 5;
    ScrollRectBase ScrollRectCom;
    public float[] PageArray;
    float HorPos = 0f;
    float StartScrollPos;
    float PosRecordVal = 0f;
    int IndexRecordVal = 0;
    bool IsBeginDrag = false;
    void Start()
    {
        PageArray = new float[PageCount];
        for (int i = 0; i < PageCount; i++)
        {
            PageArray[i] = (1f / (PageCount - 1)) * i;
        }
        ScrollRectCom = GetComponent<ScrollRectBase>();
    }

    void Update()
    {
        if (!IsBeginDrag)
        {
            ScrollRectCom.horizontalNormalizedPosition = Mathf.Lerp(ScrollRectCom.horizontalNormalizedPosition,
                HorPos, Time.deltaTime * SpeedMove);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = ScrollRectCom.horizontalNormalizedPosition;
        //Debug.Log("Unity: StartScrollPos " + StartScrollPos + ", EndScrollPos " + posX);
        if (StartScrollPos - posX < 0f)
        {
            if (IndexRecordVal < PageArray.Length - 1)
            {
                IndexRecordVal++;
            }
        }

        if (StartScrollPos - posX > 0f)
        {
            if (IndexRecordVal > 0)
            {
                IndexRecordVal--;
            }
        }
        PosRecordVal = posX;
        HorPos = PageArray[IndexRecordVal];
        IsBeginDrag = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        StartScrollPos = ScrollRectCom.horizontalNormalizedPosition;
        IsBeginDrag = true;
    }
}