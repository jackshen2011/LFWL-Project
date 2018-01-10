using UnityEngine;
using System.Collections;
using MoleMole;
using RoomCardNet;
using System;

/// <summary>
/// 通用确认界面，有三个button，根据初始化的类型不同，进行不同的点击响应处理
/// </summary>
class EnsureDlg : UnModalUIBase
{
    /// <summary>
    /// JieSanPaiJu   -> 解散好友房间牌局.
    /// JieSanTouPiao -> 解散好友房间投票界面.
    /// GaoJiJieSuan  -> 高级结算界面.
    /// ShareZhanJi   -> 分享玩家高级结算战绩面板.
    /// ShopPanelDlg  -> 游戏商城大面板.
    /// ShopCoinDlg   -> 游戏购买金币面板.
    /// RoomCradOverDlg -> 创建房间数已满,不能继续创建.
    /// PaiJuWeiWanChengDlg -> 登录后,还有牌局未完成.
    /// FriendRoomQueDingTuiChuDlg -> 好友房牌局未开始时退出对话框.
    /// WeiZhaoDaoDuiShouDlg -> 未找到对手对话框.
    /// WeiXinLoginDlg -> 微信登陆秦人麻将游戏.
    /// QiTaDengLuDlg -> 其它登陆/重复登陆.
    /// PlayerZhanJiPanelDlg -> 玩家战绩面板.
    /// GameSetPanelDlg -> 游戏设置面板.
    /// GameTuiGuangDlg -> 游戏推广面板.
    /// GameHelpDlg -> 游戏帮助面板.
    /// GameYinDaoDlg -> 游戏引导面板.
    /// GameFenXiangDlg -> 游戏分享面板.
    /// GameMsgDlg -> 游戏消息面板.
    /// JiaRuPaiJuDlg -> 加入牌局/好友房牌局玩法对话框.
    /// BanBenGengXin -> 版本更新提醒
    /// WLYC_CXDL -> 网络异常，请重新登陆 
    /// PaiJuBuZaiDlg -> 牌局不存在
    /// Game_WEIHU_Dlg ->游戏维护
    /// PromotionDlg -> 首充活动
    /// GiftsDlg -> 车展礼包
    /// GuangBoSendDlg -> 发送广播
    /// XiaZhuDlg -> 下注
    /// YongHuDengLu 审核用户登录
    /// YongHuZhuCe 审核用户注册
    /// GamePaiHang 自建比赛模式时游戏排行榜.
    /// HaiXuanRuKouDlg 海选入口界面.
    /// ShouJiHaoYanZheng 手机号验证界面.
    /// HaiXuanRuChangQuan 海选入场券购买界面.
    /// TaoTaiSaiRuKou 淘汰赛入口界面.
    /// MyRoomRuKou 自建比赛入口界面.
    /// MyRoomPlayerPanel 自建比赛房间里玩家列表数据面板.
    /// BiSaiQianEixt 比赛前退出.
    /// BiSaiKaBuZu 比赛卡不足.
    /// HuaShangDaZhuanPan 华商大转盘.
    /// ThemeRaceXiangQing 主题赛详情
    /// QRGongZhongHao 亲人麻将公众号.
    /// ZuanShiBuZuMaiCoin 钻石不足买金币.
    /// ZuanShiBuZuMaiCard 钻石不足买房卡.
    /// ShopDiamondCoinDlg 充钻石购买金币.
    /// ShopDiamondCardDlg 充钻石购买房卡.
    /// ZuanShiToCoin 钻石换金币.
    /// ZuanShiToCard 钻石换房卡.
    /// QinRenBiSaiChangDlg 亲人比赛场入口面板.
    /// KaiShiQian_TuiChu_QinRenBiSai 游戏开始前退出秦人比赛场景.
    /// QinRenBiSaiPaiHang 秦人比赛场游戏中玩家排行榜.
    /// YouXiZhong_TuiChu_QinRenBiSai 秦人比赛场游戏中退出提示界面.
    /// SheQuSaiDaiMaYanZheng 社区赛代码验证.
    /// </summary>
    public enum EnsureKind
    {
        FuLiJin01,
        FuLiJin02,
        JinBiChaoChu,
        QuXiaoPiPei,
        PiPeiShiBai,
        JinRuTuoGuan,
        JieSanPaiJu,
        JieSanTouPiao,
        GaoJiJieSuan,
        ShareZhanJi,
        ShopPanelDlg,
        ShopCoinDlg,
        ShopCardDlg,
        RoomCradOverDlg,
        PaiJuWeiWanChengDlg,
        FriendRoomQueDingTuiChuDlg,
        WeiZhaoDaoDuiShouDlg,
        WeiXinLoginDlg,
        QiTaDengLuDlg,
        PlayerZhanJiPanelDlg,
        GameSetPanelDlg,
        GameTuiGuangDlg,
        GameHelpDlg,
        GameYinDaoDlg,
        GameFenXiangDlg,
        GameMsgDlg,
        JiaRuPaiJuDlg,
		BanBenGengXin,
		UserXieYi,
		WLYC_CXDL,
		PaiJuBuZaiDlg,
		Game_WEIHU_Dlg,
		PromotionDlg,
		GiftsDlg,
		GuangBoSendDlg,
		XiaZhuDlg,
		YongHuDengLu,
		YongHuZhuCe,
        GamePaiHang,
        HaiXuanRuKouDlg,
        ShouJiHaoYanZheng,
        HaiXuanRuChangQuan,
        TaoTaiSaiRuKou,
        ZongJueSaiRuKou,
        MyRoomRuKou,
        MyRoomPlayerPanel,
        BiSaiQianEixt,
        BiSaiKaBuZu,
        HuaShangDaZhuanPan,
		ThemeRaceXiangQing,
        QRGongZhongHao,
        ZuanShiBuZuMaiCoin,
        ZuanShiBuZuMaiCard,
        ShopDiamondCoinDlg,
        ShopDiamondCardDlg,
        ZuanShiToCoin,
        ZuanShiToCard,
        QinRenBiSaiChangDlg,
        KaiShiQian_TuiChu_QinRenBiSai,
        QinRenBiSaiPaiHang,
        YouXiZhong_TuiChu_QinRenBiSai,
        SheQuSaiDaiMaYanZheng,
    }
    //[HideInInspector]
    public GameObject btn1;
    public GameObject btn2;
    public GameObject btn3;

