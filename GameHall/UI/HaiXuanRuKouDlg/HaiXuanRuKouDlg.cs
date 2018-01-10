using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 海选入口对话框面板.
/// </summary>
class HaiXuanRuKouDlg : MonoBehaviour
{
    /// <summary>
    /// 晋级图片.
    /// </summary>
    public GameObject JinJiObj;
    /// <summary>
    /// 海选入场券对话框.
    /// </summary>
    [HideInInspector]
    public HaiXuanRuChangQuanDlg pHaiXuanRuChangQuanDlg;
    /// <summary>
    /// 暂无比赛对象.
    /// </summary>
    public GameObject ZanWuBiSaiObj;
    /// <summary>
    /// 比赛排行数据.
    /// </summary>
    public GameObject BiSaiPaiHangObj;
    /// <summary>
    /// 玩家战绩信息对象.
    /// </summary>
    public GameObject PlayerZhanJiObj;
    /// <summary>
    /// 活动详情对象.
    /// </summary>
    //public GameObject XingQingObj;
    /// <summary>
    /// 比赛场次img.
    /// </summary>
    public ImageBase BiSaiChangCiImg;
    /// <summary>
    /// 比赛场次Sprite.
    /// </summary>
    public Sprite[] BiSaiChangCiSpAy;
    /// <summary>
    /// 报名/开始比赛/比赛未开始按键图集.
    /// </summary>
    //public Sprite[] BaoMingBiSaiBtSp;
    /// <summary>
    /// 报名/开始比赛按键图片.
    /// </summary>
    //ImageBase BaoMingBiSaiBtImg;
    /// <summary>
    /// 海选排行数据.
    /// 参数格式：
    /// 0-3位无用，然后每3位为一个玩家的数据，id，name，积分，分别是前9名的数据，
    /// 然后是4位自己的数据，id，name，积分，名次。
    /// </summary>
    public object[] HaiXuanPaiHangDt;
    EnsureDlg DlgHaiXuan;
    OneRoomData.DataDefine_MultiRaceType eMultiRaceType;
    /// <summary>
    /// 是否打开社区赛代码验证面板.
    /// </summary>
    [HideInInspector]
    public bool IsOpenSheQuSaiDaiMaYanZheng = true;
    /// <summary>
    /// 海选赛开始时间.
    /// </summary>
    DateTime mStartTime;
    /// <summary>
    /// 海选赛结束时间.
    /// </summary>
    DateTime mEndTime;
    /// <summary>
    /// 初始化海选入口排行界面.
    /// </summary>
    public void Init(int biSaiChangCi, OneRoomData.DataDefine_MultiRaceType multiRaceType)
    {
        eMultiRaceType = multiRaceType;
        OnClickXiangQingBt();
        DlgHaiXuan = GetComponent<EnsureDlg>();
        //BaoMingBiSaiBtImg = dlg.btn2.GetComponent<ImageBase>();
        //dlg.btn2.SetActive(false);
        if (biSaiChangCi < 0)
        {
            biSaiChangCi = 0;
        }
        BiSaiChangCiImg.sprite = BiSaiChangCiSpAy[biSaiChangCi];
        MainRoot._gRoomData.cCacheRoomData.nCurChangCi = biSaiChangCi;
        MainRoot._gRoomData.cCacheRoomData.nDiFen = 1;
        ZanWuBiSaiObj.SetActive(true);
        BiSaiPaiHangObj.SetActive(false);
        PlayerZhanJiObj.SetActive(false);
        //XingQingObj.SetActive(false);
        //ChangeBaoMingBiSaiImg(); //test.
        //ShowHaiXuanRuKouPaiHangInfo(); //test.
        //ShwoPlayerSelfZhanJiDt(); //test.
    }

