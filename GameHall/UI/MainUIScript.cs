using UnityEngine;
using System.Collections;
using MoleMole;
using System;

class MainUIScript : UnModalUIBase
{
    /// <summary>
    /// 是否隐藏比赛/主题赛按键.
    /// </summary>
    public bool IsHiddenBiSaiBt = false;
    /// <summary>
    /// 华商大转盘.
    /// </summary>
    public HuaShangDaZhuanPanCtrl pHuaShangDaZhuanPan;
    /// <summary>
    /// GameHall场景中QinRenSelectUIView对象.
    /// </summary>
    [HideInInspector]
	public QinRenSelectUIView pQinRenSelectUIView;
	/// <summary>
	/// GameHall场景里玩家的头像,昵称等信息管理.
	/// </summary>
	public PlayerInfoCtrl pPlayerInfoCtrl;
	public Transform testPanel;
	public WeiXinLoginCtrl pWeiXinLoginCtrl;
	bool b_isClick = false;
	EnsureDlg pZhanJiUI_NULL;
	public GameObject Btn_Promotion;    //主界面的活动按钮
	public GameObject Btn_Gifts;        //主界面的礼包按钮
	EnsureDlg pPromotionUI; //首充活动界面
	EnsureDlg pGiftsUI; //首充活动界面
	public GameObject Btn_GuangBo;	//广播按钮
	//IOS审核隐藏按钮
	public GameObject m_qinren_btn; //秦人麻将按钮
	public GameObject m_bisai_btn;  //比赛房间按钮
	public GameObject m_GameShopBt; //游戏商城按钮
	public GameObject m_BtnYinDao; //引导按钮
	public GameObject m_BtnZhanJi; //战绩按钮
	public GameObject m_BtnInvite; //邀请按钮

	public EnsureDlg WeiXinLoginDlg;
	public EnsureDlg AuditLoginUI;
	public EnsureDlg AuditRegUI;


	void Start()
    {
        nSysMsgArrayLenth = 255;
		SysMsgArray = new SystemMsgText[nSysMsgArrayLenth];
		SpawnTipsMsgText();
		Initial(0);
        //SpawnShouJiHaoYanZhengDlg(); //test.

#if UNITY_EDITOR
        if (testPanel != null)
		{
			testPanel.gameObject.SetActive(true);
		}
		Initial(-1);
#endif

		if (MainRoot._gPlayerData == null || !MainRoot._gPlayerData.isLoginSucceed)
		{
			SpawnWeiXinLoginDlg();
		}
		if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
		{
			InitialAuditMainUI();
		}
		MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong05);

        if (MainRoot._gPlayerData != null && MainRoot._gPlayerData.IsOpenQinRenBiSaiRuKouDlg)
        {
            OnBiSaiBtn_Click();
        }

        if (IsOpenBiSaiSelectUIView)
        {
            Invoke("OnBiSaiBtn_Click", 0.5f);
        }