	public GameObject pPromotionUI_GetBtn; //首充活动界面的领取按钮
	public TextBase p_ShowTextA;   //首充活动界面的领取码显示,广播发送界面的输入内容,下注金币总数
	public TextBase p_ShowTextB;    //广播输入的字数，可下注金币
	public TextBase p_ShowTextC;    //下注输入的数字

	public InputFieldBase p_input1; //标准输入框1
	public InputFieldBase p_input2; //标准输入框2
	public InputFieldBase p_input3; //标准输入框3
	public InputFieldBase p_input4; //标准输入框4

	/// <summary>
	/// 界面类型
	/// </summary>
	public int nEnsurekind = -1;
	/// <summary>
	/// 初始化确认界面类型
	/// </summary>
	
	//客户端版本号暂存
	public string sTempVer = "";

	[HideInInspector]
	float locktime = 0.3f;
	bool isbtnlock = false;
	/// <summary>
	/// 按键锁
	/// </summary>
	void LockTimer()
	{
		isbtnlock = true;
	}
	/// <summary>
	/// 解除按键锁
	/// </summary>
	void UnlockTimer()
	{
		isbtnlock = false;
	}
	public void Initial(EnsureKind ekind,string sverurl = "")
    {
        if (System.Enum.IsDefined(typeof(EnsureKind), ekind))
        {

			nEnsurekind = (int)ekind;
			if (sverurl != "")
			{
				sTempVer = sverurl;
			}
			switch (ekind)
			{
				case EnsureKind.FuLiJin01:
					break;
				case EnsureKind.FuLiJin02:
					break;
				case EnsureKind.JinBiChaoChu:
					break;
				case EnsureKind.QuXiaoPiPei:
					break;
				case EnsureKind.PiPeiShiBai:
					break;
				case EnsureKind.JinRuTuoGuan:
					break;
				case EnsureKind.JieSanPaiJu:
					break;
				case EnsureKind.JieSanTouPiao:
					break;
				case EnsureKind.GaoJiJieSuan:
					break;
				case EnsureKind.ShareZhanJi:
					if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
					{
						gameObject.SetActive(false);
						DestroyObject(gameObject);
					}
					break;
				case EnsureKind.ShopPanelDlg:
					break;
				case EnsureKind.ShopCoinDlg:
					break;
				case EnsureKind.ShopCardDlg:
					break;
				case EnsureKind.RoomCradOverDlg:
					break;
				case EnsureKind.PaiJuWeiWanChengDlg:
					break;
				case EnsureKind.FriendRoomQueDingTuiChuDlg:
					break;
				case EnsureKind.WeiZhaoDaoDuiShouDlg:
					break;
				case EnsureKind.WeiXinLoginDlg:
					break;
				case EnsureKind.QiTaDengLuDlg:
					break;
				case EnsureKind.PlayerZhanJiPanelDlg:
					break;
				case EnsureKind.GameSetPanelDlg:
					break;
				case EnsureKind.GameTuiGuangDlg:
					break;
				case EnsureKind.GameHelpDlg:
					break;
				case EnsureKind.GameYinDaoDlg:
					break;
				case EnsureKind.GameFenXiangDlg:
					break;
				case EnsureKind.GameMsgDlg:
					break;
				case EnsureKind.JiaRuPaiJuDlg:
					break;
				case EnsureKind.BanBenGengXin:
					break;
				case EnsureKind.UserXieYi:
					break;
				case EnsureKind.WLYC_CXDL:
					break;
				case EnsureKind.PaiJuBuZaiDlg:
					break;
				case EnsureKind.Game_WEIHU_Dlg:
					break;
				case EnsureKind.PromotionDlg:
					if (pPromotionUI_GetBtn != null && p_ShowTextA!=null)
					{
						switch (MainRoot._gPlayerData.isUserPromotioned)
						{
							case -1:
								{
									pPromotionUI_GetBtn.SetActive(true);
									p_ShowTextA.text = "";
									break;
								}
							case 0:
								{
									pPromotionUI_GetBtn.SetActive(false);
									p_ShowTextA.text = MainRoot._gPlayerData.sUserPromotionCode +"    (未领取)";
									break;
								}
							case 1:
								{
									pPromotionUI_GetBtn.SetActive(false);
									p_ShowTextA.text = "已领取"; break;
								}
							default:
								break;
						}
					}
					break;
				case EnsureKind.GiftsDlg:
					break;
				case EnsureKind.XiaZhuDlg:
					if (p_ShowTextA!=null)
					{
						p_ShowTextA.text = UserInfoPanel.GetPlayerCoinValInfo(MainRoot._gPlayerData.PlayerCoin);
					}
					if (p_ShowTextB != null)
					{
						p_ShowTextB.text = UserInfoPanel.GetPlayerCoinValInfo(Mathf.Max(MainRoot._gPlayerData.PlayerCoin-100000,0));
					}
					break;
				default:
					break;
			}
		}
    }
    /// <summary>
    /// 标准确认界面的按钮点击响应
    /// </summary>
    /// <param name="nbtn">1,2,3三个按钮</param>
    public void OnBtnClick(int nbtn)
    {
        switch (nbtn)
        {
            case 1:
                OnBtn_1_Click();
                break;
            case 2:
                OnBtn_2_Click();
                break;
            case 3:
                OnBtn_3_Click();
                break;
            default:
				OnBtn_4_Click();
                break;
        }
    }
   	/// <summary>
   	/// 标准确认界面的第一个按钮点击响应
   	/// </summary>
    void OnBtn_1_Click()
    {
		if (isbtnlock)
		{
			return;
		}
		LockTimer();
		Invoke("UnlockTimer", locktime);
		switch ((EnsureKind)nEnsurekind)
        {
            case EnsureKind.FuLiJin01://当日第一次领取福利金
            case EnsureKind.FuLiJin02://当日第二次领取福利金
                if ((MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
                    || MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null) {
                    Debug.Log("Unity:"+"lingQU fuLiJin...");
                    if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    {
                        RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerJiuJiu();
                    }
                    DestroyThis();
                }
                break;
            case EnsureKind.JinBiChaoChu://金币超出取消
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                    DestroyThis();
                    MainRoot._gMainRoot.ChangeScene("GameHall");//取消后退出金币房间
                }
                break;
            case EnsureKind.QuXiaoPiPei://取消匹配取消
                DestroyThis();
                break;
            case EnsureKind.KaiShiQian_TuiChu_QinRenBiSai:
                {
                    Debug.Log("Unity: click KaiShiQian_TuiChu_QinRenBiSai bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.YouXiZhong_TuiChu_QinRenBiSai:
                {
                    Debug.Log("Unity: click YouXiZhong_TuiChu_QinRenBiSai bt2");
                    DestroyThis();
                    break;
                }
            case EnsureKind.PiPeiShiBai://匹配失败退出
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                    DestroyThis();
                    MainRoot._gMainRoot.ChangeScene("GameHall");//确认托管后退出金币房间
                }
                break;
            case EnsureKind.JinRuTuoGuan://进入托管取消
				DestroyThis();
				break;
            case EnsureKind.JieSanPaiJu://申请解散好友房间.
                Debug.Log("Unity:"+"apply kill game!");
                DestroyThis();
                if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallRoomOwnerDissolutionCardRoom(1,
                        MainRoot._gRoomData.cCurRoomData.nRoomId);
                }
                break;
            case EnsureKind.JieSanTouPiao://拒绝解散好友房间.
                Debug.Log("Unity:"+"refuse kill game!");
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMJieSanShiBai();
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom != null) {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom.HiddenDlgBt(JieSanFriendRoomCtrl.BtEnum.RefuseBt);
                }
                if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerAnsDissolutionCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId, false);
                }
                break;
            case EnsureKind.GaoJiJieSuan: //高级结算退出房间.
                Debug.Log("Unity:"+"Eixt the room!");
                DestroyThis();
                MainRoot._gMainRoot.ChangeScene("GameHall");
                break;
            case EnsureKind.ShareZhanJi:
                Debug.Log("Unity:"+"Share to friends!");
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pShareZhanJiCtrl.ShareToWeiXinFriend();
                break;
            case EnsureKind.ShopPanelDlg:
                Debug.Log("Unity:"+"Show coin shop panel!");
                if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel.ShowShopPanel(ShopPanelCtrl.ShopPanelEnum.COIN_PANEL);
                }
                break;
            case EnsureKind.ShopCoinDlg:
                Debug.Log("Unity:"+"Select to close ShopCoinDlg!");
                MainRoot._gUIModule.pUnModalUIControl.SetActiveIsGotoQinRenBiSaiGame(false);
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null
                    && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ShaanxiMahjong")
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                    MainRoot._gMainRoot.ChangeScene("GameHall");//确认托管后退出金币房间
                }
                DestroyThis();
                break;
            case EnsureKind.ShopCardDlg:
                Debug.Log("Unity:"+"Select to close ShopCardDlg!");
                DestroyThis();
                break;
            case EnsureKind.ShopDiamondCoinDlg:
                {
                    Debug.Log("Unity:" + "click ShopDiamondCoinDlg bt1");
                    MainRoot._gUIModule.pUnModalUIControl.SetActiveIsGotoQinRenBiSaiGame(false);
                    DestroyThis();
                    break;
                }
            case EnsureKind.ShopDiamondCardDlg:
                {
                    Debug.Log("Unity:" + "click ShopDiamondCardDlg bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiToCoin:
                {
                    Debug.Log("Unity:" + "click ZuanShiToCoin bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiToCard:
                {
                    Debug.Log("Unity:" + "click ZuanShiToCard bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.RoomCradOverDlg:
                Debug.Log("Unity:"+"Select to close RoomCradOverDlg!");
                DestroyThis();
                break;
            case EnsureKind.PaiJuWeiWanChengDlg: //牌局未完成取消按键.
                Debug.Log("Unity:"+"PaiJuWeiWanChengDlg -> QuXiaoBt");
                DestroyThis();
                break;
            case EnsureKind.FriendRoomQueDingTuiChuDlg: //确定退出的取消按键.
                Debug.Log("Unity:"+"FriendRoomQueDingTuiChuDlg -> QuXiaoBt");
                DestroyThis();
                break;
            case EnsureKind.WeiZhaoDaoDuiShouDlg: //未找到对手确定按键.
                Debug.Log("Unity:"+"WeiZhaoDaoDuiShouDlg -> QueDingBt!");
                DestroyThis();
                break;
            case EnsureKind.WeiXinLoginDlg:
                Debug.Log("Unity:" + "click weixin loginBt!");

				if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl.DuiGouObj.activeInHierarchy)
                {
					if (MainRoot._gWeChat_Module.IsWeChatLoginValid_iOS())
					{
						this.Invoke("", 500.0f);
						MainRoot._gWeChat_Module.GetWeChatUserInfo();   //获取微信用户信息
					}
					else
					{
						Debug.Log("Unity:" + "IsWeChatLoginValid_iOS false");
					}
				}
                else
                {
                    Debug.Log("Unity:" + "need to select duiGou!");
                    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMXieYiAgree();
                    }
                }
                break;
            case EnsureKind.QiTaDengLuDlg:
                Debug.Log("Unity: click QiTaDengLu bt!");
                Application.Quit();
                break;
            case EnsureKind.PlayerZhanJiPanelDlg:
                {
                    Debug.Log("Unity: click PlayerZhanJiPanelDlg CoinZhanJi bt!"); //点击金币房(秦人麻将馆)战绩.
                    MainRoot._gUIModule.pUnModalUIControl.pPlayerZhanJiPanelCtrl.ShowPlayerZhanJiPanel(PlayerZhanJiPanelCtrl.ZhanJiPanelEnum.COIN_PANEL);
                    break;
                }
            case EnsureKind.GameSetPanelDlg:
                {
                    Debug.Log("Unity: click close gameSetPanel bt!");
                    MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong05);
                    DestroyThis();
                    break;
                }
            case EnsureKind.GameTuiGuangDlg:
                {
                    Debug.Log("Unity: click close GameTuiGuangDlg bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.GameHelpDlg:
                {
                    Debug.Log("Unity: click close GameHelpDlg bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.GameYinDaoDlg:
                {
                    Debug.Log("Unity: click close GameYinDaoDlg bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.GameFenXiangDlg:
                {
                    Debug.Log("Unity: click close GameFenXiangDlg bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.GameMsgDlg:
                {
                    Debug.Log("Unity: click close GameMsgDlg bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.JiaRuPaiJuDlg:
                {
                    Debug.Log("Unity: click close JiaRuPaiJuDlg bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.BanBenGengXin:
                {
                    Application.Quit();
                    break;
                }
			case EnsureKind.UserXieYi:
				{
					DestroyThis();
					break;
				}
			case EnsureKind.WLYC_CXDL:
				{
					DestroyThis();
					Application.Quit();
					break;
				}
			case EnsureKind.Game_WEIHU_Dlg:
				{
					DestroyThis();
					Application.Quit();
					break;
				}
			case EnsureKind.PromotionDlg:
				{
					DestroyThis();
					break;
				}
			case EnsureKind.GiftsDlg:
				{
					DestroyThis();
					break;
				}
			case EnsureKind.GuangBoSendDlg:
				{
					if (p_ShowTextA!=null)
					{
						if (p_ShowTextA.text.Length==0)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(55, this);//不能发送空信息
							return;
						}
						if (MainRoot._gPlayerData.PlayerCoin-5000<0)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(54, this);//不能发送空信息
							return;
						}
						if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
						{
							RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerSendGlobalChatMessage(MainRoot._gPlayerData.sUserName+"说:"+ p_ShowTextA.text);
							DestroyThis();
						}
					}
					break;
				}
			case EnsureKind.XiaZhuDlg:
				{
					int ncount = 0;
					if (p_ShowTextC.text.Length==0)
					{
						return;
					}
					try
					{
						ncount = Convert.ToInt32(p_ShowTextC.text);
						if (ncount<0)
						{
							ncount = 0;
						}
					}
					catch (Exception)
					{
					}
					if (ncount < 0)
					{
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(57, this);//您输入的金额有误，请重新输入！
						p_ShowTextC.text = "";
						return;
					}
					if (ncount>2000000000 ||(ncount!=0 && ncount > MainRoot._gPlayerData.PlayerCoin - 100000))
					{
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(56, this);//您的金币不足
						return;
					}

					if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
					{
						RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerStake_TwoHuman(ncount);
						DestroyThis();
					}
					break;
				}
            case EnsureKind.YongHuDengLu: //登录
                {
					//Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
					if (SystemSetManage.AuditVersion.IsCommonRunning)
                    {
                        return;
                    }
                    if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
                    {
                        if (p_input1 != null && p_input2 != null)
                        {
                            if (p_input1.text.Length < 6)
                            {

                            }
                            if (p_input2.text.Length < 6)
                            {

                            }
                            if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {
								//登录
								RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLogin_Audit(p_input1.text, p_input2.text);
                            }


                        }
                        Debug.Log("Unity:" + "此处去服务器登录");
                    }
                    break;
                }
            case EnsureKind.YongHuZhuCe://注册
				{
					if (SystemSetManage.AuditVersion.IsCommonRunning)
					{
						return;
					}
					if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
					{
						if (p_input1 != null  && p_input3 != null && p_input4 != null)
						{
							if (p_input1.text.Length < 6)
							{
								MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(60, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript); //账号长度不足，请重新输入
								p_input1.text = "";
								return;
							}

							if (p_input3.text.Length < 6)
							{
								MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(61, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript); //密码长度不足，请重新输入
								return;
							}
							if ( p_input3.text!= p_input4.text)
							{
								MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(62, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript); //密码长度不足，请重新输入
								return;
							}

							if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {
                                //注册
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerRegister_Audit(p_input1.text, p_input3.text);
                            }
                            

                        }
						Debug.Log("Unity:"+"此处去服务器注册");
					}
					break;
				}
            case EnsureKind.HaiXuanRuKouDlg:
                {
                    Debug.Log("Unity: click HaiXuanRuKouDlg bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.OnClickXiangQingBt();
                    break;
                }
            case EnsureKind.ShouJiHaoYanZheng:
                {
                    Debug.Log("Unity: click ShouJiHaoYanZheng bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShouJiHaoYanZhengDlg.OnClickGetYanZhengMaBt();
                    break;
                }
            case EnsureKind.SheQuSaiDaiMaYanZheng:
                {
                    Debug.Log("Unity: click SheQuSaiDaiMaYanZheng bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pSheQuSaiYanZhengMaDlg.OnClickQueRenBt();
                    break;
                }
            case EnsureKind.HaiXuanRuChangQuan:
                {
                    Debug.Log("Unity: click HaiXuanRuChangQuan bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.pHaiXuanRuChangQuanDlg.OnClickGouMaiRuChangQuanBt();
                    break;
                }
            case EnsureKind.TaoTaiSaiRuKou:
                {
                    Debug.Log("Unity: click TaoTaiSaiRuKou bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg.OnClickXiangQingBt();
                    break;
                }
            case EnsureKind.ZongJueSaiRuKou:
                {
                    Debug.Log("Unity: click ZongJueSaiRuKou bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg.OnClickXiangQingBt();
                    break;
                }
            case EnsureKind.MyRoomRuKou:
                {
                    Debug.Log("Unity: click MyRoomRuKou bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pMyRoomRuKouDlg.OnClickPlayGame();
                    break;
                }
            case EnsureKind.MyRoomPlayerPanel:
                {
                    Debug.Log("Unity: click MyRoomPlayerPanel bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.BiSaiQianEixt:
                {
                    Debug.Log("Unity: click BiSaiQianEixt bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.BiSaiKaBuZu:
                {
                    Debug.Log("Unity: click BiSaiKaBuZu bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.CARD_PANEL, MainRoot._gUIModule.pMainCanvas.transform);
                    DestroyThis();
                    break;
                }
            case EnsureKind.HuaShangDaZhuanPan:
                {
                    Debug.Log("Unity: click HuaShangDaZhuanPan bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pHuaShangDaZhuanPan.StartYunSuRunPoint();
                    break;
                }
			case EnsureKind.ThemeRaceXiangQing:
				{
					DestroyThis();
					break;
				}
            case EnsureKind.QRGongZhongHao:
                {
                    Debug.Log("Unity: click QRGongZhongHao bt1");
                    //点击确定按键,拉起微信公众号或向服务端发送消息.
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiBuZuMaiCoin:
                {
                    Debug.Log("Unity: click ZuanShiBuZuMaiCoin bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiBuZuMaiCard:
                {
                    Debug.Log("Unity: click ZuanShiBuZuMaiCard bt1");
                    DestroyThis();
                    break;
                }
            case EnsureKind.QinRenBiSaiPaiHang:
                {
                    Debug.Log("Unity: click QinRenBiSaiPaiHang bt1");
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pQinRenPaiHangDlg.ShowShopPanel(QinRenBiSaiPaiHangDlg.BiSaiEnum.PaiHang_PANEL);
                    break;
                }
            default:
                break;
        }
    }
   	/// <summary>
   	/// 标准确认界面的第2个按钮点击响应
   	/// </summary>
   	void OnBtn_2_Click()
    {
		if (isbtnlock)
		{
			return;
		}
		LockTimer();
		Invoke("UnlockTimer", locktime);
		switch ((EnsureKind)nEnsurekind)
        {
            case EnsureKind.JinBiChaoChu://金币超出确定
                MainRoot._gUIModule.pUnModalUIControl.OnPlayerClickJiBiChaoChuBt();
                DestroyThis();
                break;
            case EnsureKind.QuXiaoPiPei://取消匹配确定
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                    DestroyThis();
                    MainRoot._gMainRoot.ChangeScene("GameHall");//确认托管后退出金币房间
                }
                break;
            case EnsureKind.KaiShiQian_TuiChu_QinRenBiSai:
                {
                    Debug.Log("Unity: click KaiShiQian_TuiChu_QinRenBiSai bt2");
                    if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    {
                        //RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                        MainRoot._gPlayerData.IsOpenQinRenBiSaiRuKouDlg = true;
                        DestroyThis();
                        MainRoot._gMainRoot.ChangeScene("GameHall");//确认退出秦人比赛房间(开始前)
                    }
                    break;
                }
            case EnsureKind.YouXiZhong_TuiChu_QinRenBiSai:
                {
                    Debug.Log("Unity: click YouXiZhong_TuiChu_QinRenBiSai bt2");
                    if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    {
                        //RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                        DestroyThis();
                        MainRoot._gMainRoot.ChangeScene("GameHall");//确认退出秦人比赛房间(游戏中)
                    }
                    break;
                }
            case EnsureKind.PiPeiShiBai://匹配失败重试
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SMZhengZaiPiPei != null)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SMZhengZaiPiPei.showtime = 30f;
                }
                DestroyThis();
                break;
            case EnsureKind.JinRuTuoGuan://进入托管确定
				if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
				{
					RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
					DestroyThis();
					MainRoot._gMainRoot.ChangeScene("GameHall");//确认托管后退出金币房间
				}
                break;
            case EnsureKind.JieSanPaiJu://想想是否解散好友房间.
                Debug.Log("Unity:"+"player thinking...");
                DestroyThis();
                break;
            case EnsureKind.JieSanTouPiao://同意解散好友房间.
                Debug.Log("Unity:"+"agree kill game!");
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom != null) {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom.HiddenDlgBt(JieSanFriendRoomCtrl.BtEnum.AgreeBt);
                }
                if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerAnsDissolutionCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId, true);
                }
                break;
            case EnsureKind.GaoJiJieSuan:
                Debug.Log("Unity:"+"Share the jieSuan panel!");
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pGaoJiJieSuanCtrl != null) {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pGaoJiJieSuanCtrl.CaptureUIScreenshotByPoint();
                }
                break;
            case EnsureKind.ShareZhanJi:
                Debug.Log("Unity:"+"Share to pengYouQuan!");
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pShareZhanJiCtrl.ShareToWeiXinFriendsQuan();
                break;
            case EnsureKind.ShopPanelDlg:
                Debug.Log("Unity:"+"Show card shop panel!");
                if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel.ShowShopPanel(ShopPanelCtrl.ShopPanelEnum.CARD_PANEL);
                }
                break;
            case EnsureKind.ShopCoinDlg:
                if (MainRoot._gPlayerData == null)
                {
                    Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                    return;
                }
                if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
                {
                    Debug.Log("Unity:" + "Please login!");
                    return;
                }
                if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
                {
                    Debug.Log("Unity: player is not login!");
                    return;
                }
                Debug.Log("Unity:"+"Select to buy game coin!");
                //ShopCoinDlgCtrl shopCoinDlg = GetComponent<ShopCoinDlgCtrl>();
                //int payGood = shopCoinDlg.GetPayGoodIndex();
                //这之间得插入一张支付选择渠道确认界面
                MainRoot._gUIModule.pUnModalUIControl.ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.COIN120000);
                DestroyThis();
                break;
            case EnsureKind.ShopCardDlg:
                if (MainRoot._gPlayerData == null)
                {
                    Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                    return;
                }
                if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
                {
                    Debug.Log("Unity:" + "Please login!");
                    return;
                }
                if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
                {
                    Debug.Log("Unity: player is not login!");
                    return;
                }
                Debug.Log("Unity:"+"Select to buy game card!");
                //这之间得插入一张支付选择渠道确认界面
                MainRoot._gUIModule.pUnModalUIControl.ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.CARD5);
                DestroyThis();
                break;
            case EnsureKind.ShopDiamondCoinDlg:
                {
                    if (MainRoot._gPlayerData == null)
                    {
                        Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                        return;
                    }
                    if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
                    {
                        Debug.Log("Unity:" + "Please login!");
                        return;
                    }
                    if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
                    {
                        Debug.Log("Unity: player is not login!");
                        return;
                    }
                    Debug.Log("Unity:" + "click ShopDiamondCoinDlg bt2");
                    //ShopDiamondCoinDlgCtrl shopDlg = GetComponent<ShopDiamondCoinDlgCtrl>();
                    //int payGood = shopDlg.GetPayGoodIndex();
                    //这里添加充钻石购买金币的服务端消息.
                    MainRoot._gUIModule.pUnModalUIControl.ShopDiamondState = EnsureKind.ShopDiamondCoinDlg;
                    MainRoot._gUIModule.pUnModalUIControl.ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.JEWEL60);
                    DestroyThis();
                    break;
                }
            case EnsureKind.ShopDiamondCardDlg:
                {
                    if (MainRoot._gPlayerData == null)
                    {
                        Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                        return;
                    }
                    if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
                    {
                        Debug.Log("Unity:" + "Please login!");
                        return;
                    }
                    if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
                    {
                        Debug.Log("Unity: player is not login!");
                        return;
                    }
                    Debug.Log("Unity:" + "click ShopDiamondCardDlg bt2");
                    //这里添加充钻石购买房卡的服务端消息.
                    MainRoot._gUIModule.pUnModalUIControl.ShopDiamondState = EnsureKind.ShopDiamondCardDlg;
                    MainRoot._gUIModule.pUnModalUIControl.ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.JEWEL60);
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiToCoin:
                {
                    if (MainRoot._gPlayerData == null)
                    {
                        Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                        return;
                    }
                    if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
                    {
                        Debug.Log("Unity:" + "Please login!");
                        return;
                    }
                    if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
                    {
                        Debug.Log("Unity: player is not login!");
                        return;
                    }
                    Debug.Log("Unity:" + "click ZuanShiToCoin bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel.ShopSendMsgToServer();
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiToCard:
                {
                    if (MainRoot._gPlayerData == null)
                    {
                        Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
                        return;
                    }
                    if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
                    {
                        Debug.Log("Unity:" + "Please login!");
                        return;
                    }
                    if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
                    {
                        Debug.Log("Unity: player is not login!");
                        return;
                    }
                    Debug.Log("Unity:" + "click ZuanShiToCard bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel.ShopSendMsgToServer();
                    DestroyThis();
                    break;
                }
            case EnsureKind.PaiJuWeiWanChengDlg: //牌局未完成确定按键.
                Debug.Log("Unity:"+"PaiJuWeiWanChengDlg -> QueDingBt, make player continue playing!");
                if (MainRoot._gPlayerData.relinkUserRoomData != null)
                {//说明玩家有未完成的牌局
                 
                    if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    {//点击请求重新连入牌局
                        RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentRoomInfo((int)MainRoot._gPlayerData.relinkUserRoomData.roomType, MainRoot._gPlayerData.relinkUserRoomData.roomID);
                    }
                }
                DestroyThis();
                break;
            case EnsureKind.FriendRoomQueDingTuiChuDlg: //确定退出的确定按键.
                Debug.Log("Unity:"+"FriendRoomQueDingTuiChuDlg -> QueDingBt");
				if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
				{//未开始前点击退出房卡房间
					RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerQuitCardRoom( MainRoot._gRoomData.cCurRoomData.nRoomId);
				}
                MainRoot._gMainRoot.ChangeScene("GameHall");
				DestroyThis();
                break;
            case EnsureKind.WeiZhaoDaoDuiShouDlg: //未找到对手重试按键.
                Debug.Log("Unity:"+"WeiZhaoDaoDuiShouDlg -> retryBt!");
                DestroyThis();
                break;
            case EnsureKind.WeiXinLoginDlg:
                Debug.Log("Unity:" + "show weixin panel duigou!");
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl.ShowDuiGou();
                break;
            case EnsureKind.PlayerZhanJiPanelDlg:
                {
                    Debug.Log("Unity: click PlayerZhanJiPanelDlg CardZhanJi bt!"); //点击房卡房战绩.
                    MainRoot._gUIModule.pUnModalUIControl.pPlayerZhanJiPanelCtrl.ShowPlayerZhanJiPanel(PlayerZhanJiPanelCtrl.ZhanJiPanelEnum.CARD_PANEL);
                    break;
                }
            case EnsureKind.GameFenXiangDlg:
                {
                    Debug.Log("Unity: click fenXiangHaoYou bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.BanBenGengXin:
                {
					if (sTempVer!="")
					{
                        ChackVersionManage.DownloadApk(sTempVer);
                       //Application.OpenURL(sTempVer);
					}
					//Application.Quit();
                    break;
                }
			case EnsureKind.PaiJuBuZaiDlg:
				{
					DestroyThis();
					break;
				}
			case EnsureKind.PromotionDlg:	//首充活动-立即充值
				{
					Debug.LogError("薛冰看这里！首充活动-立即充值");
                    //这之间得插入一张支付选择渠道确认界面
                    MainRoot._gUIModule.pUnModalUIControl.ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.FIRSTCHARGE);
                    break;
				}
			case EnsureKind.GiftsDlg:   //车展礼包-领取
				{
					Debug.LogError("薛冰看这里！车展礼包-领取");
					if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
					{//请求牌局玩家及桌面信息
						RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerGetCarGifts();
						DestroyThis();
					}
					break;
				}
			case EnsureKind.GuangBoSendDlg:
				{
					DestroyThis();
					break;
				}
			case EnsureKind.YongHuDengLu://登录
				{
					if (SystemSetManage.AuditVersion.IsCommonRunning)
					{
						return;
					}
					if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
					{
						MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.InitialAuditReg();
						DestroyThis();
						Debug.Log("Unity:" + "此处打开注册界面");
					}
					break;
				}
			case EnsureKind.YongHuZhuCe://登录
				{
					if (SystemSetManage.AuditVersion.IsCommonRunning)
					{
						return;
					}
					if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
					{
						MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.InitialAuditLogin();
						DestroyThis();
						Debug.Log("Unity:" + "此处打开注册界面");
					}
					break;
                }
            case EnsureKind.HaiXuanRuKouDlg:
                {
                    Debug.Log("Unity: click HaiXuanRuKouDlg bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.OnClickStartBiSaiBt();
                    break;
                }
            case EnsureKind.ShouJiHaoYanZheng:
                {
                    Debug.Log("Unity: click ShouJiHaoYanZheng bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShouJiHaoYanZhengDlg.OnClickQueRenBt();
                    break;
                }
            case EnsureKind.SheQuSaiDaiMaYanZheng:
                {
                    Debug.Log("Unity: click SheQuSaiDaiMaYanZheng bt2");
                    DestroyThis();
                    break;
                }
            case EnsureKind.HaiXuanRuChangQuan:
                {
                    Debug.Log("Unity: click HaiXuanRuChangQuan bt2");
                    DestroyThis();
                    break;
                }
            case EnsureKind.TaoTaiSaiRuKou:
                {
                    Debug.Log("Unity: click TaoTaiSaiRuKou bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg.OnClickStartBiSaiBt();
                    break;
                }
            case EnsureKind.ZongJueSaiRuKou:
                {
                    Debug.Log("Unity: click ZongJueSaiRuKou bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg.OnClickStartBiSaiBt();
                    break;
                }
            case EnsureKind.MyRoomRuKou:
                {
                    Debug.Log("Unity: click MyRoomRuKou bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pMyRoomRuKouDlg.OnClickCreatMyRoom();
                    break;
                }
            case EnsureKind.BiSaiQianEixt:
                {
                    Debug.Log("Unity: click BiSaiQianEixt bt2");
                    //这里添加退出自建比赛房间的网络消息.
                    //if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    //{   //未开始前点击退出自建比赛房间.
                    //    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerQuitCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId);
                    //}
                    MainRoot._gMainRoot.ChangeScene("GameHall");
                    break;
                }
            case EnsureKind.HuaShangDaZhuanPan:
                {
                    Debug.Log("Unity: click HuaShangDaZhuanPan bt2");
                    DestroyThis();
                    break;
                }
            case EnsureKind.QRGongZhongHao:
                {
                    Debug.Log("Unity: click QRGongZhongHao bt2");
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiBuZuMaiCoin:
                {
                    Debug.Log("Unity: click ZuanShiBuZuMaiCoin bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.DIAMOND_PANEL, MainRoot._gUIModule.pMainCanvas.transform);
                    DestroyThis();
                    break;
                }
            case EnsureKind.ZuanShiBuZuMaiCard:
                {
                    Debug.Log("Unity: click ZuanShiBuZuMaiCard bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.DIAMOND_PANEL, MainRoot._gUIModule.pMainCanvas.transform);
                    DestroyThis();
                    break;
                }
            case EnsureKind.QinRenBiSaiPaiHang:
                {
                    Debug.Log("Unity: click QinRenBiSaiPaiHang bt2");
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pQinRenPaiHangDlg.ShowShopPanel(QinRenBiSaiPaiHangDlg.BiSaiEnum.JiangLi_PANEL);
                    break;
                }
            default:
                break;
        }
    }
    /// <summary>
    /// 标准确认界面的第3个按钮点击响应
    /// </summary>
    void OnBtn_3_Click()
    {
		if (isbtnlock)
		{
			return;
		}
		LockTimer();
		Invoke("UnlockTimer", locktime);
		switch ((EnsureKind)nEnsurekind)
        {
            case EnsureKind.GaoJiJieSuan:
                Debug.Log("Unity:"+"Close the jieSuan panel!");
                DestroyThis();
                break;
            case EnsureKind.ShareZhanJi:
                Debug.Log("Unity:"+"Eixt share panel!");
                if (MainRoot._gUIModule != null) {
                    MainRoot._gUIModule.DestroyScreenshotZhanJiTexture();
                }
                DestroyThis();
                break;
            case EnsureKind.ShopPanelDlg:
                Debug.Log("Unity:"+"Eixt shop panel!");
                DestroyThis();
                break;
            case EnsureKind.WeiXinLoginDlg:
                Debug.Log("Unity:" + "Eixt weixin login panel!");
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.OnSysMsgWaitEventHappen(40); //关闭正在链接的消息.
                DestroyThis();
                break;
            case EnsureKind.PlayerZhanJiPanelDlg:
                {
                    Debug.Log("Unity: click PlayerZhanJiPanelDlg CloseZhanJi bt!"); //点击房卡房战绩.
                    DestroyThis();
                    break;
                }
            case EnsureKind.GameFenXiangDlg:
                {
                    Debug.Log("Unity: click fenXiangPengYouQuan bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.GamePaiHang:
                {
                    Debug.Log("Unity: click GameLeaderboard close bt!");
                    DestroyThis();
                    break;
                }
            case EnsureKind.HaiXuanRuKouDlg:
                {
                    Debug.Log("Unity: click HaiXuanRuKouDlg bt3");
                    DestroyThis();
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.CheckIsTiaoGuoThisPanel(true);
                    break;
                }
            case EnsureKind.QinRenBiSaiChangDlg:
                {
                    Debug.Log("Unity: click QinRenBiSaiChangDlg bt3");
                    DestroyThis();
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.CheckIsTiaoGuoThisPanel(true);
                    break;
                }
            case EnsureKind.ShouJiHaoYanZheng:
                {
                    Debug.Log("Unity: click ShouJiHaoYanZheng bt3");
                    DestroyThis();
                    break;
                }
            case EnsureKind.TaoTaiSaiRuKou:
                {
                    Debug.Log("Unity: click TaoTaiSaiRuKou bt3");
                    DestroyThis();
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.CheckIsTiaoGuoThisPanel(true);
                    break;
                }
            case EnsureKind.ZongJueSaiRuKou:
                {
                    Debug.Log("Unity: click ZongJueSaiRuKou bt3");
                    DestroyThis();
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.CheckIsTiaoGuoThisPanel(true);
                    break;
                }
            case EnsureKind.MyRoomRuKou:
                {
                    Debug.Log("Unity: click MyRoomRuKou bt3");
                    DestroyThis();
                    break;
                }
            case EnsureKind.QinRenBiSaiPaiHang:
                {
                    Debug.Log("Unity: click QinRenBiSaiPaiHang bt3");
                    DestroyThis();
                    break;
                }
            default:
                break;
        }
    }
	/// <summary>
	/// 标准确认界面的第四个按钮点击响应 
	/// </summary>
	void OnBtn_4_Click()
	{
		if (isbtnlock)
		{
			return;
		}
		LockTimer();
		Invoke("UnlockTimer", locktime);
		switch ((EnsureKind)(nEnsurekind))
		{
			case EnsureKind.FuLiJin01:
				break;
			case EnsureKind.FuLiJin02:
				break;
			case EnsureKind.JinBiChaoChu:
				break;
			case EnsureKind.QuXiaoPiPei:
				break;
			case EnsureKind.PiPeiShiBai:
				break;
			case EnsureKind.JinRuTuoGuan:
				break;
			case EnsureKind.JieSanPaiJu:
				break;
			case EnsureKind.JieSanTouPiao:
				break;
			case EnsureKind.GaoJiJieSuan:
				break;
			case EnsureKind.ShareZhanJi:
				break;
			case EnsureKind.ShopPanelDlg:
                {
                    Debug.Log("Unity:" + "Show diamand shop panel!");
                    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel.ShowShopPanel(ShopPanelCtrl.ShopPanelEnum.DIAMOND_PANEL);
                    }
                    break;
                }
			case EnsureKind.ShopCoinDlg:
				break;
			case EnsureKind.ShopCardDlg:
				break;
			case EnsureKind.RoomCradOverDlg:
				break;
			case EnsureKind.PaiJuWeiWanChengDlg:
				break;
			case EnsureKind.FriendRoomQueDingTuiChuDlg:
				break;
			case EnsureKind.WeiZhaoDaoDuiShouDlg:
				break;
			case EnsureKind.WeiXinLoginDlg:
				GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-UserXieYi"), transform.parent.transform, false);
				EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
				dlg.Initial(EnsureDlg.EnsureKind.UserXieYi);
				break;
			case EnsureKind.QiTaDengLuDlg:
				break;
			case EnsureKind.PlayerZhanJiPanelDlg:
				break;
			case EnsureKind.GameSetPanelDlg:
				break;
			case EnsureKind.GameTuiGuangDlg:
				break;
			case EnsureKind.GameHelpDlg:
				break;
			case EnsureKind.GameYinDaoDlg:
				break;
			case EnsureKind.GameFenXiangDlg:
				break;
			case EnsureKind.GameMsgDlg:
				break;
			case EnsureKind.JiaRuPaiJuDlg:
				break;
			case EnsureKind.BanBenGengXin:
				break;
			default:
				break;
		}
	}
	public void OnGlobalChatInputValueChanged(InputFieldBase pInput)
	{
		p_ShowTextB.text = (25 - (pInput.text.Length)).ToString();
	}
	public void OnXiaZhuInputValueChanged()
	{
		Debug.Log("OnXiaZhuInputValueChanged:"+ p_ShowTextC.text);
		string temp = p_ShowTextC.text;// = (25 - (p_ShowTextA.text.Length) / 2).ToString();
		temp = p_ShowTextC.text.Replace("-","");
		p_ShowTextC.text = temp;
	}
}
