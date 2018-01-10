using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomCardNet;

class RoomCardListCtrl : MonoBehaviour
{
    /**
     * 玩家房卡列表.
     */
    public Transform[] RoomCardTrAy;
    /**
     * 房卡列表第一个元素的坐标.
     */
    public Vector3 StartPos;
    /**
     * 两个房卡元素的间隔距离.
     */
    public float OffsetPosY = 190f;
    /**
     * 玩家房卡号信息.
     */
    public List<int> RoomCardNumList = new List<int>();
    /**
     * 玩家房间的牌局状态信息.
     * PaiJuStList[index] == 0 -> "等待中".
     * PaiJuStList[index] == 1 -> "对局中".
     */
    public List<byte> PaiJuStList = new List<byte>();
    /// <summary>
    /// 房卡数据列表.
    /// </summary>
    public List<OneRoomData> RoomCardDtList = new List<OneRoomData>();
    /// <summary>
    /// 玩家房卡的當前數量.
    /// </summary>
    int RoomCardCount;
    /// <summary>
    /// 房卡数据信息来源于服务器.
    /// 這里將玩家的房卡列表信息存儲到RoomCardDtList.
    /// 此方法必须在CreateRoomCardList函数之前调用.
    /// </summary>
    public void FillRoomCardDtList(object[] args = null)
    {
        RoomCardDtList.Clear();
        RoomCardCount = (int)args[1];
        if (RoomCardCount > RoomCardMax) {
            RoomCardCount = RoomCardMax;
        }

        int diFenVal = 0;
        int maxJuShu = 0;
        string stemp = "";
        string[] datas = new string[13];
        for (int i = 0; i < RoomCardCount; i++) {
            RoomCardDtList.Add(new OneRoomData());
            stemp = ((string)args[6 + i * 6]).Replace("#", ":");
            datas = stemp.Split(":".ToCharArray());
            diFenVal = System.Convert.ToUInt16(datas[13]);
            maxJuShu = ((bool)args[7 + i * 6])?1:(System.Convert.ToInt16(datas[1]) == 0 ? 8 : 16);
            RoomCardDtList[i].SetRoomInfo((int)args[4 + i * 6], (OneRoomData.RoomStat)args[5 + i * 6], 0, maxJuShu, 0, (string)args[3 + i * 6], diFenVal, OneRoomData.RoomType.RoomType_RoomCard, 0);
        }
    }
    /**
     * 初始化房卡列表信息.
     */
    public void InitRoomCardInfo(object[] args = null)
    {
        FillRoomCardDtList(args);
        CreateRoomCardList();
        if (RoomCardCount <= 0) {
            return;
        }

        Vector3 offsetPos = Vector3.zero;
        float roomCardHeight = 0f;
        //该数据从服务器获取.
        byte paiJuval = 0;
        RoomCardDt rmCardDt = null;
        int roomCardLength = RoomCardTrAy.Length > 4 ? RoomCardTrAy.Length : 4;
        for (int i = 0; i < roomCardLength; i++) {
            roomCardHeight += OffsetPosY;
        }
        Vector2 roomCardListPtrSize = ((RectTransform)RoomCardListPtr).sizeDelta;
        roomCardListPtrSize.y = roomCardHeight;
        ((RectTransform)RoomCardListPtr).sizeDelta = roomCardListPtrSize;
        RoomCardListPtr.localPosition = new Vector3(0f, -(roomCardHeight * 0.5f), 0f);

        StartPos = new Vector3(0f, (roomCardHeight * 0.5f) - (((RectTransform)RoomCardTrAy[0]).sizeDelta.y * 0.5f), 0f);
        RoomCardNumList.Clear();
        PaiJuStList.Clear();
        for (int i = 0; i < RoomCardTrAy.Length; i++) {
            offsetPos.y = -i * OffsetPosY;
            RoomCardTrAy[i].localPosition = StartPos + offsetPos;

            if (i < RoomCardDtList.Count) {
                RoomCardNumList.Add(RoomCardDtList[i].nRoomId);

                paiJuval = (byte)(RoomCardDtList[i].eRoomState);
                PaiJuStList.Add(paiJuval);

                rmCardDt = RoomCardTrAy[i].GetComponent<RoomCardDt>();
                rmCardDt.SetPlayerRoomCardData(RoomCardDtList[i].nRoomId, RoomCardDtList[i].eRoomState, RoomCardDtList[i].nCurRound, RoomCardDtList[i].nMaxRound, RoomCardDtList[i].nRoomOwnerId, RoomCardDtList[i].sRoomOwnerName, RoomCardDtList[i].nDiFen, RoomCardDtList[i].eRoomType, RoomCardDtList[i].nGoldRoomLevel);
                rmCardDt.ShowRoomCardDtInfo();
            }
            else {
                RoomCardTrAy[i].gameObject.SetActive(false);
            }
        }
    }
    /**
     * 房卡列表父级.
     */
    public Transform RoomCardListPtr;
    /// <summary>
    /// 房卡列表最大数量.
    /// </summary>
    int RoomCardMax = 20;
    /**
     * 创建房卡名片.
     */
    void CreateRoomCardList()
    {
        if (RoomCardTrAy != null) {
            //清理玩家房卡数据.
            GameObject obj = new GameObject();
            obj.transform.parent = RoomCardListPtr;
            for (int i = 0; i < RoomCardTrAy.Length; i++) {
                RoomCardTrAy[i].parent = obj.transform;
            }
            Destroy(obj);
        }

        RoomCardDt cardDt = null;
        RoomCardTrAy = new Transform[RoomCardCount];
        for (int i = 0; i < RoomCardCount; i++) {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/RoomCardUI/PlayerRoomCard"),
                                                                RoomCardListPtr, false);
            RoomCardTrAy[i] = obj.transform;
            obj.name = "PlayerRoomCard" + i;
            cardDt = obj.GetComponent<RoomCardDt>();
            cardDt.SetIndexRoomCard((byte)i);
        }
    }
    /**
     * 查找房间卡号是否存在.
     */
    public int FindRoomCardNum(int cardNum)
    {
        int rv = -1;
        for (int i = 0; i < RoomCardNumList.Count; i++) {
            if (cardNum == RoomCardNumList[i]) {
                rv = i;
                break;
            }
        }
        return rv;
    }
    /**
     * 检测房间牌局状态,判定玩家是否可以进入牌局.
     */
    public void CheckPlayerRoomCardState(int roomId)
    {
        if (MainRoot._gRoomData != null && MainRoot._gRoomData.cCurRoomData == null)
        {
            MainRoot._gRoomData.cCurRoomData = new OneRoomData();
        }
        OnPlayerSelectFrindRoom(roomId);
        return;
    }
    /**
     * 使玩家进入房卡游戏.
     * roomId -> 房间ID.
     */
    void OnPlayerSelectFrindRoom(int roomId)
    {
        //向服务端请求加入特定房间，在返回消息中切换场景 
        Debug.Log("Unity:"+"OnPlayerSelectFrindRoom -> roomId " + roomId);
        if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null
            && MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pMyRoomRuKouDlg != null)
        {
            //加入自建比赛房.
            MainRoot._gRoomData.cCurRoomData.eRoomType = OneRoomData.RoomType.RoomType_MyRoom;
        }
        else
        {
            //加入好友房卡房.
            MainRoot._gRoomData.cCurRoomData.eRoomType = OneRoomData.RoomType.RoomType_RoomCard;
        }
		if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
		{
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerJoinCardRoom(roomId);
        }
    }
    /**
     * btSt -> 确定玩家点击的是那个房间按键.
     * RoomXX -> 房间序列号 -> [0, RoomCardMax).
     */
    public void OnClickBt(byte roomIndex)
    {
        if (roomIndex < 0 || roomIndex >= RoomCardMax)
        {
            Debug.LogWarning("Unity:"+"CheckPlayerRoomCardState -> roomIndex is wrong! roomIndex " + roomIndex);
            return;
        }
        CheckPlayerRoomCardState(RoomCardNumList[roomIndex]);
    }
}