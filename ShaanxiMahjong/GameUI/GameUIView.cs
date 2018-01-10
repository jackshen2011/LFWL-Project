using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

using RoomCardNet;

/*
 *	
 *  
 *
 *	by GWB
 *
 */



/// <summary>
/// 打牌房间的主界面
/// </summary>
class GameUIView : UnModalUIBase
{
	[HideInInspector]
	/// <summary>
	/// 下炮选择界面 
	/// </summary>	
	public XiaPaoSelectUI pXiaPaoUI;
	/// <summary>
	/// 下注界面
	/// </summary>
	public EnsureDlg pXiaZhuUI;
	/// <summary>
	/// 测试按钮容器
	/// </summary>
	public Transform testPanel;
	/// <summary>
	/// 标准弹出提示对象
	/// </summary>
	public SystemMsgText pMsgText;
	/// <summary>
	/// 界面顶部tips文字
	/// </summary>
	public TipsMsgText pTipsMsgText;
	/// <summary>
	/// 牌桌界面每个用户面前弹出的特效的位置
	/// </summary>
	public Vector2[] EffPosInfo = new Vector2[4];
	/// <summary>
	/// 取消托管界面
	/// </summary>
	public QuXiaoTuoGuanUI pQuXiaoTuoGuan;
	/// <summary>
	/// 选项界面
	/// </summary>
	public OptionUI pOptionUI;
	/// <summary>
	/// 普通结算界面
	/// </summary>
	public JieSuanUI pJieSuanUI;
	/// <summary>
	/// 房卡房游戏局数信息控制父对象.
	/// </summary>
	public GameObject FangKaJuShuObj;
    /// <summary>
    /// 游戏局数信息的按键.
    /// </summary>
    public ButtonBase FangKaRoomJuShuBtBase;
    /// <summary>
    /// 房卡游戏局数信息文本.
    /// </summary>
    public TextBase FangKaJuShuText;
	/// <summary>
	/// 剩余牌数的数字显示
	/// </summary>
	public TextBase pRemainCount;
	/// <summary>
	/// 吃碰杠胡提示界面
	/// </summary>
	public ChiPengHuTips pChiPengHuTips;
	/// <summary>
	/// 胡牌按钮
	/// </summary>
	public GameObject gHuPaiBtn;
	/// <summary>
	/// 停牌提示界面
	/// </summary>
	public TingPaiTishiUI tptishi;
	/// <summary>
	/// 解散游戏中好友房的UI控制对象.
	/// </summary>
	public JieSanFriendRoomCtrl pJieSanFriendRoom;
	/// <summary>
	/// 游戏开始时玩家匹配动画控制对象.
	/// </summary>
	public MatchPlayerCtrl pMatchPlayerCtrl;
	/// <summary>
	/// 准备按钮
	/// </summary>
	public GameObject gReadyBtn;
	/// <summary>
	/// 邀请好友加入按钮
	/// </summary>
	public GameObject gInvitaBtn;
	/// <summary>
	/// 开始游戏按键
	/// </summary>
	public GameObject StartGameBtObj;
	/// <summary>
	/// 聊天按键.
	/// </summary>
	public GameObject ChatBtObj;
    /// <summary>
    /// 退出按键对象.
    /// </summary>
    public GameObject ExitBtObj;
    /// <summary>
    /// 帮助按键对象.
    /// </summary>
    public GameObject HelpBtObj;
    /// <summary>
    /// 排名信息对象.
    /// </summary>
    public GameObject PaiMingInfoObj;
    /// <summary>
    /// 排名信息文本.
    /// </summary>
    public TextBase PaiMingInfoTx;
    public GameObject ChatFace;//聊天界面
	/// <summary>
	/// 解散按键.
	/// </summary>
	public GameObject JieSanBtObj;
	/// <summary>
	/// 托管按钮.
	/// </summary>
	public GameObject TuoGuanBtObj;
	/// <summary>
	/// 是否有显示高级结算界面.
	/// </summary>
	bool IsShowGaoJiJieSuanPanel;
	/// <summary>
	/// 剩余牌数UI.
	/// </summary>
	public GameObject ShengYuPaiShuObj;
	/// <summary>
	/// 选杠界面
	/// </summary>
	public SelectGangUI pSelect;
    /// <summary>
    /// 自建房等待界面.
    /// </summary>
    [HideInInspector]
    public MyRoomWaitPanel pMyRoomWaitPanel;
    public GameUIView()
	{
	}
	/// <summary>
	/// 初始化
	/// </summary>
	public void Initial()
	{
		nSysMsgArrayLenth = 255;
		SysMsgArray = new SystemMsgText[nSysMsgArrayLenth];
		if (MainRoot._gUIModule.nCurSceneType == 1)
		{
			InitialEffPos();
			MainRoot._gUIModule.pUnModalUIControl.pGameUIView = this;
			MainRoot._gMainRoot.UIModuleInitEnd(MainRoot._gUIModule.nCurSceneType);

			//testPanel.gameObject.SetActive(true);
		}
	}
	/// <summary>
	/// 初始化麻将房间基础UI界面
	/// </summary>
	public void InitialBaseRoomUI()
	{
		try
		{
			GameObject temp;
			temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/TipsMsgText"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
			pTipsMsgText = temp.GetComponent<TipsMsgText>();
		}
		catch (Exception)
		{

		}
	}

