using UnityEngine;
using System.Collections;
using System;

class ZongJueSaiRuKouDlg : MonoBehaviour
{
    /// <summary>
    /// 玩家信息列表父级.
    /// </summary>
    public Transform PlayerInfoListTr;
    int mDaoJiShiVal = 0; //倒计时
    float TimeLastVal = 0f;
    /// <summary>
    /// 服务器当前时间
    /// </summary>
    DateTime mServerTime;
    /// <summary>
    /// 提示信息数组.
    /// TipsObjArray[x]: 0 时间未到, 1 比赛准备中, 2 比赛已开始.
    /// </summary>
    public GameObject[] TipsObjArray;
    /// <summary>
    /// 时间信息Tip1
    /// </summary>
    public TextBase[] TxTimeTip1;
    /// <summary>
    /// 时间信息Tip2
    /// </summary>
    public TextBase TxTimeTip2;
    /// <summary>
    /// 活动详情对象.
    /// </summary>
    //public GameObject XingQingObj;
    /// <summary>
    /// 总决赛对话框组件.
    /// </summary>
    EnsureDlg ZongJueSaiDlg;
    bool IsFillDt = false;
    /// <summary>
    /// 比赛状态(0 比赛时间未到, 1 海选赛开始, 2 小组赛准备, 3 小组赛开始)
    /// </summary>
    OneRoomData.ThemeRaceState mBiSaiState = OneRoomData.ThemeRaceState.ShiJianWeiDao;
    OneRoomData.DataDefine_MultiRaceType eMultiRaceType;
    /// <summary>
    /// 初始化淘汰赛入口界面信息.
    /// </summary>
    public void InitZongJueSaiDlg(int biSaiState, int biSaiChangCi, OneRoomData.DataDefine_MultiRaceType multiRaceType)
    {
        eMultiRaceType = multiRaceType;
        OnClickXiangQingBt();
        mBiSaiState = (OneRoomData.ThemeRaceState)biSaiState;
        ZongJueSaiDlg = GetComponent<EnsureDlg>();
        ZongJueSaiDlg.btn2.SetActive(false);
        //XingQingObj.SetActive(false);
        MainRoot._gRoomData.cCacheRoomData.nCurChangCi = biSaiChangCi;
        MainRoot._gRoomData.cCacheRoomData.nDiFen = 1;
        //ShowZongJueSaiPlayerInfo(); //test.
    }