        if (MainRoot._gMainRoot.IsSpawnHaiXuanOver)
        {
            MainRoot._gMainRoot.IsSpawnHaiXuanOver = false;
            MainRoot._gUIModule.pUnModalUIControl.SpawnJinRiHaiXuanSaiOver();
        }
    }
    /// <summary>
    /// 是否直接打开比赛入口界面.
    /// </summary>
    public bool IsOpenBiSaiSelectUIView = true;
    public void Initial(int nkind)
	{
		MainRoot._gUIModule.pUnModalUIControl.pMainUIScript = this;
		pPlayerInfoCtrl.SetPlayerInfoParent(transform);
		switch (nkind)
		{
			case 0:
				MainRoot._gUIModule.pUnModalUIControl.pMainUIScript = this;
				pPlayerInfoCtrl.SetPlayerInfoParent(transform);
				if (MainRoot._gMainRoot.IsDateInCarPromotionDate())
				{
					MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(50, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);
					if (Btn_Promotion != null)
					{
						Btn_Promotion.SetActive(true);
					}
					if (Btn_Gifts != null)
					{
						if (MainRoot._gPlayerData.isUserGetCarGift == false)
						{
							Btn_Gifts.SetActive(true);
						}
						else
						{
							Btn_Gifts.SetActive(false);
						}
					}
				}
				else
				{
					if (Btn_Promotion != null)
					{
						Btn_Promotion.SetActive(false);
					}
					if (Btn_Gifts != null)
					{
						Btn_Gifts.SetActive(false);
					}
				}

				break;
			case -1:
				if (!testPanel)
					return;
				GameObject test;
				testbtn tb;
				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("微信登录", 121);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("好友分享", 122);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("朋友圈分享", 123);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("内部登录", 124);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("内登1001", 125);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("内登1002", 126);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("内登1003", 127);

				test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/testbtn"), testPanel, false);
				tb = test.GetComponent<testbtn>();
				tb.InitialTestBtn("内登1004", 128);


				break;
			default:
				break;
		}
	}

	public void OnGlobalChatBtn_Click()
	{
		if (MainRoot._gUIModule.pUnModalUIControl.pGlobalChatDlg == null)
		{
			Btn_GuangBo = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-GuangBo"), MainRoot._gUIModule.pMainCanvas.transform, false);
			EnsureDlg temp = Btn_GuangBo.GetComponent<EnsureDlg>();
			temp.Initial(EnsureDlg.EnsureKind.GuangBoSendDlg);
		}
	}
	public void OnQinRenBtn_Click()
	{
		if (MainRoot._gPlayerData == null)
		{
			Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
			return;
		}
		if (RoomCardNet.RoomCardNetClientModule.netModule == null)
		{
			Debug.Log("Unity:" + "Please login!");
			return;
		}
		if (!b_isClick)
		{
			b_isClick = true;
			MoleMole.Singleton<MoleMole.ContextManager>.Instance.Push(new MoleMole.QinRenSelectUIContext());
		}
	}
	/// <summary>
	/// 点击弹出创建麻将馆界面 
	/// </summary>
	public void OnChuangJianBtn_Click()
	{
		if (RoomCardNet.RoomCardNetClientModule.netModule == null)
		{
			Debug.Log("Unity:" + "Please login!");
			return;
		}

		if (MainRoot._gUIModule.pUnModalUIControl.pCreatMjUI == null)
		{
			GameObject test;
			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/CreatMjUI"), MainRoot._gUIModule.pMainCanvas.transform, false);
			MainRoot._gUIModule.pUnModalUIControl.pCreatMjUI = test.GetComponent<CreatMjUI>();
			MainRoot._gUIModule.pUnModalUIControl.pCreatMjUI.Initial(MainRoot._gPlayerData.sUserName);
		}

	}
	public void OnPengYouBtn_Click()
	{
		if (MainRoot._gPlayerData == null) {
			Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
			return;
		}
		if (RoomCardNet.RoomCardNetClientModule.netModule == null)
		{
			Debug.Log("Unity:" + "Please login!");
			return;
		}
		Debug.Log("Unity:" + "OnPengYouBtn_Click");
		RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallGetPlayerCardRoomList();
        //产生房卡房加入面板.
        SpawnRoomCardPanel();
    }
	public void OnBiSaiBtn_Click()
    {
        if (!IsOpenBiSaiSelectUIView)
        {
            if (MainRoot._gPlayerData == null)
            {
                Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                return;
            }
            if (RoomCardNet.RoomCardNetClientModule.netModule == null || !RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect)
            {
                Debug.Log("Unity:" + "Please login!");
                return;
            }
            if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
            {
                Debug.Log("Unity: player is not login!");
                return;
            }
        }
        //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(46, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript); //暂未开放

        if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null
            && MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.IsTiaoGuoThisPanel)
        {
            MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.CheckIsTiaoGuoThisPanel(false);
        }
        else
        {
            if (IsOpenBiSaiSelectUIView)
            {
                GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/BiSaiSelectUIView"), MainRoot._gUIModule.pMainCanvas.transform, false);
                obj.transform.localScale = Vector3.zero;
            }
            else
            {
                if (!b_isClick)
                {
                    b_isClick = true;
                    MoleMole.Singleton<MoleMole.ContextManager>.Instance.Push(new MoleMole.BiSaiSelectUIViewContext());
                }
            }
        }
    }
	/// <summary>
	/// 点击推广按键.
	/// </summary>
	public void OnClickTuiGuangBt()
	{
		Debug.Log("Unity: OnClickTuiGuangBt...");
		SpawnGameTuiGuangDlg();
	}
	/// <summary>
	/// 点击消息按键.
	/// </summary>
	public void OnClickMsgBt()
	{
		Debug.Log("Unity: OnClickMsgBt!");
		//MainRoot._gUIModule.pUnModalUIControl.SpawnGameMsgDlg();

		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/NoticeBoard"), MainRoot._gUIModule.pMainCanvas.transform, false);
		NoticeBoard dlg = obj.GetComponent<NoticeBoard>();
		dlg.Initial(NoticeBoard.NoticeType.SHOW_NOMAL);
	}
	/// <summary>
	/// 点击设置按键.
	/// </summary>
	public void OnClickGameSetBt()
	{
		Debug.Log("Unity: OnClickGameSetBt!");
		MainRoot._gUIModule.pUnModalUIControl.SpawnGameSetPanel();
	}
	/// <summary>
	/// 点击规则按键.
	/// </summary>
	public void OnClickGuiZeBt()
	{
		Debug.Log("Unity: OnClickGuiZeBt!");
		MainRoot._gUIModule.pUnModalUIControl.SpawnGameHelpDlg();
	}
	/// <summary>
	/// 点击引导按键.
	/// </summary>
	public void OnClickYinDaoBt()
	{
		Debug.Log("Unity: OnClickYinDaoBt!");
		SpawnGameYinDaoDlg();
	}
	/// <summary>
	/// 点击分享按键.
	/// </summary>
	public void OnClickFenXiangBt()
	{
		Debug.Log("Unity: OnClickFenXiangBt!");
		SpawnGameFenXiangDlg();
	}
	public void OnQuit_Click()
	{

#if !UNITY_EDITOR
       if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
        {
        }
        else
        {
            MainRoot._gWeChat_Module.RemoveWeChatAuthorize();
        }
#endif  
		//Application.Quit();
		Destroy(MainRoot._gPlayerData.gameObject);
		MainRoot._gPlayerData = null ;
		Destroy(MainRoot._gRoomData.gameObject);
		MainRoot._gRoomData = null;

		MainRoot._gGameSetData = null;


        //
        //MainRoot._gWeChat_Module.DestroyThis();
        //MainRoot._gWeChat_Module = null;

        MainRoot._gGameAudioManage = null;

		MainRoot._gMainRoot._bIsInitialed = false;

		MainRoot._gMainRoot.MainRootInitial();

		SpawnWeiXinLoginDlg(true);
		InitialLoginAndRegUI(true);

        if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            //注册
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerQuitLogin();
        }
    }
	public void OnClickGameShopBt()
	{
		if (RoomCardNet.RoomCardNetClientModule.netModule == null)
		{
			Debug.Log("Unity:" + "Please login!");
			return;
		}
		Debug.Log("Unity:" + "OnClickGameShopBt...");
		SpawnShopPanelObj( ShopPanelCtrl.ShopPanelEnum.DIAMOND_PANEL );
	}
	public void OnClickZhanJiBt()
	{
		//遮挡主界面，防止多次点击
		//MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(47, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);
        MainRoot._gUIModule.pUnModalUIControl.SpawnPlayerZhanJiPanel();
        RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerGetRecordList(0);
		RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerGetRecordList(1);
	}
	public void ResetMainUIDt()
	{
		b_isClick = false;
	}
	/// <summary>
	/// 游戏商城大面板对象.
	/// </summary>
	public ShopPanelCtrl pShopPanel;
	/// <summary>
	/// 产生游戏商城大面板对象.
	/// </summary>
	public void SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum panelSt = ShopPanelCtrl.ShopPanelEnum.COIN_PANEL, Transform tr = null)
	{
		if (pShopPanel != null)
        {
            pShopPanel.ShowShopPanel(panelSt);
            return;
		}
		Transform trParent = tr != null ? tr : transform;
		GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/ShopPanelUI/ShopPanelCtrl"), trParent, false);
		EnsureDlg temp = test.GetComponent<EnsureDlg>();
		temp.Initial(EnsureDlg.EnsureKind.ShopPanelDlg);

		pShopPanel = test.GetComponent<ShopPanelCtrl>();
		pShopPanel.SetShopPanelInfo();
		pShopPanel.ShowShopPanel(panelSt);
    }
    /// <summary>
    /// 产生钻石换金币面板对象.
    /// </summary>
    public void SpawnDiamondToCoinPanel(int diamond, int coin)
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/ED-ZuanShiToCoin"), trParent, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.ZuanShiToCoin);
        temp.p_ShowTextA.text = diamond.ToString();
        temp.p_ShowTextB.text = coin.ToString();
    }
    /// <summary>
    /// 产生钻石换房卡面板对象.
    /// </summary>
    public void SpawnDiamondToCardPanel(int diamond, int card)
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/ED-ZuanShiToCard"), trParent, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.ZuanShiToCard);
        temp.p_ShowTextA.text = diamond.ToString();
        temp.p_ShowTextB.text = card.ToString();
    }
    /// <summary>
    /// 产生钻石不足买金币面板.
    /// </summary>
    public void SpawnDlgZuanShiBuZuMaiCoin()
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/ED-ZuanShiBuZuMaiCoin"), trParent, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.ZuanShiBuZuMaiCoin);
    }
    /// <summary>
    /// 产生钻石不足买房卡面板.
    /// </summary>
    public void SpawnDlgZuanShiBuZuMaiCard()
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/ED-ZuanShiBuZuMaiCard"), trParent, false);
        EnsureDlg temp = test.GetComponent<EnsureDlg>();
        temp.Initial(EnsureDlg.EnsureKind.ZuanShiBuZuMaiCard);
    }
    /// <summary>
    /// 产生游戏推广面板.
    /// </summary>
    public void SpawnGameTuiGuangDlg()
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameTuiGuang/ED-GameTuiGuang"), transform.parent, false);
		EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.GameTuiGuangDlg);
	}
	/// <summary>
	/// 产生游戏分享面板.
	/// </summary>
	public void SpawnGameFenXiangDlg()
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameFenXiangDlg/ED-GameFenXiangDlg"), transform.parent, false);
		EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.GameFenXiangDlg);
	}
	/// <summary>
	/// 产生游戏引导面板.
	/// </summary>
	public void SpawnGameYinDaoDlg()
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameYinDaoDlg/ED-GameYinDaoDlg"), transform.parent, false);
		EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.GameYinDaoDlg);
	}
	/// <summary>
	/// 好友房卡选择的总控制对象.
	/// </summary>
	public RoomCardCtrl pRoomCardCtrl;
	/// <summary>
	/// 产生好友房卡选择的总控制对象.
	/// </summary>
	public void SpawnRoomCardPanel()
	{
		//Debug.Log("Unity:"+"SpawnRoomCardPanel...");
        if (pRoomCardCtrl != null)
        {
            return;
        }
		GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/RoomCardUI/RoomCardCtrl"), transform.parent, false);
		pRoomCardCtrl = obj.GetComponent<RoomCardCtrl>();
		pRoomCardCtrl.IsActiveDlg = false;
	}

    /// <summary>
    /// 初始化好友/自建比赛房卡游戏面板数据.
    /// </summary>
    public void ShowRoomCardJiaRuPanel(object[] args)
    {
        InitRoomCardInfo(args);
    }

	void InitRoomCardInfo(object[] args = null)
	{
		if (pRoomCardCtrl == null) {
			return;
		}
		pRoomCardCtrl.InitRoomCardInfo(args);
	}

	/// <summary>
	/// GameHall的滚动信息控制对象.
	/// </summary>
	public TipsMsgText pTipsMsgText;
	void SpawnTipsMsgText()
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/TipsMsgText"), transform, false);
		pTipsMsgText = obj.GetComponent<TipsMsgText>();
	}

    /// <summary>
    /// 产生房卡购买面板.
    /// </summary>
    public void SpawnShopCardDlg()
	{
        ShopManageCtrl shopManage = new ShopManageCtrl();
        int diamondMin = shopManage.GetMinDiamondToCard();
        if (diamondMin > MainRoot._gPlayerData.GemCount)
        {
            //用户钻石不足,请充钻石购买房卡.
            SpawnShopDiamondCardDlg();
            return;
        }

		GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-ShopCard"), transform, false);
		EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.ShopCardDlg);
    }

    /// <summary>
    /// 产生充钻石购买房卡面板.
    /// </summary>
    public void SpawnShopDiamondCardDlg()
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-ShopDiamondCard"), transform, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.ShopDiamondCardDlg);
    }

    /// <summary>
    /// 产生创建房间数已满,不能继续创建的消息对话框.
    /// </summary>
    public void SpawnRoomCradOverDlg()
	{
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-RoomCradOver"), transform, false);
		EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.RoomCradOverDlg);
	}


	/// <summary>
	/// 显示各种标准系统提示的主接口，调用类必须继承自UnModalUIBase
	/// </summary>
	/// <param name="nIndex"></param>
	/// <param name="pCaller"></param>
	/// <param name="param"></param>
	public GameObject ShowOneSysMsgText(int nIndex, UnModalUIBase pCaller, Transform tr = null, object param = null)
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
			Transform trParent = tr == null ? transform : tr;
			GameObject objMsg = (GameObject)Instantiate(Resources.Load(MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].sPath), trParent, false);
			SystemMsgText pMsg = objMsg.GetComponent<SystemMsgText>();
			SysMsgArray[nIndex] = pMsg;
			if (MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].fTime > 0) //倒计时类型
			{
				if (nIndex == 1 || nIndex == 3)  //等待匹配，下炮提示
				{
					pMsg.SM_SetSystemMsgTextWithWaitEvent(nIndex, pCaller, param);
				}
				pMsg.SM_SetSystemMsgTextWithTimeOver(nIndex, MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].fTime, pCaller.OnSysMsgTextTimeOverCallFunc, param);
				return objMsg;
			}
			if (MainRoot._gMainRoot.dSysMsgInfoDic[nIndex].fTime == 0)    //事件类型
			{
				pMsg.SM_SetSystemMsgTextWithWaitEvent(nIndex, pCaller, param);
            }
            return objMsg;
        }
		else
		{
			Debug.LogError("Unity:" + "systemmessagetext.xml Not Have Index:" + nIndex.ToString());
            return null;
		}
	}

    EnsureDlg DlgPaiJuWeiWanCheng;
	/// <summary>
	/// 产生登陆时牌局未完成对话框.
	/// </summary>
	public void SpawnLoginPaiJuWeiWanChengDlg()
	{
        if (DlgPaiJuWeiWanCheng != null)
        {
            return;
        }
		Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
		//if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
		//{
		//	trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
		//	if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
		//		&& !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick)
		//	{
		//		trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
		//	}
		//}
		GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-PaiJuWeiWanCheng"), trParent, false);
        DlgPaiJuWeiWanCheng = obj.GetComponent<EnsureDlg>();
        DlgPaiJuWeiWanCheng.Initial(EnsureDlg.EnsureKind.PaiJuWeiWanChengDlg);
	}
	/// <summary>
	/// 产生从商城成功购买金币的系统消息.
	/// </summary>
	public void SpawnSMByCoin()
	{
		if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
			Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
			if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
				&& !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick) {
				trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
			}
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(37, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent); //购买金币成功.
		}
	}
	/// <summary>
	/// 产生从商城成功购买房卡的系统消息.
	/// </summary>
	public void SpawnSMByCard()
	{
		if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
			Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
			if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
				&& !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick) {
				trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
			}
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(36, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent); //购买房卡成功.
		}
	}
	/// <summary>
	/// 产生同意协议的系统消息.
	/// </summary>
	public void SpawnSMXieYiAgree()
	{
		if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
		{
			Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
			if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
				&& !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick)
			{
				trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
			}
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(38, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);
		}
	}
	/// <summary>
	/// 主界面邀请微信好友
	/// </summary>
	public void OnInviteFriendBtnClick()
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
		MainRoot._gWeChat_Module.ShareInfoToWeChat();
	}
	/// <summary>
	/// 产生微信登陆面板.
	/// </summary>
	void SpawnWeiXinLoginDlg(bool isReInit=false)
	{
		if (WeiXinLoginDlg!=null)
		{
			WeiXinLoginDlg.DestroyThis(); 
		}
		GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-DengLu"), transform, false);
		WeiXinLoginDlg = obj.GetComponent<EnsureDlg>();
		WeiXinLoginDlg.Initial(EnsureDlg.EnsureKind.WeiXinLoginDlg);
#if !UNITY_EDITOR

		WeiXinLoginDlg.btn3.SetActive(false);
#endif
		pWeiXinLoginCtrl = obj.GetComponent<WeiXinLoginCtrl>();
		if (!isReInit)
		{
			pWeiXinLoginCtrl.InitialTips();
		}
		
		if (MainRoot._gMainRoot.IsDateInCarPromotionDate())
		{
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(48, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);//产生华商车展loading.
		}
	}
	/// <summary>
	/// 产生微信登陆成功系统消息.
	/// </summary>
	public void SpawnDengLuLianJieChengGong()
	{
		if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
		{
			Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(39, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生微信登陆成功系统消息.
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.OnSysMsgWaitEventHappen(40);
			if (MainRoot._gWeChat_Module.IsWeChatAuthorized())
			{
				Debug.Log("Unity:" + "WeChat Authorize Login!");
				MainRoot._gWeChat_Module.GetWeChatUserInfo();   //获取微信用户信息
			}
			else
			{
				if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
				{
					Debug.Log("Unity:" + "WeChat Authorize Login!");
				}
				else if (SystemSetManage.AuditVersion.IsCommonRunning)
				{
					MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl.LoginBtObj.SetActive(true);
				}
			}
		}
	}
	/// <summary>
	/// 产生登陆连接中系统消息.
	/// </summary>
	public void SpawnDengLuLianJieZhong()
	{
		if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
		{
			Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
			MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(40, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生登陆连接中系统消息.
		}
    }
    /// <summary>
    /// 产生"比赛时间为XX:XX至XX:XX, 请稍后进入"系统消息.
    /// </summary>
    public void SpawnHaiXuanShiJianWeiDao(DateTime startTime, DateTime endTime)
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            GameObject objMsg = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(64, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"比赛时间为XX:XX至XX:XX, 请稍后进入"系统消息.
            if (startTime != null && endTime != null)
            {
                SystemMsgText msgTx = objMsg.GetComponent<SystemMsgText>();
                msgTx.gmsgtext.text = "比赛时间为" + startTime.ToString("HH:mm") + "至" + endTime.ToString("HH:mm") + "，请稍后进入！";
            }
        }
    }
    /// <summary>
    /// 产生"您已使用了免费参赛的资格"系统消息.
    /// </summary>
    public void SpawnYiShiYongMFCanSai()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(65, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"您已使用了免费参赛的资格"系统消息.
        }
    }
    /// <summary>
    /// 产生"您已获得淘汰赛资格，无须重复参加海选赛"系统消息.
    /// </summary>
    public void SpawnYiHuoDeTaoTaiSaiZG()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(66, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"您已获得淘汰赛资格，无须重复参加海选赛"系统消息.
        }
    }
    /// <summary>
    /// 产生"比赛尚未开始，请稍后再试"系统消息.
    /// </summary>
    public void SpawnSMBiSaiWeiKaiShi()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(72, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"比赛尚未开始，请稍后再试"系统消息.
        }
    }
    /// <summary>
    /// 产生"比赛已开始"系统消息.
    /// </summary>
    public void SpawnSMBiSaiYiKaiShi()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(73, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"比赛已开始"系统消息.
        }
    }
    /// <summary>
    /// 产生"比赛已开始，系统视为自动退赛！"系统消息.
    /// </summary>
    public void SpawnSMBiSaiYiKaiShi_TuiSai()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(74, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"比赛已开始，系统视为自动退赛！"系统消息.
        }
    }
    /// <summary>
    /// 产生"请输入准确的手机号码"系统消息.
    /// </summary>
    public void SpawnReinputShouJiHao()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(68, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"请输入准确的手机号码"系统消息.
        }
    }
    /// <summary>
    /// 产生"您的验证码已过期，请重新获取"系统消息.
    /// </summary>
    public void SpawnReclickGetYanZhengMa()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(69, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"您的验证码已过期，请重新获取"系统消息.
        }
    }
    /// <summary>
    /// 产生"您的验证码输入错误，请重新输入"系统消息.
    /// </summary>
    public void SpawnYanZhengMaCuoWuMsg()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(70, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"您的验证码输入错误，请重新输入"系统消息.
        }
    }
    /// <summary>
    /// 产生"验证成功，祝您游戏愉快"系统消息.
    /// </summary>
    public void SpawnYanZhengMaZhengQueMsg()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(71, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"验证成功，祝您游戏愉快"系统消息.
        }
    }
    /// <summary>
    /// 产生"验证成功，祝您游戏愉快"系统消息(社区代码验证成功).
    /// </summary>
    public void SpawnSheQuDaiMaChengGongMsg()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(85, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"验证成功，祝您游戏愉快"系统消息.
        }
    }
    /// <summary>
    /// 产生"社区代码错误"系统消息.
    /// </summary>
    public void SpawnSheQuDaiMaCuoWuMsg()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(86, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"社区代码错误"系统消息.
        }
    }
    /// <summary>
    /// 产生"无法连接到 iTunes Store"的系统消息.
    /// </summary>
    public void SpawnSMNotLinkiTunesStore()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(76, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent); //无法连接到 iTunes Store.
        }
    }
    /// <summary>
    /// 产生"该账号当前不能用于支付"的系统消息.
    /// </summary>
    public void SpawnSMZhangHaoNotZhiFu()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(77, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent); //该账号当前不能用于支付.
        }
    }
    /// <summary>
    /// 首充活动按钮点击消息
    /// </summary>
    public void OnPromotionBtnClick()
	{
		if (pPromotionUI == null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("101-CarExhibition-Activity/101-Prefab/ED-101-CarExhibition-Activity"), transform, false);
			pPromotionUI = obj.GetComponent<EnsureDlg>();
			pPromotionUI.Initial(EnsureDlg.EnsureKind.PromotionDlg);
		}

	}
	/// <summary>
	/// 车展礼包按钮点击消息
	/// </summary>
	public void OnGiftsBtnClick()
	{
		//ED-101-CarExhibition-Gifts
		if (pGiftsUI == null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("101-CarExhibition-Activity/101-Prefab/ED-101-CarExhibition-Gifts"), transform, false);
			pGiftsUI = obj.GetComponent<EnsureDlg>();
			pGiftsUI.Initial(EnsureDlg.EnsureKind.GiftsDlg);
		}
	}
	/// <summary>
	/// 如果首充和领取界面已经打开，刷它。
	/// </summary>
	public void FreshPromotionGiftData()
	{
		if (pPromotionUI != null)
		{
			if (pPromotionUI.pPromotionUI_GetBtn != null && pPromotionUI.p_ShowTextA != null)
			{
				switch (MainRoot._gPlayerData.isUserPromotioned)
				{
					case -1:
						{
							pPromotionUI.pPromotionUI_GetBtn.SetActive(true);
							pPromotionUI.p_ShowTextA.text = "";
							break;
						}
					case 0:
						{
							pPromotionUI.pPromotionUI_GetBtn.SetActive(false);
							pPromotionUI.p_ShowTextA.text = MainRoot._gPlayerData.sUserPromotionCode + "    (未领取)";
							break;
						}
					case 1:
						{
							pPromotionUI.pPromotionUI_GetBtn.SetActive(false);
							pPromotionUI.p_ShowTextA.text = "已领取"; break;
						}
					default:
						break;
				}
			}
		}
		if (MainRoot._gPlayerData.isUserGetCarGift)
		{
			if (pGiftsUI != null)
			{
				pGiftsUI.DestroyThis();
				pGiftsUI = null;
			}
			if (Btn_Gifts != null)
			{
				Btn_Gifts.SetActive(false);
			}
		}
	}
	/// <summary>
	/// 初始化审核登录界面
	/// </summary>
	public void InitialAuditLogin()
	{
		if (AuditLoginUI == null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-YongHuDengLu"), transform, false);
			AuditLoginUI = obj.GetComponent<EnsureDlg>();
			AuditLoginUI.Initial(EnsureDlg.EnsureKind.YongHuDengLu);
			if (WeiXinLoginDlg != null)
			{
				WeiXinLoginDlg.btn1.SetActive(false);
			}
		}
	}
	public void CloseAuditLogin_RegUI()
	{
		if (AuditLoginUI != null)
		{
			AuditLoginUI.DestroyThis();
		}
		if (AuditRegUI != null)
		{
			AuditRegUI.DestroyThis();
		}
	}
	/// <summary>
	/// 初始化审核注册界面
	/// </summary>
	public void InitialAuditReg()
	{
		if (AuditRegUI == null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-YongHuZhuCe"), transform, false);
			AuditRegUI = obj.GetComponent<EnsureDlg>();
			AuditRegUI.Initial(EnsureDlg.EnsureKind.YongHuZhuCe);

		}
	}
	public void InitialAuditMainUI()
	{
        if(SystemSetManage.AuditVersion.IsIOSAudit)
        {
            //if (m_qinren_btn != null)
            //m_qinren_btn.transform.position = new Vector3(-10000, m_qinren_btn.transform.position.y, m_qinren_btn.transform.position.z);
            if (m_bisai_btn != null)
                m_bisai_btn.transform.position = new Vector3(-10000, m_bisai_btn.transform.position.y, m_bisai_btn.transform.position.z);
            //if (m_GameShopBt != null)
            //	m_GameShopBt.transform.position = new Vector3(-10000, m_GameShopBt.transform.position.y, m_GameShopBt.transform.position.z);
            if (m_BtnYinDao != null)
            	m_BtnYinDao.transform.position = new Vector3(-10000, m_BtnYinDao.transform.position.y, m_BtnYinDao.transform.position.z);
            //if (m_BtnZhanJi != null)
            //	m_BtnZhanJi.transform.position = new Vector3(-10000, m_BtnZhanJi.transform.position.y, m_BtnZhanJi.transform.position.z);
            if (m_BtnInvite != null)
                m_BtnInvite.transform.position = new Vector3(-10000, m_BtnInvite.transform.position.y, m_BtnInvite.transform.position.z);
            if (Btn_GuangBo != null)
                Btn_GuangBo.transform.position = new Vector3(-10000, Btn_GuangBo.transform.position.y, Btn_GuangBo.transform.position.z);
        }
        else if(SystemSetManage.AuditVersion.IsMyAppAudit)
        {
            //if (m_qinren_btn != null)
            //m_qinren_btn.transform.position = new Vector3(-10000, m_qinren_btn.transform.position.y, m_qinren_btn.transform.position.z);
            if (m_bisai_btn != null)
                m_bisai_btn.transform.position = new Vector3(-10000, m_bisai_btn.transform.position.y, m_bisai_btn.transform.position.z);
            //if (m_GameShopBt != null)
            //	m_GameShopBt.transform.position = new Vector3(-10000, m_GameShopBt.transform.position.y, m_GameShopBt.transform.position.z);
            //if (m_BtnYinDao != null)
            //	m_BtnYinDao.transform.position = new Vector3(-10000, m_BtnYinDao.transform.position.y, m_BtnYinDao.transform.position.z);
            //if (m_BtnZhanJi != null)
            //	m_BtnZhanJi.transform.position = new Vector3(-10000, m_BtnZhanJi.transform.position.y, m_BtnZhanJi.transform.position.z);
            if (m_BtnInvite != null)
                m_BtnInvite.transform.position = new Vector3(-10000, m_BtnInvite.transform.position.y, m_BtnInvite.transform.position.z);
            if (Btn_GuangBo != null)
                Btn_GuangBo.transform.position = new Vector3(-10000, Btn_GuangBo.transform.position.y, Btn_GuangBo.transform.position.z);
        }
        else if (SystemSetManage.AuditVersion.IsCopyRightAudit)
        {
            //if (m_qinren_btn != null)
            //m_qinren_btn.transform.position = new Vector3(-10000, m_qinren_btn.transform.position.y, m_qinren_btn.transform.position.z);
            if (m_bisai_btn != null)
                m_bisai_btn.transform.position = new Vector3(-10000, m_bisai_btn.transform.position.y, m_bisai_btn.transform.position.z);
            //if (m_GameShopBt != null)
            //	m_GameShopBt.transform.position = new Vector3(-10000, m_GameShopBt.transform.position.y, m_GameShopBt.transform.position.z);
            //if (m_BtnYinDao != null)
            //	m_BtnYinDao.transform.position = new Vector3(-10000, m_BtnYinDao.transform.position.y, m_BtnYinDao.transform.position.z);
            //if (m_BtnZhanJi != null)
            //	m_BtnZhanJi.transform.position = new Vector3(-10000, m_BtnZhanJi.transform.position.y, m_BtnZhanJi.transform.position.z);
            if (m_BtnInvite != null)
                m_BtnInvite.transform.position = new Vector3(-10000, m_BtnInvite.transform.position.y, m_BtnInvite.transform.position.z);
            if (Btn_GuangBo != null)
                Btn_GuangBo.transform.position = new Vector3(-10000, Btn_GuangBo.transform.position.y, Btn_GuangBo.transform.position.z);
        }


    }
	public void InitialLoginAndRegUI(bool isReInit=false)
	{
		//DengLuLianJieZhong
		if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
		{
			InitialAuditLogin();
			InitialAuditMainUI();
		}
		else if (SystemSetManage.AuditVersion.IsCommonRunning)
		{
			SpawnWeiXinLoginDlg(isReInit);
			if (WeiXinLoginDlg != null)
			{
				WeiXinLoginDlg.btn1.SetActive(true);
			}
		}
    }

    /// <summary>
    /// 手机号验证对话框.
    /// </summary>
    [HideInInspector]
    public ShouJiHaoYanZhengDlg pShouJiHaoYanZhengDlg;
    /// <summary>
    /// 产生手机号验证对话框.
    /// </summary>
    public void SpawnShouJiHaoYanZhengDlg()
    {
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-ShouJiHaoYanZheng"),
                            transform.parent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.ShouJiHaoYanZheng);
        pShouJiHaoYanZhengDlg = obj.GetComponent<ShouJiHaoYanZhengDlg>();
        pShouJiHaoYanZhengDlg.Init();
    }


    /// <summary>
    /// 社区赛代码验证对话框.
    /// </summary>
    [HideInInspector]
    public SheQuSaiDaiMaDlg pSheQuSaiYanZhengMaDlg;
    /// <summary>
    /// 产生手机号验证对话框.
    /// </summary>
    public void SpawnSheQuSaiYanZhengMaDlg()
    {
        if (pSheQuSaiYanZhengMaDlg != null)
        {
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-SheQuSaiDaiMa"),
                            MainRoot._gUIModule.pMainCanvas.transform, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.SheQuSaiDaiMaYanZheng);
        pSheQuSaiYanZhengMaDlg = obj.GetComponent<SheQuSaiDaiMaDlg>();
        pSheQuSaiYanZhengMaDlg.Init();
    }

    EnsureDlg pBiSaiBuZuDlg;
    /// <summary>
    /// 产生比赛卡不足面板.
    /// </summary>
    public void SpawnDlgBiSaiKaBuZu()
    {
        if (pBiSaiBuZuDlg != null)
        {
            return;
        }
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/ED-BiSaiKaBuZu"), MainRoot._gUIModule.pMainCanvas.transform, false);
        pBiSaiBuZuDlg = test.GetComponent<EnsureDlg>();
        pBiSaiBuZuDlg.Initial(EnsureDlg.EnsureKind.BiSaiKaBuZu);
    }

    public void SpawnDlgHuaShangDaZhuanPan()
    {
        if (pHuaShangDaZhuanPan != null)
        {
            return;
        }
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/HuaShangDaZhuanPan/ED-HuaShangDaZhuanPan"), MainRoot._gUIModule.pMainCanvas.transform, false);
        EnsureDlg dlg = test.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.HuaShangDaZhuanPan);
        pHuaShangDaZhuanPan = test.GetComponent<HuaShangDaZhuanPanCtrl>();
        pHuaShangDaZhuanPan.Init();
    }

    /// <summary>
    /// 是否隐藏比赛房间/主题比赛按键.
    /// </summary>
    public void SetIsHiddenBiSaiBt(bool isHidden)
    {
        IsHiddenBiSaiBt = isHidden;
        m_bisai_btn.SetActive(!IsHiddenBiSaiBt);
    }

    /// <summary>
    /// 产生秦人麻将公众号预制.
    /// </summary>
    public void SpawnDlgQRGongZhongHao()
    {
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/HuaShangDaZhuanPan/ED-QRGongZhongHao"), MainRoot._gUIModule.pMainCanvas.transform, false);
        EnsureDlg dlg = test.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.QRGongZhongHao);
    }
}