	/// <summary>
	/// 初始化金币房间UI界面
	/// </summary>
	public void InitialGoldRoomUI(int coin=0)
	{
		try
		{
			if (MainRoot._gGameRoomCenter != null)
			{
				MainRoot._gGameRoomCenter.InitialGameRoom();
			}
			MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
				MainRoot._gPlayerData.nUserId,
				MainRoot._gPlayerData.sUserName,
				MainRoot._gPlayerData.sHeadImgUrl,
				0,
				coin,
				MainRoot._gPlayerData.nSex,
				MainRoot._gRoomData.cCurRoomData.nDiFen
				);
			PlayerBase tempplay;
			tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
			tempplay.InitialLinkPanel();    //初始化人物面板
            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_MyRoom)
            {
            }
            else
            {
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//删除匹配中.
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(1, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配中.
            }
			MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.WAIT_JOIN;
		}
		catch (Exception ex)
		{
			Debug.LogError("Unity: " + ex.ToString());
		}
	}
	/// <summary>
	/// 初始化房卡房间UI界面，加载主UI，显示自己
	/// </summary>
	public void InitialCardRoomUI()
	{

		ShowReadyBtn(false);
		if (MainRoot._gGameRoomCenter != null)
		{
			MainRoot._gGameRoomCenter.InitialGameRoom();
		}
		PlayerBase[] tempa = new PlayerBase[4];
		bool nIsUserMAX = true;
		int n = 2;
		int ncou8ntuser = 0;
		for (int i = 0; i < 4; i++)
		{
			if ((int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5] != -1) //不是空座位
			{
				tempa[i] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5]);
				if (tempa[i] == null)
				{
					if (MainRoot._gPlayerData.nUserId == (int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5])
					{
						MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
						(int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5],
						(string)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 1],
						(string)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 2],
						i,  //sit
						MainRoot._gPlayerData.selfCardJiFenData,  //gold
						(int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 3],   //sex
						MainRoot._gRoomData.cCurRoomData.vRoomSetting[13]   //difen
						);
					}
					else
					{
						MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
						(int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5],
						(string)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 1],
						(string)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 2],
						i,  //sit
						MainRoot._gPlayerData.selfCardJiFenData,  //gold
						(int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 3],   //sex
						MainRoot._gRoomData.cCurRoomData.vRoomSetting[13]   //difen
						);
					}

                    MainRoot._gGameRoomCenter.gGameRoom.SetTablePlayerIp((int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5], (string)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[25+i]);

                }
			}
			else
			{
				ncou8ntuser++;
				nIsUserMAX = false;
			}
			if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom() && ncou8ntuser ==2)	//如果二人麻将，则两人挤满
			{
				nIsUserMAX = true;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			tempa[i] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5]);
			if (tempa[i] != null)
			{
				tempa[i].InitialLinkPanel();    //初始化人物面板
				tempa[i].SetZhuangjia((MainRoot._gGameRoomCenter.gGameRoom.GetZhuangJiaUserId == tempa[i].nUserId));
				tempa[i].SetReadyState(((int)MainRoot._gRoomData.cCurRoomData.tempUserJoinInfo[n + i * 5 + 4]) == 2);
			}
		}
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowReadyBtn(nIsUserMAX);//如果人已经满了，则显示准备按钮，关闭邀请好友按钮
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMPersonJiaRu();
	}


    /// <summary>
    /// 初始化海选赛房间UI界面
    /// </summary>
    public void InitialHaiXuanRoomUI(int coin = 0)
    {
        try
        {
            if (MainRoot._gGameRoomCenter != null)
            {
                MainRoot._gGameRoomCenter.InitialGameRoom();
            }
            MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
                MainRoot._gPlayerData.nUserId,
                MainRoot._gPlayerData.sUserName,
                MainRoot._gPlayerData.sHeadImgUrl,
                0,
                coin,
                MainRoot._gPlayerData.nSex,
                MainRoot._gRoomData.cCurRoomData.nDiFen
                );
            PlayerBase tempplay;
            tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
            tempplay.InitialLinkPanel();    //初始化人物面板
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//删除匹配中.
            MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.WAIT_JOIN;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unity: " + ex.ToString());
        }
    }

    /// <summary>
    /// 初始化淘汰赛/总决赛房间UI界面
    /// </summary>
    public void InitialThemeRaceRoomGroupUI(int coin = 0)
    {
        try
        {
            if (MainRoot._gGameRoomCenter != null)
            {
                MainRoot._gGameRoomCenter.InitialGameRoom();
            }
            MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
                MainRoot._gPlayerData.nUserId,
                MainRoot._gPlayerData.sUserName,
                MainRoot._gPlayerData.sHeadImgUrl,
                0,
                coin,
                MainRoot._gPlayerData.nSex,
                MainRoot._gRoomData.cCurRoomData.nDiFen
                );
            PlayerBase tempplay;
            tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
            tempplay.InitialLinkPanel();    //初始化人物面板
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//删除匹配中.
            MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.WAIT_JOIN;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unity: " + ex.ToString());
        }
    }
    /// <summary>
    /// 初始化自建赛房间UI界面
    /// </summary>
    public void InitialRaceMyRoomUI(int coin = 0)
    {
        try
        {
            if (MainRoot._gGameRoomCenter != null)
            {
                MainRoot._gGameRoomCenter.InitialGameRoom();
            }
            MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
                MainRoot._gPlayerData.nUserId,
                MainRoot._gPlayerData.sUserName,
                MainRoot._gPlayerData.sHeadImgUrl,
                0,
                coin,
                MainRoot._gPlayerData.nSex,
                MainRoot._gRoomData.cCurRoomData.nDiFen
                );
            PlayerBase tempplay;
            tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
            tempplay.InitialLinkPanel();    //初始化人物面板
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//删除匹配中.
            MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.WAIT_JOIN;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unity: " + ex.ToString());
        }
    }
    /// <summary>
    /// 初始化测试UI界面
    /// </summary>
    /// <param name="ntype"></param>
    public void InitialTestUI()
	{
		try
		{
			GameObject test;
			testbtn tb;


			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();

			tb.InitialTestBtn("提示1", 1);

			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("提示2", 2);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("提示3", 3);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("提示4", 4);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("提示5", 5);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("提示7", 7);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("提示9", 9);


			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("吃碰胡1", 10);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("吃碰胡2", 11);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("吃碰胡3", 12);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("吃碰胡4", 13);
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("吃碰胡5", 14);


			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("事件测试", 15);

			//打牌测试
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家接牌", 100);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家打牌", 101);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家碰牌", 102);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家直杠牌", 103);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家续杠", 112);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家暗杠", 113);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("下家胡", 118);

			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家接牌", 104);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家打牌", 105);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家碰牌", 106);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家直杠牌", 107);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家续杠", 114);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家暗杠", 115);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("对家胡", 119);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家接牌", 108);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家打牌", 109);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家碰牌", 110);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家直杠牌", 111);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家续杠", 116);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家暗杠", 117);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("上家胡", 120);


			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("创建房间", 21);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("匹配成功", 22);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("都下炮", 23);

			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("掷骰子", 24);
			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("自己接牌", 25);

			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("流局", 26);

			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("截图分享", 27);

			test = (GameObject)Instantiate(Resources.Load("Prefab/testbtn"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.testPanel.gameObject.transform, false);
			tb = test.GetComponent<testbtn>();
			tb.InitialTestBtn("截图分享圈", 28);

		}
		catch (System.Exception)
		{
			Debug.LogError("Unity:" + "System.Exception");
		}
	}
	/// <summary>
	/// 初始化牌桌玩家头像位置
	/// </summary>
	private void InitialEffPos()    //1上家,2对家，3下家
	{
		EffPosInfo[0] = new Vector2(0.0f, -162.0f);//主家
		EffPosInfo[1] = new Vector2(-386.0f, 82.0f);//上家
		EffPosInfo[2] = new Vector2(0.0f, 282.0f);//对家
		EffPosInfo[3] = new Vector2(387.0f, 82.0f);//下家
	}
	/// <summary>
	/// 显示吃碰胡杠自摸特效
	/// </summary>
	/// <param name="nkind"></param>
	/// <param name="nuserid"></param>
	public void InitialEff(int nkind, int nuserid)
	{
		GameObject ptemp = null;
		int npos = 0;
		ParticleControl pcontrol;
		switch (nkind)
		{
			case 0:
				ptemp = (GameObject)Instantiate(Resources.Load("Prefab/EffectUI-Chi"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform, false);
				break;
			case 1:
				ptemp = (GameObject)Instantiate(Resources.Load("Prefab/EffectUI-Gang"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform, false);
				break;
			case 2:
				ptemp = (GameObject)Instantiate(Resources.Load("Prefab/EffectUI-Hu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform, false);
				break;
			case 3:
				ptemp = (GameObject)Instantiate(Resources.Load("Prefab/EffectUI-Peng"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform, false);
				break;
			case 4:
				ptemp = (GameObject)Instantiate(Resources.Load("Prefab/EffectUI-ZiMo"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform, false);
				break;
			default:
				break;
		}
		pcontrol = ptemp.GetComponent<ParticleControl>();

		npos = MainRoot._gGameRoomCenter.gGameRoom.GetUserUISitByUserID(nuserid);
		((RectTransform)pcontrol.transform).anchoredPosition = EffPosInfo[npos];
		if (pcontrol)
		{
			pcontrol.Initial();
		}
	}
	/// <summary>
	/// 初始化下炮选择界面
	/// </summary>
	public void InitialXiaPaoUI()
	{
		if (pXiaPaoUI != null)
		{
			pXiaPaoUI.DestroyThis();
			return;
		}
		GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/XiaPaoSelectUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
		pXiaPaoUI = temp.GetComponent<XiaPaoSelectUI>();
		pXiaPaoUI.Initial();
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(3, pXiaPaoUI);
	}
	/// <summary>
	/// 下注界面
	/// </summary>
	public void InitialXiaZhuUI()
	{
		if (pXiaZhuUI != null)
		{
			pXiaZhuUI.DestroyThis();
			return;
		}
		GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-XiaZhu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
		pXiaZhuUI = temp.GetComponent<EnsureDlg>();
		pXiaZhuUI.Initial(EnsureDlg.EnsureKind.XiaZhuDlg);
	}
	/// <summary>
	/// 初始化结算界面
	/// </summary>
	/// <param name="param"></param>
	public void InitialJieSuanUI(object[] param, bool isLiuJu = false)
	{
		if (MainRoot._gRoomData.cCurRoomData != null)
		{
            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard
                || MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group
                || MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_MyRoom)
            {
                if (MainRoot._gRoomData.cCurRoomData.nCurRound + 1 >= MainRoot._gRoomData.cCurRoomData.nMaxRound)
                {
                    return;
                }
            }
		}
		if (isLiuJu)
		{
			DestroySMLiuJu();
		}
		IsMatchPlayerSuccess = false;
		GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/JieSuanUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
		pJieSuanUI = temp.GetComponent<JieSuanUI>();
		SetJieSuanUIActive(false);
		pJieSuanUI.Initial(param, isLiuJu);
	}
	/// <summary>
	/// 显示结算界面
	/// </summary>
	/// <param name="isActive"></param>
	public void SetJieSuanUIActive(bool isActive)
	{
		if (pJieSuanUI)
		{
			pJieSuanUI.SetJieSuanUIActive(isActive);
		}
	}
	/// <summary>
	/// 设置剩余牌数
	/// </summary>
	/// <param name="n"></param>
	public void SetpRemainCount(int n)
	{
		if (n < 0 || n > 108)
		{
			pRemainCount.text = "Err";
			return;
		}
		pRemainCount.text = n.ToString();
	}
	public void Start()
	{
        //MainRoot._gRoomData.cCurRoomData.eRoomType = OneRoomData.RoomType.MyRoom; //test
        bool isZiJianFangZhu = false; //自建房房主,该参数是动态获取.
        SetActiveStartGameBt(false);
        PaiMingInfoObj.SetActive(false);
        LookPlayerListBtObj.SetActive(false);
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType) {
			case OneRoomData.RoomType.RoomType_Gold:
				gReadyBtn.SetActive(false);
                SetActiveInvitaBt(false);
				FangKaJuShuObj.SetActive(false);
				JieSanBtObj.SetActive(false);
				break;
			case OneRoomData.RoomType.RoomType_RoomCard:
				ShowFangKaRoomJuShuInfo(0);
				FangKaJuShuObj.SetActive(true);
				break;
            case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                FangKaRoomJuShuBtBase.enabled = false;
                gReadyBtn.SetActive(false);
                SetActiveInvitaBt(false);
                JieSanBtObj.SetActive(false);
                GameObject msgObj = ShowOneSysMsgText(75, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//比赛准备中.
                SystemMsgText msgText = msgObj.GetComponent<SystemMsgText>();
                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan)
                {
                    Transform msgParent = msgText.gtimertext.gameObject.transform.parent;
                    msgParent.gameObject.SetActive(false);
                }
                else
                {
                    msgText.gtimertext.text = MainRoot._gRoomData.cCurRoomData.TimeBiSaiStart.ToString("HH:mm");
                }

                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                {
                    SetActiveExitBt(false);
                }
                else
                {
                    FangKaJuShuObj.SetActive(false);
                }
                break;
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    FangKaRoomJuShuBtBase.enabled = false;
                    gReadyBtn.SetActive(false);
                    SetActiveInvitaBt(isZiJianFangZhu);
                    HelpBtObj.SetActive(false);
                    LookPlayerListBtObj.SetActive(true);
                    SpawnMyRoomWaitPanel();
                    break;
                }
            case OneRoomData.RoomType.RoomType_OfficialRoom:
                {
                    FangKaRoomJuShuBtBase.enabled = false;
                    HelpBtObj.SetActive(false);
                    JieSanBtObj.SetActive(false);
                    break;
                }
        }

        ChatBtObj.SetActive(false);
		ShengYuPaiShuObj.SetActive(false);
		TuoGuanBtObj.SetActive(false);
		MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong05);
    }

    /// <summary>
    /// 产生自建房等待界面.
    /// </summary>
    void SpawnMyRoomWaitPanel()
    {
        if (pMyRoomWaitPanel != null)
        {
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/MyRoomWaitPanel"), transform, false);
        pMyRoomWaitPanel = obj.GetComponent<MyRoomWaitPanel>();
        pMyRoomWaitPanel.Init();
    }
    /// <summary>
    /// 设置房卡房间游戏的局数信息.
    /// </summary>
    public void ShowFangKaRoomJuShuInfo(int num)
	{
		MainRoot._gRoomData.cCurRoomData.nCurRound = num;
		FangKaJuShuText.text = (num + 1) + "/" + MainRoot._gRoomData.cCurRoomData.nMaxRound + "局";
	}
	public void ClickJieSanFriendRoom()
	{
		if (IsShowGaoJiJieSuanPanel)
		{
			return;
		}

		GameObject test = null;
		EnsureDlg temp = null;
		if (MainRoot._gRoomData.cCurRoomData != null)
		{
			switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
			{
				case OneRoomData.RoomType.RoomType_RoomCard:
					if (MainRoot._gPlayerData != null && MainRoot._gRoomData != null)
					{
						Debug.Log("MainRoot._gRoomData.cCurRoomData.eRoomState === " + MainRoot._gRoomData.cCurRoomData.eRoomState);
						switch (MainRoot._gRoomData.cCurRoomData.eRoomState)
						{
							case OneRoomData.RoomStat.PLAYINGWAIT://房卡房非第一局等待玩家准备状态
							case OneRoomData.RoomStat.PLAYING:
								if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
								{
									RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerAskDissolutionCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId);
								}
								break;
							default:
								if (MainRoot._gPlayerData.nUserId == MainRoot._gRoomData.cCurRoomData.nRoomOwnerId)
								{
									Debug.Log("RoomOwner jie san card room!");
									test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameRoomDlg/ED-JieSanPaiJu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
									temp = test.GetComponent<EnsureDlg>();
									temp.Initial(EnsureDlg.EnsureKind.JieSanPaiJu);
								}
								else
								{
									MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMHostOnly();
								}
								break;
						}
					}
					break;
			}
		}
	}
	/// <summary>
	/// 当房主解散了好友房,在牌局未开始前(服务器返回消息时调用).
	/// </summary>
	public void OnFriendRoomJieSan()
	{
		if (MainRoot._gRoomData.cCurRoomData.eRoomType != OneRoomData.RoomType.RoomType_RoomCard)
		{
			return;
		}
		MainRoot._gRoomData.cCurRoomData = new OneRoomData();
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMHostJieSan();
		MainRoot._gMainRoot.ChangeScene("GameHall");
	}
	/// <summary>
	/// 点击游戏局数按键.
	/// </summary>
	public void CkickGameJuShuBt()
	{
		Debug.Log("CkickGameJuShuBt...");
        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group
            || MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_OfficialRoom)
        {
            return;
        }
        SpawnGameWanFaDlg();
	}
    /// <summary>
    /// 开始前退出游戏房间面板.
    /// </summary>
    EnsureDlg pKaiShiQianTuiChuDlg;
    /// <summary>
    /// 界面关闭按钮
    /// </summary>
    public void ClickCloseButton()
	{
		try
		{
			GameObject test = null;
			EnsureDlg temp = null;
			if (MainRoot._gRoomData.cCurRoomData != null) {
				switch (MainRoot._gRoomData.cCurRoomData.eRoomType) {
					case OneRoomData.RoomType.RoomType_Gold:
						if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.PLAYING)
						{
							test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-YouXiZhong-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
							temp = test.GetComponent<EnsureDlg>();
							temp.Initial(EnsureDlg.EnsureKind.JinRuTuoGuan);
						}
						else if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.COLSE)
						{
							MainRoot._gMainRoot.ChangeScene("GameHall");//金币房间结算后退出牌桌，直接退
						}
						else
						{
							test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-KaiShiQian-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false); //开始前点击退出.
							temp = test.GetComponent<EnsureDlg>();
							temp.Initial(EnsureDlg.EnsureKind.QuXiaoPiPei);
                            pKaiShiQianTuiChuDlg = temp;
                        }
						break;
					case OneRoomData.RoomType.RoomType_RoomCard:
						if (IsShowGaoJiJieSuanPanel)
						{
							MainRoot._gMainRoot.ChangeScene("GameHall");
						}
						else
						{
							if (MainRoot._gPlayerData != null && MainRoot._gRoomData != null)
							{
								Debug.Log("MainRoot._gRoomData.cCurRoomData.eRoomState === " + MainRoot._gRoomData.cCurRoomData.eRoomState);
								switch (MainRoot._gRoomData.cCurRoomData.eRoomState)
								{
									case OneRoomData.RoomStat.PLAYINGWAIT://房卡房非第一局等待玩家准备状态
									case OneRoomData.RoomStat.PLAYING:
										if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
										{
											RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerAskDissolutionCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId);
										}
										break;
									case OneRoomData.RoomStat.COLSE:
										MainRoot._gMainRoot.ChangeScene("GameHall");
										break;
									default:
										SpawnFriendRoomQueDingTuiChuDlg();
										break;
								}
							}
						}
						break;
                    case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
                        if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.PLAYING)
                        {
                            test = (GameObject)Instantiate(Resources.Load("Prefab/ED-YouXiZhong-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
                            temp = test.GetComponent<EnsureDlg>();
                            temp.Initial(EnsureDlg.EnsureKind.JinRuTuoGuan);
                        }
                        else if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.COLSE)
                        {
                            MainRoot._gMainRoot.ChangeScene("GameHall");//海选房间结算后退出牌桌，直接退
                        }
                        else
                        {
                            test = (GameObject)Instantiate(Resources.Load("Prefab/ED-KaiShiQian-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false); //开始前点击退出.
                            temp = test.GetComponent<EnsureDlg>();
                            temp.Initial(EnsureDlg.EnsureKind.QuXiaoPiPei);
                            pKaiShiQianTuiChuDlg = temp;
                        }
                        break;
                    case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                        {
                            if (IsShowGaoJiJieSuanPanel)
                            {
                                MainRoot._gMainRoot.ChangeScene("GameHall");
                            }
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_MyRoom:
                        {
                            if (IsShowGaoJiJieSuanPanel)
                            {
                                MainRoot._gMainRoot.ChangeScene("GameHall");
                            }
                            else
                            {
                                test = (GameObject)Instantiate(Resources.Load("Prefab/ED-BiSaiQian-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false); //比赛开始前点击退出.
                                temp = test.GetComponent<EnsureDlg>();
                                temp.Initial(EnsureDlg.EnsureKind.BiSaiQianEixt);
                                pKaiShiQianTuiChuDlg = temp;
                            }
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_OfficialRoom:
                        {
                            if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.PLAYING)
                            {
                                test = (GameObject)Instantiate(Resources.Load("Prefab/ED-YouXiZhong-TuiChu-QinRenBiSai"), MainRoot._gUIModule.pMainCanvas.transform, false); //游戏中点击退出.
                                temp = test.GetComponent<EnsureDlg>();
                                temp.Initial(EnsureDlg.EnsureKind.YouXiZhong_TuiChu_QinRenBiSai);
                            }
                            else if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.COLSE)
                            {
                                MainRoot._gMainRoot.ChangeScene("GameHall");//秦人比赛房间结算后退出牌桌，直接退
                            }
                            else
                            {
                                test = (GameObject)Instantiate(Resources.Load("Prefab/ED-KaiShiQian-TuiChu-QinRenBiSai"), MainRoot._gUIModule.pMainCanvas.transform, false); //开始前点击退出.
                                temp = test.GetComponent<EnsureDlg>();
                                temp.Initial(EnsureDlg.EnsureKind.KaiShiQian_TuiChu_QinRenBiSai);
                                pKaiShiQianTuiChuDlg = temp;
                            }
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_NULL:
                        test = (GameObject)Instantiate(Resources.Load("Prefab/ED-KaiShiQian-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false); //开始前点击退出.
                        temp = test.GetComponent<EnsureDlg>();
                        temp.Initial(EnsureDlg.EnsureKind.QuXiaoPiPei);
                        pKaiShiQianTuiChuDlg = temp;
                        break;
				}
			}
			else {
                test = (GameObject)Instantiate(Resources.Load("Prefab/ED-KaiShiQian-TuiChu"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false); //开始前点击退出.
                temp = test.GetComponent<EnsureDlg>();
                temp.Initial(EnsureDlg.EnsureKind.QuXiaoPiPei);
                pKaiShiQianTuiChuDlg = temp;
            }
		}
		catch (Exception)
		{

		}

	}
	/// <summary>
	/// 界面托管按钮
	/// </summary>
	public void OnTuoGuanBtnClick()
	{
		try
		{
			if (pQuXiaoTuoGuan != null)
			{
				return;
			}
            if (MainRoot._pMJGameTable == null || MainRoot._pMJGameTable.GameState != GAMESTATE.Start)
            {//牌局未开始不能托管
                return;
            }
			if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
			{
				RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerStartGoldRoomTuoGuan(MainRoot._gPlayerData.nUserId, true);
			}
		}
		catch (Exception)
		{

		}
	}
	/// <summary>
	/// 进入托管状态
	/// </summary>
	public void ShowTuoGuanUI(bool isTuoGuan)
	{
        MainRoot._gPlayerData.m_bTrustee = isTuoGuan;
        if (isTuoGuan)
		{
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong16,true);
            if (pQuXiaoTuoGuan != null)
			{
				return;
			}
			GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/QuXiaoTuoGuanUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
			pQuXiaoTuoGuan = test.GetComponent<QuXiaoTuoGuanUI>();
			//Debug.Log("Unity:" + "OnTuoGuanBtnClick");
		}
		else
		{
			if (pQuXiaoTuoGuan != null)
			{
				pQuXiaoTuoGuan.DestroyThis();
			}
		}
	}
	/// <summary>
	/// 界面聊天按钮
	/// </summary>
    public void OnChatBtnClick()
    {
        Debug.Log("Unity:"+"OnChatBtnClick");
        if (ChatFace != null)
        {
            ChatFace.SetActive(!ChatFace.activeSelf);
        }
    }
    /// <summary>
    /// 点击表情
    /// </summary>
    public void OnClickChatEmotion(int mEmotionFlag)
    {
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            if (MainRoot._gRoomData)
            {
                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerSendEmotion((int)MainRoot._gRoomData.cCurRoomData.eRoomType, MainRoot._gRoomData.cCurRoomData.nRoomId, mEmotionFlag);
            }
        }
    }
    /// <summary>
    /// 停牌提示按钮
    /// </summary>
    public void OnTipsBtnClick()
    {
        Debug.Log("Unity:"+"OnTipsBtnClick");
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
        {
            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.gameObject.activeSelf)
            {
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.HideTingPaiTishi();
            }
            else
            {
                //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.Initial();
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTiShi();
            }
        }
    }
	/// <summary>
	/// 帮助按钮
	/// </summary>
    public void OnHelpBtnClick()
    {
        Debug.Log("Unity:"+"OnHelpBtnClick");
        MainRoot._gUIModule.pUnModalUIControl.SpawnGameHelpDlg();
		if (RoomCardNet.RoomCardNetClientModule.netModule == null)
		{
			Debug.Log("Unity:" + "Please login!");
			return;
		}
	}
	/// <summary>
	/// 选项按钮
	/// </summary>
    public void OnOptionBtnClick()
    {
        try
        {
            Debug.Log("Unity:"+"OnOptionBtnClick");
            MainRoot._gUIModule.pUnModalUIControl.SpawnGameSetPanel();
        }
        catch (Exception)
        {

        }

    }
    public void MakeSureCallBack()
    {
        //Singleton<ContextManager>.Instance.Pop();

    }
    public void CancelCallBack()
    {
        //Singleton<ContextManager>.Instance.Pop();

    }
	/// <summary>
	/// 创建吃碰胡提示界面
	/// </summary>
	/// <returns></returns>
    public ChiPengHuTips GetChiPengHuTiShi()
    {
        if (pChiPengHuTips == null)
        {
            GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ChiPengHuTips"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
            pChiPengHuTips = temp.GetComponent<ChiPengHuTips>();
            pChiPengHuTips.Initial();
        }
        return pChiPengHuTips;
    }

	/// <summary>
	/// 显示各种标准系统提示的主接口，调用类必须继承自UnModalUIBase
	/// </summary>
	/// <param name="nIndex"></param>
	/// <param name="pCaller"></param>
	/// <param name="param"></param>
	public GameObject ShowOneSysMsgText(int nIndex,UnModalUIBase pCaller,object param = null)  
    {
        if (nIndex < SysMsgArray.Length && MainRoot._gMainRoot.dSysMsgInfoDic.ContainsKey(nIndex))
        {
			if (SysMsgArray[nIndex] != null)
			{
				if (SysMsgArray[nIndex].bIsEvent && SysMsgArray[nIndex].bIsTimer)
				{
					SysMsgArray[nIndex].pEventCaller.OnSysMsgWaitEventHappen(nIndex);
				}
				else
				{
					if (SysMsgArray[nIndex].bIsEvent)
					{
						SysMsgArray[nIndex].pEventCaller.OnSysMsgWaitEventHappen(nIndex);
					}
					else if (SysMsgArray[nIndex].bIsTimer)
					{
						SysMsgArray[nIndex].showtime = 0.0f;
						SysMsgArray[nIndex].SM_OnSysMsgTimeOver();
					}
				}
				SysMsgArray[nIndex] = null;
			}
			GameObject test = (GameObject)GameObject.Instantiate(Resources.Load(MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].sPath),transform, false);
            SystemMsgText pMsg = test.GetComponent<SystemMsgText>();
			SysMsgArray[nIndex] = pMsg;

			if (nIndex == 1)
            {
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SMZhengZaiPiPei = pMsg;
            }
            if (MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].fTime>0) //倒计时类型
            {
                if (nIndex == 1 || nIndex == 3)  //等待匹配，下炮提示
                {
                    pMsg.SM_SetSystemMsgTextWithWaitEvent(nIndex, pCaller, param);
                }
                pMsg.SM_SetSystemMsgTextWithTimeOver(nIndex, MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].fTime, pCaller.OnSysMsgTextTimeOverCallFunc, param);
            }
            if (MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].fTime==0)    //事件类型
            {
                pMsg.SM_SetSystemMsgTextWithWaitEvent(nIndex, pCaller, param);
            }
            return pMsg.gameObject;
        }
        else
        {
            Debug.LogError("Unity:"+"systemmessagetext.xml Not Have Index:"+nIndex.ToString());
        }
        return null;
    }

    /// <summary>
    /// 产生解散好友房间投票面板.
    /// </summary>
    public void SpawnJieSanTouPiao(object[] args)
    {
        if (pJieSanFriendRoom != null)
        {
            return;
        }
        GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameRoomDlg/ED-JieSanTouPiao"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.JieSanTouPiao);
        pJieSanFriendRoom = test.GetComponent<JieSanFriendRoomCtrl>();
        pJieSanFriendRoom.FaQiJieSanPlayerName = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[2]).sUserName;
        pJieSanFriendRoom.InitJieSanRoom(args);
    }
    /// <summary>
    /// 匹配成功.
    /// </summary>
    bool IsMatchPlayerSuccess;
    /// <summary>
    /// 产生匹配成功动画播放预制对象.
    /// </summary>
    public void SpawnMatchPlayerPanel(object[] args)
    {
        if (pMatchPlayerCtrl != null) {
            return;
        }
        IsMatchPlayerSuccess = true;
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/StartAnimation"), transform, false);
        pMatchPlayerCtrl = obj.GetComponent<MatchPlayerCtrl>();
        pMatchPlayerCtrl.InitMatchPlayerInfo(args);

        if (pKaiShiQianTuiChuDlg != null)
        {
            pKaiShiQianTuiChuDlg.DestroyThis();
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameSetPanelCtrl != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameSetPanelCtrl.OnDestoryDlg();
        }
    }

    /// <summary>
    /// 高级结算的控制脚本对象.
    /// </summary>
    public GaoJiJieSuanCtrl pGaoJiJieSuanCtrl;
    /// <summary>
    /// 产生高级结算对象.
    /// </summary>
    public void SpawnGaoJiJieSuanObj(object[] args = null)
    {
        if (pGaoJiJieSuanCtrl != null) {
            return;
        }
        IsShowGaoJiJieSuanPanel = true;
        JieSanBtObj.SetActive(false);
        Transform parentTr = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform;
        GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GaoJiJieSuanUI/ED-GaoJiJieSuan"), parentTr, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.GaoJiJieSuan);
        pGaoJiJieSuanCtrl = test.GetComponent<GaoJiJieSuanCtrl>();
        pGaoJiJieSuanCtrl.InitGaoJiJieSuanInfo(args);

        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_MyRoom:
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                {
                    SetActiveExitBt(true);
                    break;
                }
        }
    }
    public ShareZhanJiCtrl pShareZhanJiCtrl;
    /// <summary>
    /// 产生高级结算分享界面对象.
    /// </summary>
    public void SpawnShareGaoJiJieSuanObj()
    {
        Transform parentTr = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform;
        GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GaoJiJieSuanUI/ED-ShareZhanJi"), parentTr, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.ShareZhanJi);
        pShareZhanJiCtrl = test.GetComponent<ShareZhanJiCtrl>();
    }
	/// <summary>
	/// 根据人数情况控制准备和邀请按钮显示
	/// </summary>
	/// <param name="nIsUserMax"></param>
	public void ShowReadyBtn(bool nIsUserMax)
	{
        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group ||
            MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_MyRoom)
        {
            //淘汰赛、总决赛、自建比赛房间自动匹配玩家进行比赛.
            return;
        }

		gReadyBtn.SetActive(nIsUserMax);
		if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
		{
            SetActiveInvitaBt(false);
		}
		else if (SystemSetManage.AuditVersion.IsCommonRunning)
		{
            SetActiveInvitaBt(!nIsUserMax);
		}

		
	}
	/// <summary>
	/// 隐藏自己客户端的准备按钮和邀请按钮
	/// </summary>
	public void SetReadyAndInvitationFalse()
	{
		gReadyBtn.SetActive(false);
        SetActiveInvitaBt(false);
	}
	/// <summary>
	/// 准备按钮点击响应
	/// </summary>
	public void OnReadyBtnClick()
	{
		if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            gReadyBtn.SetActive(false);
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerReadyCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId);
		}
	}
	/// <summary>
	/// 邀请微信好友按钮点击响应
	/// </summary>
	public void OnInvitaFriendBtnClick()
	{
		if (RoomCardNet.RoomCardNetClientModule.netModule == null)
		{
			Debug.Log("Unity:" + "Please login!");
			return;
		}
		if (MainRoot._gWeChat_Module == null)
		{
			Debug.Log("Unity:" + "_gWeChat_Module is null!");
			return;
		}
		MainRoot._gWeChat_Module.ShareInfoToWeChat(
			MainRoot._gPlayerData.sUserName+"打麻将三缺一喊你加入！房号"
			+MainRoot._gRoomData.cCurRoomData.nRoomId.ToString()+"，快来吧，少年！",
			"秦人麻将"
			);   //微信好友分享

	}
    /// <summary>
    /// 点击开始游戏按键消息.
    /// </summary>
    public void OnStartGameBtClick()
    {
        Debug.Log("Unity:"+"OnStartGameBtClick...");
        SetActiveStartGameBt(false);
		//Net_CallPlayerReadyGame
		switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
		{
            case OneRoomData.RoomType.RoomType_RoomCard:
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnCardRoomReadyNext();
                break;
            case OneRoomData.RoomType.RoomType_Gold:
                //在这里添加金币房下一局的代码.
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
        MainRoot._gGameRoomCenter.gGameRoom.pMainPlayer.SetReadyState(true);//自己已准备
    }
    /// <summary>
    /// 开关邀请微信好友按键UI.
    /// </summary>
    public void SetActiveInvitaBt(bool isActive)
    {
        gInvitaBtn.SetActive(isActive);
    }
    /// <summary>
    /// 设置退出按键的显示/隐藏.
    /// </summary>
    public void SetActiveExitBt(bool isActive)
    {
        ExitBtObj.SetActive(isActive);
    }
    /// <summary>
    /// 开关开始游戏按键UI.
    /// </summary>
    public void SetActiveStartGameBt(bool isActive)
    {
        if (IsShowGaoJiJieSuanPanel && isActive)
        {
            return;
        }
        StartGameBtObj.SetActive(isActive);
    }
    /// <summary>
    /// 产生好友房玩法对话框.
    /// </summary>
    public void SpawnGameWanFaDlg()
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/RoomCardUI/ED-JiaRuPaiJuDlg"), transform, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.JiaRuPaiJuDlg);
        RoomCardInfoCtrl roomCardCom = obj.GetComponent<RoomCardInfoCtrl>();
        roomCardCom.InitRoomCardInfo();
    }
    /// <summary>
    /// 产生好友房未开始时玩家点击退出后,弹出确定退出对话框.
    /// </summary>
    public void SpawnFriendRoomQueDingTuiChuDlg()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-QueDingTuiChu"), transform, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.FriendRoomQueDingTuiChuDlg);
    }
    /// <summary>
    /// 产生未找到对手对话框.
    /// </summary>
    public void SpawnWeiZhaoDaoDuiShouDlg()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-WeiZhaoDaoDuiShou"), transform, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.WeiZhaoDaoDuiShouDlg);
    }
    /// <summary>
    /// 正在匹配消息.
    /// </summary>
    public SystemMsgText SMZhengZaiPiPei;
    /// <summary>
    /// 匹配失败对话框.
    /// </summary>
    public EnsureDlg PiPeiShiBaiTuiChuDlg;
    /// <summary>
    /// 产生匹配失败退出面板.
    /// </summary>
    public void SpawnPiPeiShiBaiTuiChuDlg()
    {
        if (IsMatchPlayerSuccess)
        {
            return;
        }
        OnSysMsgWaitEventHappen(1); //关闭等待匹配消息.
        OnSysMsgWaitEventHappen(75);//删除比赛匹配中

        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    ShowOneSysMsgText(1, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//等待匹配消息
                    return;
                }
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                {
                    GameObject msgObj = ShowOneSysMsgText(75, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//比赛匹配中
                    SystemMsgText msgText = msgObj.GetComponent<SystemMsgText>();
                    msgText.gtimertext.text = MainRoot._gRoomData.cCurRoomData.TimeBiSaiStart.ToString("HH:mm");
                    return;
                }
        }

        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-PiPeiShiBai-TuiChu"), transform, false);
        PiPeiShiBaiTuiChuDlg = obj.GetComponent<EnsureDlg>();
        PiPeiShiBaiTuiChuDlg.Initial(EnsureDlg.EnsureKind.PiPeiShiBai);
    }
    /// <summary>
    /// 房卡游戏中,有用户离开房间.
    /// </summary>
    public void SpawnSMPersonLiKai()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(34, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //房卡游戏中,有用户离开房间.
    }
    /// <summary>
    /// 房卡游戏中,有用户加入房间.
    /// </summary>
    public void SpawnSMPersonJiaRu()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(33, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //房卡游戏中,有用户加入房间.
    }
    /// <summary>
    /// 房卡游戏中,用户等待时间超过1分钟后的等待出牌.
    /// </summary>
    public void SpawnSMDengDaiChuPai()
    {
        if (MainRoot._gRoomData.cCurRoomData == null)
        {
            return;
        }
        if (MainRoot._gRoomData.cCurRoomData.eRoomType != OneRoomData.RoomType.RoomType_RoomCard)
        {
            return;
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSuanUI != null)
        {
            return;
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pGaoJiJieSuanCtrl != null)
        {
            return;
        }
        if (MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.PLAYING
            || MainRoot._gRoomData.cCurRoomData.eRoomState == OneRoomData.RoomStat.PLAYINGWAIT)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(28, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //房卡游戏中,等待出牌.
        }
    }
    /// <summary>
    /// 删除等待出牌.
    /// </summary>
    public void DestroySMDengDaiChuPai()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(28);//删除等待出牌.
    }
    /// <summary>
    /// 删除系统消息流局.
    /// </summary>
    public void DestroySMLiuJu()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(22); //删除系统消息流局.
    }
    /// <summary>
    /// 解散投票通过,房间解散时投票解散.
    /// </summary>
    public void SpawnSMTouPiaoJieSan()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(27, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //解散投票通过,房间解散时投票解散.
    }
    /// <summary>
    /// 解散投票未通过时出现,解散失败.
    /// </summary>
    public void SpawnSMJieSanShiBai()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(26, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //解散投票未通过时出现,解散失败.
    }
    /// <summary>
    /// 其他用户在牌局未开始前点击解散房间,显示只有房主可以申请解散房间.
    /// </summary>
    public void SpawnSMHostOnly()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(25, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他用户在牌局未开始前点击解散房间,显示只有房主可以申请解散房间.
    }
    /// <summary>
    /// 房主解散房间时出现,房主解散房间.
    /// </summary>
    public void SpawnSMHostJieSan()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(24, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//房主解散房间时出现,房主解散房间.
    }
    /// <summary>
    /// 匹配成功牌局开始时.
    /// </summary>
    public void SpawnSMPaiJuKaiShi()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(23, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配成功牌局开始时.
    }
    /// <summary>
    /// 用户的网络从WIFI网络切换至移动网络.
    /// </summary>
    public void SpawnSMQieHuanYD()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(15, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//用户的网络从WIFI网络切换至移动网络.
    }
    /// <summary>
    /// 用户的网络从移动网络切换至wifi网络.
    /// </summary>
    public void SpawnSMQieHuanWifi()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(14, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//用户的网络从移动网络切换至wifi网络.
    }
    /// <summary>
    /// 牌桌解散.
    /// </summary>
    public void SpawnSMPaiJuJieSan()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(13, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//牌桌解散.
    }
    /// <summary>
    /// 赠送道具减金币消息.
    /// indexVal 道具类型.
    /// indexVal == 0 -> 点赞/啤酒.
    /// indexVal == 1 -> 牛粪.
    /// indexVal == 2 -> 鲜花.
    /// indexVal == 3 -> 拖鞋.
    /// userPanel -> 获赠玩家的UserInfoPanel信息.
    /// </summary>
    public void SpawnSMSubCoin(int indexVal, UserInfoPanel userPanel)
    {
        GameObject obj = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(41, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, -200);//减金币.
        DaoJuUICtrl daoJuCom = obj.GetComponent<DaoJuUICtrl>();
        daoJuCom.InitDaoJuUI(indexVal, userPanel);
    }
    /// <summary>
    /// 金币房间点击准备下一局
    /// </summary>
    public void OnGoldRoomReadyNext()
	{
		//case 0xF7B3532://S_CallPlayerReadyGame CRC32HashCode   //玩家准备消息 用于一局结束后，玩家准备下一局
		if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
		{
			RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerReadyGame(false);
		}
		MainRoot._pMJGameTable.ClearGame();

		SetpRemainCount(108);
		for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; i++)
		{
			if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i] != null)
			{
				MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetZhuangjia(false);
				MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetXiaPaoImg(-1);
			}
		}
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//自己已准备
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(49, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配中.
	}
	/// <summary>
	/// 房卡房间点击准备下一局
	/// </summary>
	public void OnCardRoomReadyNext()
	{
		//case 0xF7B3532://S_CallPlayerReadyGame CRC32HashCode   //玩家准备消息 用于一局结束后，玩家准备下一局
		if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
		{
			RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerReadyGame(true);
		}
		MainRoot._pMJGameTable.ClearGame();

		SetpRemainCount(108);
		for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; i++)
		{
			if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i]!=null)
			{
				MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetZhuangjia(false);
				MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetXiaPaoImg(-1);
			}
		}
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//自己已准备
    }

    /// <summary>
    /// 海选赛房间点击准备下一局
    /// </summary>
    public void OnHaiXuanRoomReadyNext()
    {
        //需要向服务端请求是否还有下一局.
        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallHaiXuanSaiClickStartBiSaiBt(MainRoot._gRoomData.cCurRoomData.eMultiRaceType);
        }
        MainRoot._pMJGameTable.ClearGame();

        SetpRemainCount(108);
        for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; i++)
        {
            if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i] != null)
            {
                MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetZhuangjia(false);
                MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetXiaPaoImg(-1);
            }
        }
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//自己已准备
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(49, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配中.
    }
    
    /// <summary>
    /// 淘汰赛房间点击准备下一局
    /// </summary>
    public void OnGroupRoomReadyNext()
    {
        if (ExitBtObj.activeInHierarchy || IsShowGaoJiJieSuanPanel)
        {
            SetActiveStartGameBt(false);
            return;
        }

        //这里添加淘汰赛下一局的网络消息.
        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerReadyGame(true);
        }
        MainRoot._pMJGameTable.ClearGame();

        SetpRemainCount(108);
        for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; i++)
        {
            if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i] != null)
            {
                MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetZhuangjia(false);
                MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetXiaPaoImg(-1);
            }
        }
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//自己已准备
        //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(49, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配中.
    }

    /// <summary>
    /// 自建赛房间点击准备下一局
    /// </summary>
    public void OnMyRoomReadyNext()
    {
        //这里添加自建赛下一局的网络消息.
        //if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        //{
        //    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallHaiXuanSaiClickStartBiSaiBt();
        //}
        MainRoot._pMJGameTable.ClearGame();

        SetpRemainCount(108);
        for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; i++)
        {
            if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i] != null)
            {
                MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetZhuangjia(false);
                MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i].SetXiaPaoImg(-1);
            }
        }
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//自己已准备
        //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(49, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配中.
    }

    /// <summary>
    /// 当玩家在金币房中点击下一局后的判断逻辑.
    /// </summary>
    public void OnPlayerClickNextGameFromGoldRoom(object[] args)
    {
        int playerCoin = (int)args[2];
        int stateShiBai = (int)args[3];
        int[] coinNumMinArray = MainRoot._gPlayerData.GetGameCoinNumMinAy();
        if (MainRoot._gPlayerData != null)
        {
            MainRoot._gPlayerData.PlayerCoin = playerCoin;
            MainRoot._gPlayerData.UpdatePlayerDtInfo();
        }
        switch (stateShiBai)
        {
            case 1:
                Debug.Log("Unity: playerCoin buZu!");
                for (int i = 0; i < coinNumMinArray.Length; i++)
                {
                    if (playerCoin < coinNumMinArray[i])
                    {
                        if (i == 0)
                        {
                            if (!MainRoot._gPlayerData.GetCanBringJiuJiCoin())
                            {
                                MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg((ShopCoinDlgCtrl.ShopCoinEnum)i);
                                break;
                            }
                            else
                            {
                                MainRoot._gUIModule.pUnModalUIControl.SpawnFuLiJinDlg();
                                break;
                            }
                        }
                        else
                        {
                            MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg((ShopCoinDlgCtrl.ShopCoinEnum)i);
                            break;
                        }
                    }
                }
                break;
            case 2:
                Debug.Log("Unity: playerCoin taiDuo!");
                MainRoot._gUIModule.pUnModalUIControl.SpawnJinBiChaoChuDlg((MoleMole.QinRenSelectUIView.SelectRoomEnum)MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel);
                break;
        }
    }
	/// <summary>
	/// 显示指定玩家头像上的出牌提示
	/// </summary>
	/// <param name="nuserid"></param>
	public void ShowCurOperatePlayerEff(int nuserid)
	{
		PlayerBase tempa;
		for (int i = 0; i < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; i++)
		{
			tempa = MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[i];
			if (tempa != null)
			{
				if (tempa.nUserId == nuserid)
				{
					if (tempa.pInfoPanel.pPlayerHeadEff != null)
					{
						tempa.pInfoPanel.pPlayerHeadEff.SetActive(true);
					}
				}
				else
				{
					if (tempa.pInfoPanel.pPlayerHeadEff != null)
					{
						tempa.pInfoPanel.pPlayerHeadEff.SetActive(false);
					}
				}
			}
		}
	}
    
    /// <summary>
    /// 自建比赛模式排行榜名片列表控制脚本.
    /// </summary>
    [HideInInspector]
    public VerticalListUICtrl PaiHangListUICom;
    /// <summary>
    /// 产生自建比赛模式游戏排行榜对话框.
    /// </summary>
    public void SpawnGamePaiHangDlg()
    {
        if (PaiHangListUICom != null)
        {
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/GamePaiHang/ED-GamePaiHang"), transform, false);
        PaiHangListUICom = obj.GetComponent<VerticalListUICtrl>();
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.GamePaiHang);

        VerticalListUICtrl.ObjListConfigDt configDt = new VerticalListUICtrl.ObjListConfigDt();
        int countVal = 50;
        configDt.CountObj = countVal > 64 ? 64 : countVal; //max == 64
        configDt.ObjName = "PlayerPaiHangUI";
        configDt.ObjPrefab = "Prefab/GamePaiHang/" + configDt.ObjName;
        PaiHangListUICom.CreateObjList(configDt); //test.
    }


    /// <summary>
    /// 秦人比赛模式排行榜名片列表控制脚本.
    /// </summary>
    [HideInInspector]
    public QinRenBiSaiPaiHangDlg pQinRenPaiHangDlg;
    /// <summary>
    /// 产生秦人比赛模式游戏排行榜对话框.
    /// </summary>
    public void SpawnGameQinRenPaiHangDlg()
    {
        if (pQinRenPaiHangDlg != null)
        {
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/QinRenBiSaiPaiHang/ED-QinRenBiSaiPaiHang"), transform, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.QinRenBiSaiPaiHang);
        pQinRenPaiHangDlg = obj.GetComponent<QinRenBiSaiPaiHangDlg>();
        pQinRenPaiHangDlg.ShowShopPanel(QinRenBiSaiPaiHangDlg.BiSaiEnum.PaiHang_PANEL);
    }

    /// <summary>
    /// 点击自建比赛房间排名按键。
    /// </summary>
    public void OnClickPaiMingBt()
    {
        Debug.Log("Unity: OnClickPaiMingBt");
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    SpawnGamePaiHangDlg();
                    break;
                }
            case OneRoomData.RoomType.RoomType_OfficialRoom:
                {
                    SpawnGameQinRenPaiHangDlg();
                    break;
                }
        }
    }

    /// <summary>
    /// 收到服务端返回玩家在自建比赛房间的排名信息.
    /// </summary>
    public void OnReceivedPaiMingMsg(int arg)
    {
        int maxPlayer = 32; //test.
        PaiMingInfoTx.text = arg + "/" + maxPlayer;
    }

    /// <summary>
    /// 查看自建比赛房间玩家信息按键
    /// </summary>
    public GameObject LookPlayerListBtObj;
    /// <summary>
    /// 点击查看自建比赛房间玩家信息按键.
    /// </summary>
    public void OnClickLookPlayerListBt()
    {
        Debug.Log("Unity: OnClickLookPlayerListBt");
        MainRoot._gUIModule.pUnModalUIControl.SpawnMyRoomPlayerPanelDlg();
    }
    /// <summary>
    /// 删除下炮中
    /// </summary>
    public void ClearXiaPaoZhong()
    {
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(9);//下炮完毕
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(10);//下炮完毕
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(11);//下炮完毕
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(12);//下炮完毕
    }
}