    /// <summary>
    /// 显示海选入口界面排行信息.
    /// args[x]: 0-2 无效数据, 3-33 排行數據, 34 是否晉級, 35 是否打开社区赛代码面板(只在社区赛使用).
    /// 36 开赛时间, 37 结束时间.
    /// </summary>
    public void ShowHaiXuanRuKouPaiHangInfo(object[] args = null)
    {
        HaiXuanPaiHangDt = args;
        bool isHavePaiHangDt = false;
        bool isPlayerJinJi = (int)args[34] == 0 ? false : true;
        int listDtCount = 0;
        mStartTime = (DateTime)args[36];
        mEndTime = (DateTime)args[37];
        //Debug.Log("Unity: timeStart " + mStartTime.ToString("hh:mm") + ", timeEnd " + mEndTime.ToString("hh:mm"));
        //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnHaiXuanShiJianWeiDao(mStartTime, mEndTime); //test
        switch (eMultiRaceType)
        {
            case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                {
                    IsOpenSheQuSaiDaiMaYanZheng = (int)args[35] == 0 ? true : false;
                    if (IsOpenSheQuSaiDaiMaYanZheng)
                    {
                        //打开社区赛代码输入面板.
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSheQuSaiYanZhengMaDlg();
                    }
                    break;
                }
        }

        if (JinJiObj != null)
        {
            JinJiObj.SetActive(isPlayerJinJi);
        }
        DlgHaiXuan.btn2.SetActive(!isPlayerJinJi);

        for (int i = 0; i < 8; i++)
        {
            if ((int)HaiXuanPaiHangDt[(i * 3) + 3] != 0)
            {
                isHavePaiHangDt = true;
                listDtCount++;
            }
            else
            {
                break;
            }
        }

        if (!isHavePaiHangDt)
        {
            return;
        }
        VerticalListUICtrl verListUI = GetComponent<VerticalListUICtrl>();
        VerticalListUICtrl.ObjListConfigDt configDt = new VerticalListUICtrl.ObjListConfigDt();
        configDt.CountObj = listDtCount;
        configDt.ObjName = "HaiXuanPaiHangUI";
        configDt.ObjPrefab = "Prefab/HaiXuanRuKou/" + configDt.ObjName;
        verListUI.CreateObjList(configDt);

        ZanWuBiSaiObj.SetActive(false);
        BiSaiPaiHangObj.SetActive(true);
        ShowPlayerSelfZhanJiDt();
    }

    /// <summary>
    /// 显示玩家自己的战绩信息.
    /// </summary>
    public void ShowPlayerSelfZhanJiDt()
    {
        bool isHavePaiHangDt = false; //玩家是否进入8强.
        for (int i = 0; i < 8; i++)
        {
            if ((int)HaiXuanPaiHangDt[(i * 3) + 3] == MainRoot._gPlayerData.nUserId && MainRoot._gPlayerData.nUserId != 0)
            {
                isHavePaiHangDt = true;
                break;
            }
        }

        if (isHavePaiHangDt)
        {
            Debug.Log("Unity: ShwoPlayerSelfZhanJiDt -> isHavePaiHangDt == " + isHavePaiHangDt);
            return;
        }

        if ((int)HaiXuanPaiHangDt[30] != MainRoot._gPlayerData.nUserId)
        {
            Debug.Log("Unity: ShwoPlayerSelfZhanJiDt -> paiHangBang playerId is wrong!");
            return;
        }

        if ((int)HaiXuanPaiHangDt[33] == 0)
        {
            //没有玩家名次,需要报名.
            //ChangeBaoMingBiSaiImg(0);
            Debug.Log("Unity: ShwoPlayerSelfZhanJiDt -> paiHangBang player mingCiVal is zero!");
            return;
        }

        object[] args = new object[4];
        args[0] = HaiXuanPaiHangDt[33];
        args[1] = HaiXuanPaiHangDt[31];
        args[2] = HaiXuanPaiHangDt[32];
        args[3] = 1;
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/HaiXuanPaiHangUI"),
                            PlayerZhanJiObj.transform, false);
        HaiXuanPaiHangUI haiXuanUICom = obj.GetComponent<HaiXuanPaiHangUI>();
        haiXuanUICom.ShowListUiDt(args);
        haiXuanUICom.ChangeZhanJiTextType();

