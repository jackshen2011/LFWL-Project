using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using RoomCardNet;

class JieSuanUI : UnModalUIBase
{
    /// <summary>
    /// 海选赛时结算界面的文本标题对象.
    /// </summary>
    public GameObject HaiXuanSaiTxObj;
    /// <summary>
    /// 结算界面返回按键
    /// </summary>
    public GameObject FanHuiBt;
    /// <summary>
    /// 结算界面下一局按键
    /// </summary>
    public GameObject XiaYiJuBt;
    // Use this for initialization
    /// <summary>
    /// 牌局结算信息.
    /// </summary>
    public ImageBase PaiJuJiSuanInfo;
    /// <summary>
    /// PaiJuJiSuanSp[0] -> 上家胜.
    /// PaiJuJiSuanSp[1] -> 对家胜.
    /// PaiJuJiSuanSp[2] -> 本家胜.
    /// PaiJuJiSuanSp[3] -> 下家胜.
    /// PaiJuJiSuanSp[4] -> 流局.
    /// </summary>
    public Sprite[] PaiJuJiSuanSp;
    /// <summary>
    /// 玩家信息的父级.
    /// </summary>
    public Transform PlayerInfoPtr;
    /**
     * PlayerPos[0] -> 上家.
     * PlayerPos[1] -> 对家.
     * PlayerPos[2] -> 本家.
     * PlayerPos[3] -> 下家.
     */
    public Vector2[] PlayerPos;
    SJiSuanGameCtrl pSJiSuanGameCtrl;
    GameObject gJieSuanMainUser;
    GameObject gJieSuan_MainUser_Infoitem;
    JieSuanOtherUser pJieSuanOtherUser;
    public enum ChiHuEnum
    {
        Null = -1,
        ChiHu,
        DianPao,
        PingHu,
        ZiMo,
    }
    Dictionary<int, JieSuanOtherUser> dJieSuanUserDic = new Dictionary<int, JieSuanOtherUser>();
    Dictionary<int, Vector2> dJieSuanUserPosDic = new Dictionary<int, Vector2>();
    float TimeLastRaceRoom;
    void Update()
    {
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                {
                    if (Time.time - TimeLastRaceRoom < 15f)
                    {
                        return;
                    }
                    //淘汰赛房间自动开启下一局游戏.
                    OnNextBtnClick();
                    break;
                }
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    if (Time.time - TimeLastRaceRoom < 10f)
                    {
                        return;
                    }
                    //自建房间自动开启下一局游戏.
                    OnNextBtnClick();
                    break;
                }
            case OneRoomData.RoomType.RoomType_OfficialRoom:
                {
                    if (Time.time - TimeLastRaceRoom < 10f)
                    {
                        return;
                    }
                    //秦人比赛房间自动开启下一局游戏.
                    OnNextBtnClick();
                    break;
                }
        }
    }

    public void SetJieSuanUIActive(bool isActive)
    {
        TimeLastRaceRoom = Time.time;
        gameObject.SetActive(isActive);
    }

    /// <summary>
    /// 初始化普通结算面板.
    /// isLiuJu == true -> 流局.
    /// param[30] -> 当前局数.
    /// </summary>
    public void Initial(object[] param, bool isLiuJu = false)
    {
        //JieSuanMainUser JieSuan_MainUser_Infoitem  JieSuanOtherUser
        GameObject temp = null;
        int hiddenSt = 0;
        TimeLastRaceRoom = Time.time;
        //这里需要服务器告知客户端是否隐藏海选赛的下一局按键.
        //hiddenSt = (int)param[30];
        HiddenJieSuanUIBt(hiddenSt);
        pSJiSuanGameCtrl = GetComponent<SJiSuanGameCtrl>();
        pSJiSuanGameCtrl.InitJieSuanGameInfo(param, isLiuJu);
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_Gold:
            case OneRoomData.RoomType.RoomType_RoomCard:
            case OneRoomData.RoomType.RoomType_TwoHumanRoom:
                {
                    HaiXuanSaiTxObj.SetActive(false);
                    break;
                }
            case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
                {
                    HaiXuanSaiTxObj.SetActive(true);
                    break;
                }
            default:
                {
                    HaiXuanSaiTxObj.SetActive(false);
                    break;
                }
        }

        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
        {
            MainRoot._gRoomData.cCurRoomData.nCurRound = ((ushort)param[30]) + 1;
            //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo();
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips.DestroyThis();
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.DestroyThis();
        }
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(false);
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.DestroySMDengDaiChuPai();


        bool isYingjia = false;
        bool isFangPao = false;
        PlayerBase playerBaseDt = null;
        PlayerBase playerBaseYingJia = null;

        for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.MAX_USER; i++)
        {
            isYingjia = false;
            isFangPao = false;

            temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/JieSuanOtherUser"), gameObject.transform, false);
            if (PlayerInfoPtr != null)
            {
                temp.transform.SetParent(PlayerInfoPtr);
            }
            ((RectTransform)temp.transform).anchoredPosition = PlayerPos[i];
            pJieSuanOtherUser = temp.GetComponent<JieSuanOtherUser>();
            dJieSuanUserDic.Add(i, pJieSuanOtherUser);
            if ((int)(param[2 + i * 6]) != -1)
            {
                playerBaseDt = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)(param[2 + i * 6]));
                pJieSuanOtherUser.Initial(i, playerBaseDt.nSex, playerBaseDt.sUserName, (int)(param[7 + i * 6]), playerBaseDt.nUserId);
                isYingjia = (int)(param[28]) == (int)param[2 + i * 6];
                isFangPao = (int)(param[26]) == (int)param[2 + i * 6];
                if (isLiuJu)
                {
                    pJieSuanOtherUser.SetPlayerBiaoJi(playerBaseDt.nUIUserSit, false);
                    pJieSuanOtherUser.SetPlayerChiHuInfo((int)ChiHuEnum.Null);
                }
                else
                {
                    pJieSuanOtherUser.SetPlayerBiaoJi(playerBaseDt.nUIUserSit, isYingjia);
                    if (isYingjia)
                    {
                        playerBaseYingJia = playerBaseDt;
                        if ((int)(param[28]) == (int)(param[26]))
                        {
                            //自摸.
                            pJieSuanOtherUser.SetPlayerChiHuInfo((int)ChiHuEnum.ZiMo);
                        }
                        if ((int)(param[28]) != (int)(param[26]))
                        {
                            //吃胡.
                            pJieSuanOtherUser.SetPlayerChiHuInfo((int)ChiHuEnum.ChiHu);
                        }
                    }
                    if (isFangPao)
                    {
                        if ((int)(param[28]) != (int)(param[26]))
                        {
                            //点炮.
                            pJieSuanOtherUser.SetPlayerChiHuInfo((int)ChiHuEnum.DianPao);
                        }
                    }
                    if ((int)param[2 + i * 6] != (int)(param[26]) && (int)param[2 + i * 6] != (int)(param[28]))
                    {
                        //自点吃什么都不是.
                        pJieSuanOtherUser.SetPlayerChiHuInfo((int)ChiHuEnum.Null);
                    }
                }
            }
            else
            {
                playerBaseDt = null;
            }
        }

        if (isLiuJu)
        {
            SetPaiJuJieSuanInfo(4);
        }
        else
        {
            if (playerBaseYingJia != null)
            {
                SetPaiJuJieSuanInfo(playerBaseYingJia.nUIUserSit);
            }
        }
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShengYuPaiShuObj.SetActive(false);
    }
    /// <summary>
    /// indexVal == 0 -> 上家胜.
    /// indexVal == 1 -> 对家胜.
    /// indexVal == 2 -> 本家胜.
    /// indexVal == 3 -> 下家胜.
    /// indexVal == 4 -> 流局.
    /// </summary>
    void SetPaiJuJieSuanInfo(int indexVal)
    {
        PaiJuJiSuanInfo.sprite = PaiJuJiSuanSp[indexVal];
    }
    public void OnGoBackBtnClick()
    {
        Debug.Log("Unity:" + "OnGoBackBtnClick...");
        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_MyRoom)
        {
        }
        else
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveStartGameBt(true);
        }
        DestroyThis();
    }
    public void OnNextBtnClick()
    {
        Debug.Log("Unity:" + "OnNextBtnClick...");
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_RoomCard:
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnCardRoomReadyNext();
                break;
            case OneRoomData.RoomType.RoomType_Gold:
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnGoldRoomReadyNext();
                break;
            case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnHaiXuanRoomReadyNext();
                    break;
                }
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnGroupRoomReadyNext();
                    break;
                }
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnMyRoomReadyNext();
                    break;
                }
        }
        DestroyThis();
    }
    /// <summary>
    /// 海选赛/自建比赛结束时隐藏返回和下一局按键.
    /// </summary>
    void HiddenJieSuanUIBt(int hiddenState)
    {
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
                {
                    if (hiddenState == 0)
                    {
                        return;
                    }
                    //FanHuiBt.SetActive(false);
                    XiaYiJuBt.SetActive(false);
                    MainRoot._gUIModule.pUnModalUIControl.SpawnJinRiHaiXuanSaiOver();
                    break;
                }
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                {
                    TimeLastRaceRoom = Time.time;
                    FanHuiBt.SetActive(false);
                    break;
                }
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    TimeLastRaceRoom = Time.time;
                    FanHuiBt.SetActive(false);
                    XiaYiJuBt.SetActive(false);
                    break;
                }
            case OneRoomData.RoomType.RoomType_OfficialRoom:
                {
                    TimeLastRaceRoom = Time.time;
                    FanHuiBt.SetActive(false);
                    XiaYiJuBt.SetActive(false);
                    break;
                }
        }
    }
}