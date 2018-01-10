using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 玩家的手牌类
/// </summary>
class HandMaJiang : MaJiang, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //public const float hangUpYPosition = PlayerHandMaJiang.firstHandYpos+0.02f;//升起的ypos
    //public const float yPosition = PlayerHandMaJiang.firstHandYpos;//正常的ypos
    public const float handWidth = 0.074f;//手牌宽
    public const float choiceClearance=0.051f;//接的牌间隔

    public Camera handCamera;//手牌摄像机

    bool selected;//是否挂起 准备打出
    bool dragFlag;//是否拖拽

    PlayerHandMaJiang parent;
    public void SetParent(PlayerHandMaJiang p)
    {
        if (parent == null)
        {
            parent = p;
        }
    }
    /// <summary>
    /// 设置是否挂起这张牌
    /// </summary>
    public bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            Vector3 pos = gameObject.transform.position; ;
            selected = value;
            if (selected)
            {
                
                gameObject.transform.position = new Vector3(pos.x, parent.firstHandYpos+0.02f, pos.z);
            }
            else
            {
                gameObject.transform.position = new Vector3(pos.x, parent.firstHandYpos, pos.z);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        /*if (Input.touchCount > 0)
        {

            // Get movement of the finger since last frame
            //获取手指自最后一帧的移动
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

            // Move object across XY plane
            //移动物体在XY平面
            transform.Translate(new Vector3(-touchDeltaPosition.x * 0.1f,
            -touchDeltaPosition.y * 0.1f, 0),Space.World);
        }*/

    }
    private void SetDraggedPosition(PointerEventData eventData)
    {
        if (MainRoot._pMJGameTable.playerHandMaJiang.dragMaJiang)
        {
            dragFlag = true;
            Vector3 pos;
            pos = handCamera.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y));
            pos = new Vector3(pos.x, pos.y, -0.1f);
            MainRoot._pMJGameTable.playerHandMaJiang.dragMaJiang.transform.position = pos;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Unity:"+"OnPointerDown");
        if (true)
        {
            if (!Selected)
            {
                //Selected = true;
            }
            else {
            }
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!dragFlag)
        {
            if (Selected)
            {
                MainRoot._pMJGameTable.UserPutOutMJ(ArrayIndex < 0 ? true : false, ArrayIndex);
            }
            else
            {
                MainRoot._pMJGameTable.playerHandMaJiang.OnHandMaJiangPointUp(this);
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong01, true);
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragFlag = true;
        CreateDragShadow();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragFlag = false;
        if (MainRoot._pMJGameTable.playerHandMaJiang.dragMaJiang)
        {
            Destroy(MainRoot._pMJGameTable.playerHandMaJiang.dragMaJiang.gameObject);
        }
        
        if (eventData.position.y>180)
        {
            MainRoot._pMJGameTable.UserPutOutMJ(ArrayIndex < 0 ? true : false, ArrayIndex);
            MainRoot._pMJGameTable.playerHandMaJiang.AllHandMaJiangDown();
        }
    }
    void CreateDragShadow()
    {
        if (MainRoot._pMJGameTable.playerHandMaJiang.dragMaJiang)
        {
            return;
        }
        GameObject obj;
        obj = Instantiate(gameObject);
        HandMaJiang shadow = obj.GetComponent<HandMaJiang>();
        MainRoot._pMJGameTable.playerHandMaJiang.dragMaJiang = shadow;
    }
}