        PlayerZhanJiObj.SetActive(true);
        //ChangeBaoMingBiSaiImg(1);
    }

    /// <summary>
    /// 点击活动详情按键.
    /// </summary>
    public void OnClickXiangQingBt()
    {
		//XingQingObj.SetActive(true);
		GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-ThemeRaceXiangQing"), transform.parent.transform, false);
		EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.ThemeRaceXiangQing);
		dlg.p_ShowTextA.text = MainRoot._gPlayerData.sThemeRace_HuoDongXiangQing;
	}

    /// <summary>
    /// 点击详情关闭按键.
    /// </summary>
    public void OnClickXiangQingCloseBt()
    {
        //XingQingObj.SetActive(false);
    }

    /// <summary>
    /// 点击开始比赛按键.
    /// </summary>
    public void OnClickStartBiSaiBt()
    {
        Debug.Log("Unity: OnClickStartBiSaiBt");
        if (MainRoot._gPlayerData.relinkUserRoomData != null)
        {//说明玩家有未完成的牌局
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnLoginPaiJuWeiWanChengDlg();//点击请求重新连入牌局
            return;
        }

        switch (eMultiRaceType)
        {
            case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                {
                    if (IsOpenSheQuSaiDaiMaYanZheng)
                    {
                        //打开社区赛代码输入面板.
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSheQuSaiYanZhengMaDlg();
                        return;
                    }
                    break;
                }
        }

        //这里添加向服务端发送点击海选界面开始比赛按键消息.
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallHaiXuanSaiClickStartBiSaiBt(eMultiRaceType);
        }
        //OnReceivedStartBtMsg(1); //test.
    }

    /// <summary>
    /// 收到服务端开始比赛按键的返回消息.
    /// arg: 0 比赛时间未到, 1 海选入场券界面, 2 用户已在其他轮次获得淘汰赛资格, 3 允许玩家进入海选比赛游戏.
    /// </summary>
    public void OnReceivedStartBtMsg(int arg)
    {
        //arg: 0 比赛时间未到, 1 海选入场券界面, 2 用户已在其他轮次获得淘汰赛资格, 3 允许玩家进入海选比赛游戏.
        GameObject obj = null;
        EnsureDlg dlg = null;
        switch (arg)
        {
            case 0: //比赛时间未到.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnHaiXuanShiJianWeiDao(mStartTime, mEndTime);
                    break;
                }
            case 1: //海选入场券界面.
                {
                    obj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-HaiXuanRuChangQuan"),
                                        transform.parent, false);
                    dlg = obj.GetComponent<EnsureDlg>();
                    dlg.Initial(EnsureDlg.EnsureKind.HaiXuanRuChangQuan);
                    pHaiXuanRuChangQuanDlg = obj.GetComponent<HaiXuanRuChangQuanDlg>();
                    break;
                }
            case 2: //用户已在其他轮次获得淘汰赛资格.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnYiHuoDeTaoTaiSaiZG();
                    break;
                }
            case 3: //允许玩家进入海选比赛游戏.
                {
                    //if (!MainRoot._gPlayerData.IsMianFeiBiSaiOver)
                    //{
                    //    MainRoot._gPlayerData.IsMianFeiBiSaiOver = true;
                    //    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnYiShiYongMFCanSai();
                    //}
                    MakePlayerIntoHaiXuanSai();
                    break;
                }
            default:
                {
                    Debug.Log("Unity: OnReceivedStartBtMsg -> arg was wrong! arg == " + arg);
                    break;
                }
        }
    }

    /// <summary>
    /// 使玩家进入海选赛游戏场景.
    /// </summary>
    public void MakePlayerIntoHaiXuanSai()
    {
        //这里添加玩家进入海选比赛游戏场景的网络消息.
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShaanxiMahjong")
        {
            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan;
            MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = eMultiRaceType;
            MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong"); //直接创建海选赛房间，进入游戏场景.
        }
    }
}