    /// <summary>
    /// 显示服务端返回的总决赛界面玩家数据信息.
    /// args 总决赛玩家信息,需要服务端按照玩家排名信息进行发送.
    /// </summary>
    public void ShowZongJueSaiPlayerInfo(object[] args = null)
    {
        if (IsFillDt)
        {
            return;
        }
        IsFillDt = true;

        //playerDt[0] id 5
        //playerDt[1] 昵称 6
        //playerDt[2] 微信头像url 7
        //playerDt[3] 性别 8
        //playerDt[4] 总决赛名次.
        object[] playerDt = new object[5];
        bool isShowStartBt = false;
        int userId = 0;
        GameObject obj = null;
        ZongJueSaiPlayerInfo playerInfo = null;
        bool isZongJueSaiOver = false;
        int startIndex = 3;
        string shouJiHaoVal = MainRoot._gPlayerData.TelInfo; //服务端获取手机号.
        bool isXuanShou = false; //是否是比赛选手.
        ShowTipInfo(args);
        isZongJueSaiOver = (bool)args[startIndex];
        for (int i = 0; i < PlayerInfoListTr.childCount; i++)
        {
            obj = (GameObject)Instantiate(Resources.Load("Prefab/ZongJueSaiRuKou/ZongJueSaiPlayerInfo"),
                                PlayerInfoListTr.GetChild(i), false);

            playerDt[0] = args[startIndex + 1 + (i * 4)];
            playerDt[1] = args[startIndex + 2 + (i * 4)];
            playerDt[2] = args[startIndex + 3 + (i * 4)];
            playerDt[3] = args[startIndex + 4 + (i * 4)];
            playerDt[4] = isZongJueSaiOver == true ? i : -1;
            playerInfo = obj.GetComponent<ZongJueSaiPlayerInfo>();
            playerInfo.ShowPlayerInfo(playerDt);

            userId = (int)playerDt[0];
            if (userId == MainRoot._gPlayerData.nUserId && MainRoot._gPlayerData.nUserId != 0 && !isZongJueSaiOver)
            {
                isShowStartBt = true;
            }

            if (userId == MainRoot._gPlayerData.nUserId && MainRoot._gPlayerData.nUserId != 0)
            {
                isXuanShou = true;
            }
        }

        if (isShowStartBt != ZongJueSaiDlg.btn2.activeInHierarchy)
        {
            ZongJueSaiDlg.btn2.SetActive(isShowStartBt);
        }
        
        if (shouJiHaoVal == "" && isXuanShou)
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShouJiHaoYanZhengDlg();
        }
    }

    /// <summary>
    /// 点击活动详情按键.
    /// </summary>
    public void OnClickXiangQingBt()
    {
		//XingQingObj.SetActive(true);
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-ThemeRaceXiangQing"), transform.parent.transform, false);
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

        //这里添加向服务端发送点击总决赛界面开始比赛按键消息.
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallTaoTaiSaiClickStartBiSaiBt(1, eMultiRaceType);
        }
    }

    /// <summary>
    /// 收到服务端开始比赛按键的返回消息.
    /// arg: 0 比赛尚未开始, 1 进入比赛, 2 比赛已开始,系统视为自动退赛.
    /// </summary>
    public void OnReceivedStartBtMsg(int arg)
    {
        //arg: 0 比赛尚未开始, 1 进入比赛.
        OneRoomData.ThemeRace_GroupState state = (OneRoomData.ThemeRace_GroupState)arg;
        switch (state)
        {
            case OneRoomData.ThemeRace_GroupState.Game_NotStart: //比赛尚未开始.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiWeiKaiShi();
                    break;
                }
            case OneRoomData.ThemeRace_GroupState.Game_Ready: //进入比赛.
                {
                    MakePlayerIntoZongJueSai();
                    break;
                }
            case OneRoomData.ThemeRace_GroupState.Game_Runing: //比赛已开始,系统视为自动退赛.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiYiKaiShi_TuiSai();
                    break;
                }
        }
    }

    /// <summary>
    /// 使玩家进入总决赛游戏场景.
    /// </summary>
    public void MakePlayerIntoZongJueSai()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShaanxiMahjong")
        {
            MainRoot._gRoomData.cCacheRoomData.IsZongJueSaiRoom = true;
            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_Group;
            MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = eMultiRaceType;
            MainRoot._gRoomData.cCacheRoomData.nMaxRound = 8;
            MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong"); //直接创建总决赛房间，进入游戏场景.
        }
    }

    /// <summary>
    /// 显示提示信息.
    /// </summary>
    void ShowTipInfo(object[] args)
    {
        DateTime raceTime = (DateTime)args[20];
        MainRoot._gRoomData.cCacheRoomData.TimeBiSaiStart = raceTime;
        mServerTime = (DateTime)args[21];
        foreach (var item in TipsObjArray)
        {
            item.SetActive(false);
        }
        
        switch (mBiSaiState)
        {
            case OneRoomData.ThemeRaceState.GroupStart:
                {
                    //TipsObjArray[2].SetActive(true);
                    break;
                }
            case OneRoomData.ThemeRaceState.GroupZhunBei:
                {
                    mDaoJiShiVal = (((raceTime.Hour - mServerTime.Hour) * 60 + (raceTime.Minute - mServerTime.Minute)) * 60) + (60 - mServerTime.Second);
                    TxTimeTip2.text = mDaoJiShiVal.ToString();
                    TipsObjArray[1].SetActive(true);
                    break;
                }
            case OneRoomData.ThemeRaceState.ShiJianWeiDao:
                {
                    TipsObjArray[0].SetActive(true);
                    TxTimeTip1[0].text = raceTime.Month.ToString();
                    TxTimeTip1[1].text = raceTime.Day.ToString();

                    mDaoJiShiVal = -1;
                    if (raceTime.Day == mServerTime.Day)
                    {
                        mDaoJiShiVal = (((raceTime.Hour - mServerTime.Hour) * 60 + (raceTime.Minute - mServerTime.Minute)) * 60) + (60 - mServerTime.Second);
                    }
                    break;
                }
        }
        TimeLastVal = Time.time;
    }

    void Update()
    {
        UpdateTipState();
        UpdateTipDaoJiShi();
    }

    /// <summary>
    /// 更新提示信息的显示状态.
    /// </summary>
    void UpdateTipState()
    {
        if (!TipsObjArray[0].activeInHierarchy)
        {
            return;
        }

        if (mDaoJiShiVal <= -1)
        {
            return;
        }

        if (Time.time - TimeLastVal < 1f)
        {
            return;
        }
        TimeLastVal = Time.time;
        mDaoJiShiVal--;

        if (mDaoJiShiVal <= 600)
        {
            //比赛进入准备阶段.
            TipsObjArray[0].SetActive(false);
            TipsObjArray[1].SetActive(true);
        }
    }

    /// <summary>
    /// 更新提示信息的倒计时.
    /// </summary>
    void UpdateTipDaoJiShi()
    {
        if (!TipsObjArray[1].activeInHierarchy)
        {
            return;
        }

        if (Time.time - TimeLastVal < 1f)
        {
            return;
        }
        TimeLastVal = Time.time;

        mDaoJiShiVal--;
        TxTimeTip2.text = mDaoJiShiVal.ToString();
        if (mDaoJiShiVal <= 0)
        {
            //比赛已开始.
            TipsObjArray[1].SetActive(false);
            //TipsObjArray[2].SetActive(true);
        }
    }
}