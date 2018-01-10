using UnityEngine;
using System.Collections;
using RoomCardNet;

/// <summary>
/// 所有UI界面的根节点，父对象容器
/// </summary>
class UIModule : MonoBehaviour {
    // Use this for initialization
    /// <summary>
    /// 模态界面管理器
    /// </summary>
    public ModalUIControl pModalUIControl;
	/// <summary>
	/// 非模态界面管理器
	/// </summary>
    public UnModalUIControl pUnModalUIControl;
	/// <summary>
	/// UI主摄像机
	/// </summary>
    public Camera pUICamera;
	/// <summary>
	/// UI画布
	/// </summary>
    public Canvas pMainCanvas;
    public int nCurSceneType;
    public void InitialUIModule(int nSceneType)
    {
        nCurSceneType = nSceneType;
        //分辨率配置
        //SystemSetManage.ResolutionSet(pMainCanvas);
        //GameObject.DontDestroyOnLoad(gameObject);
    }
	/// <summary>
	/// 游戏房间内的消息都会转到这里执行
	/// </summary>
	/// <param name="nFunId"></param>
	/// <param name="args"></param>
	public void OnNetMessageEnterMainUI(uint nFunId, params object[] args)   
    {
        if (args.Length == 0)
        {
            return;
        }
        switch (nFunId)
        {
            case 0x55CB9860://C_AnsPlayerLoginReturn CRC32HashCode  //玩家登陆后面
                MainRoot._gMainRoot.IntialPlayerData(args);
                break;
			case 0xB1A6BBC5://C_AnsCreateCardRoomFailed CRC32HashCode //返回创建房卡失败的消息
				if ((int)args[3] == 0)  //房卡不足，弹出购买界面
				{
					if (pUnModalUIControl.pMainUIScript != null)
					{
						pUnModalUIControl.pMainUIScript.SpawnShopCardDlg();
					}
				}
				if ((int)args[3] == 1)  //创建房间数已满，不能继续创建
				{
					if (pUnModalUIControl.pMainUIScript != null)
					{
						pUnModalUIControl.pMainUIScript.SpawnRoomCradOverDlg();
					}
				}
				break;
			case 0x5D5E830E://C_AnsJoinCardRoomFailed CRC32HashCode 加入房卡房间失败
                switch ((int)(args[1]))
                {
                    case 0:
                        Debug.Log("Unity:"+"not find the room!");
						GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/RoomCardUI/ED-PaiJuBuZaiDlg"), MainRoot._gUIModule.pMainCanvas.transform, false);
						EnsureDlg temp = test.GetComponent<EnsureDlg>();
						temp.Initial(EnsureDlg.EnsureKind.PaiJuBuZaiDlg);
						break;
                    case 1:
                        Debug.Log("Unity:"+"player has over!"); //人数已满.
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.SpawnPaiJuPlayingDlg();
                        break;
                }
				break;
			case 0xA90A82DE://C_AnsOneselfJoinCardRoom CRC32HashCode //自己加入房卡房间
                            //存数据，切场景，加载主UI，显示自己
                {
                    //MainRoot._gPlayerData.nFangKaCount -= 1;
                    MainRoot._gRoomData.cCacheRoomData.nRoomId = (int)args[1];
                    MainRoot._gRoomData.cCacheRoomData.eRoomState = OneRoomData.RoomStat.WAIT_START;
                    MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_RoomCard;
                    MainRoot._gRoomData.cCacheRoomData.nCurRound = 0;
                    MainRoot._gRoomData.cCacheRoomData.nRoomOwnerId = (int)args[4];
                    MainRoot._gRoomData.cCacheRoomData.sRoomOwnerName = (string)args[5];
                    MainRoot._gRoomData.cCacheRoomData.sRoomSetting = (string)args[6];
                    Debug.Log("Unity:"+"0xA90A82DE 房卡房间创建成功，自己加入房卡房间:" + MainRoot._gRoomData.cCacheRoomData.ToString());
                    
                    //分解房间设置信息
                    string[] vset = MainRoot._gRoomData.cCacheRoomData.sRoomSetting.Split(":".ToCharArray());
                    MainRoot._gRoomData.cCacheRoomData.vRoomSetting = new int[vset.Length];
                    for (int i = 1; i < vset.Length; i++)
                    {
                        MainRoot._gRoomData.cCacheRoomData.vRoomSetting[i - 1] = System.Convert.ToInt32(vset[i]);
                    }
					MainRoot._gRoomData.cCacheRoomData.nMaxRound = MainRoot._gRoomData.cCacheRoomData.vRoomSetting[0]==0?8:16;//最大局数

					MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[1] = (int)args[1];//玩家id
					for (int index = 0; index < 4; index++)
					{
						MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[2 + index*5] = (object)args[7 + index * 5];	//id
						MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[2 + index*5+1] = (object)args[7 + index * 5+1];//name
						MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[2 + index*5+2] = (object)args[7 + index * 5+2];//headurl
						MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[2 + index*5+3] = (object)args[7 + index * 5+3];//sex
						MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[2 + index * 5 + 4] = (object)args[7 + index * 5 + 4];//准备
                        if (args.Length>31)
                        {
                            MainRoot._gRoomData.cCacheRoomData.tempUserJoinInfo[25 + index] = (object)args[28 + index];//准备
                        }
                    }
					MainRoot._gRoomData.cCacheRoomData.bIsErRenMjRoom = (bool)args[27];
					if (MainRoot._gRoomData.cCacheRoomData.IsErRenCardRoom())
					{
						MainRoot._gRoomData.cCacheRoomData.nMaxRound = 1;//最大局数
					}
					MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");

					break;
                }
			case 0xC9D8E039://C_CallPlayerJiuJiOk CRC32HashCode
                {
                    //修改玩家数据，请修改界面
                    if ((int)args[1] == MainRoot._gPlayerData.nUserId)
                    {
                        MainRoot._gPlayerData.PlayerCoin+=(int)args[2];
                        MainRoot._gPlayerData.UpdatePlayerDtInfo();
                        if (args.Length > 4)
                        {
                            MainRoot._gPlayerData.SetPlayerJiuJiFlag((int)args[3], (System.DateTime)args[4]);
                        }
                    }
                    break;
                }
            case 0xC2C24BF2://C_CallGetPlayerCardRoomList CRC32HashCode //取玩家历史列表信息
                {
                    //玩家房间列表显示
                    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowRoomCardJiaRuPanel(args);
                    }
                    break;
                }
            case 0x12F1E88://C_AnsPlayerStrandGame CRC32HashCode  //玩家登录 滞留游戏信息
                {
                    if (MainRoot._gPlayerData)
                    {
                        MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[2], (int)args[3], (int)args[4]);
                    }
                    break;
                }
            case 0x4A372289://C_AnsPlayerCurrentRoomInfo CRC32HashCode    //发送当前牌局信息
                {//请求房间信息返回成功，请处理界面显示

                    //args[2]-房间id
                    //args[3]-房间类型
                    if ((int)args[3] == (int)OneRoomData.RoomType.RoomType_RoomCard)
                    {
                        //args[4]-牌局的名字
                        //args[5]-创建者的ID
                        //args[6]-创建者的名字
                        //args[7]-房间的设置
                        MainRoot._gRoomData.cCacheRoomData.nRoomId = (int)args[2];
                        //MainRoot._gRoomData.cCacheRoomData.eRoomState = OneRoomData.RoomStat.WAIT_START;
                        MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_RoomCard;
                        MainRoot._gRoomData.cCacheRoomData.nCurRound = 0;
                        MainRoot._gRoomData.cCacheRoomData.nRoomOwnerId = (int)args[5];
                        MainRoot._gRoomData.cCacheRoomData.sRoomOwnerName = (string)args[6];
                        MainRoot._gRoomData.cCacheRoomData.sRoomSetting = (string)args[7];
                        MainRoot._gRoomData.cCacheRoomData.bIsErRenMjRoom = (bool)args[8];
                        //分解房间设置信息
                        string[] vset = MainRoot._gRoomData.cCacheRoomData.sRoomSetting.Split(":".ToCharArray());
                        MainRoot._gRoomData.cCacheRoomData.vRoomSetting = new int[vset.Length];
                        for (int i = 1; i < vset.Length; i++)
                        {
                            MainRoot._gRoomData.cCacheRoomData.vRoomSetting[i - 1] = System.Convert.ToInt32(vset[i]);
                        }
                        MainRoot._gRoomData.cCacheRoomData.nMaxRound = MainRoot._gRoomData.cCacheRoomData.IsErRenCardRoom() ? 1 : (MainRoot._gRoomData.cCacheRoomData.vRoomSetting[0] == 0 ? 8 : 16);//最大局数
                        MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");

                        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                        {//请求牌局玩家及桌面信息
                            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentTableData((int)args[3], (int)args[2]);
                        }
                    }
                    else if ((int)args[3] == (int)OneRoomData.RoomType.RoomType_Gold)
                    {
                        //args[4]-牌局的等级
                        MainRoot._gRoomData.cCacheRoomData.nRoomId = (int)args[2];
                        MainRoot._gRoomData.cCacheRoomData.nGoldRoomLevel = (int)args[4];
                        //MainRoot._gRoomData.cCacheRoomData.eRoomState = OneRoomData.RoomStat.WAIT_START;
                        MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_Gold;
                        MainRoot._gRoomData.cCacheRoomData.nCurRound = 0;
                        MainRoot._gRoomData.cCacheRoomData.nMaxRound = 0;
                        MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");

                        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                        {//请求牌局玩家及桌面信息
                            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentTableData((int)args[3], (int)args[2]);
                        }
                    }
                    else if ((int)args[3] == (int)OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan || (int)args[3] == (int)OneRoomData.RoomType.RoomType_MultRace_HaiXuan)
                    {
                        //args[4]-牌局的等级
                        MainRoot._gRoomData.cCacheRoomData.nRoomId = (int)args[2];
                        MainRoot._gRoomData.cCacheRoomData.nGoldRoomLevel = 1;
                        //MainRoot._gRoomData.cCacheRoomData.eRoomState = OneRoomData.RoomStat.WAIT_START;
                        MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan;
                        MainRoot._gRoomData.cCacheRoomData.nCurRound = 0;
                        MainRoot._gRoomData.cCacheRoomData.nMaxRound = 0;
                        MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");

                        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                        {//请求牌局玩家及桌面信息
                            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentTableData((int)args[3], (int)args[2]);
                        }
                    }
                    else if ((int)args[3] == (int)OneRoomData.RoomType.RoomType_ThemeRace_Group || (int)args[3] == (int)OneRoomData.RoomType.RoomType_MultRace_Group)
                    {
                        //args[4]-牌局的名字
                        //args[5]-创建者的ID
                        //args[6]-创建者的名字
                        //args[7]-房间的设置
                        MainRoot._gRoomData.cCacheRoomData.nRoomId = (int)args[2];
                        //MainRoot._gRoomData.cCacheRoomData.eRoomState = OneRoomData.RoomStat.WAIT_START;
                        MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_Group;
                        MainRoot._gRoomData.cCacheRoomData.nCurRound = 0;
                        MainRoot._gRoomData.cCacheRoomData.nRoomOwnerId = 1;
                        MainRoot._gRoomData.cCacheRoomData.sRoomOwnerName = "";
                        MainRoot._gRoomData.cCacheRoomData.sRoomSetting = "0";
                        //分解房间设置信息
                        string[] vset = MainRoot._gRoomData.cCacheRoomData.sRoomSetting.Split(":".ToCharArray());
                        MainRoot._gRoomData.cCacheRoomData.vRoomSetting = new int[vset.Length];
                        for (int i = 1; i < vset.Length; i++)
                        {
                            MainRoot._gRoomData.cCacheRoomData.vRoomSetting[i - 1] = System.Convert.ToInt32(vset[i]);
                        }
                        MainRoot._gRoomData.cCacheRoomData.nMaxRound = 8;//最大局数
                        MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");

                        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                        {//请求牌局玩家及桌面信息
                            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentTableData((int)args[3], (int)args[2]);
                        }
                    }
                    else
                    {
                        Debug.LogError("Unity:" + "不存在的房屋类型");
                    }
                    break;
                }
            case 0x4CEB16F9://C_AnsPlayerCurrentRunRoomInfo CRC32HashCode    ////发送当前有玩家进行的牌局信息
                {//有牌局，弹框进入
                    MainRoot._gPlayerData.relinkUserRoomData = null;
                    if (MainRoot._gPlayerData)
                    {
                        if ((int)args[4] == (int)OneRoomData.RoomType.RoomType_Gold)
                        {
                            MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], (int)args[6]);
                        }
                        else if((int)args[4] == (int)OneRoomData.RoomType.RoomType_RoomCard)
                        {
                            MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], 0);
                        }
                        else if ((int)args[4] == (int)OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan || (int)args[4] == (int)OneRoomData.RoomType.RoomType_MultRace_HaiXuan)
                        {
                            MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], 1);
                            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan; //强制转换海选赛的房间类型.
                            switch ((OneRoomData.RoomType)((int)args[4]))
                            {
                                case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                                    {
                                        MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace;
                                        break;
                                    }
                                case OneRoomData.RoomType.RoomType_MultRace_Group:
                                    {
                                        MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1;
                                        break;
                                    }
                            }
                        }
                        else if ((int)args[4] == (int)OneRoomData.RoomType.RoomType_ThemeRace_Group || (int)args[4] == (int)OneRoomData.RoomType.RoomType_MultRace_Group)
                        {
                            MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], 0);
                            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_Group; //强制转换淘汰赛的房间类型.
                            switch ((OneRoomData.RoomType)((int)args[4]))
                            {
                                case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                                    {
                                        MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace;
                                        break;
                                    }
                                case OneRoomData.RoomType.RoomType_MultRace_Group:
                                    {
                                        MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1;
                                        break;
                                    }
                            }
                        }
                    }
                    if (MainRoot._gPlayerData.relinkUserRoomData != null)
                    {//说明玩家有未完成的牌局
                        if (!(bool)args[2])
                        {//直接请求复盘
                            if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {//点击请求重新连入牌局
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentRoomInfo((int)MainRoot._gPlayerData.relinkUserRoomData.roomType, MainRoot._gPlayerData.relinkUserRoomData.roomID);
                            }
                        }
                        else
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnLoginPaiJuWeiWanChengDlg();//点击请求重新连入牌局
                        }
                    }
                    return;
                }
            case 0xC565E781://C_AnsUpdatePlayerCardNum CRC32HashCode //更新玩家的房卡.
                {
                    if (MainRoot._gPlayerData != null)
                    {
                        Debug.Log("Unity: updata player cardNum!");
                        MainRoot._gPlayerData.nFangKaCount = (int)args[3];
                        MainRoot._gPlayerData.UpdatePlayerDtInfo();
                    }
                    return;
                }
            case 0x29701AD6://C_AnsPlayerPayOKReturn CRC32HashCode //玩家充值成功后返回的消息
                {
                    //args[x]: [3] 类型, [4] 金币, [5] 房卡, [6] 钻石.
                    if (MainRoot._gPlayerData != null)
                    {
                        Debug.Log("Unity: updata player daoJuDt!");
                        int gemAddNum = (int)args[6];
                        MainRoot._gPlayerData.nCoinNum += (int)args[4];
                        MainRoot._gPlayerData.nFangKaCount += (int)args[5];
                        MainRoot._gPlayerData.GemCount += gemAddNum;
                        CommonLibrary.Product.Product_RetrunCode rv = (CommonLibrary.Product.Product_RetrunCode)args[7];
                        switch (rv)
                        {
                            case CommonLibrary.Product.Product_RetrunCode.RetrunCode_Normal:
                                {
                                    MainRoot._gPlayerData.UpdatePlayerDtInfo();
                                    if (gemAddNum == 60)
                                    {
                                        MainRoot._gUIModule.pUnModalUIControl.ShopDiamondToCoinCard();
                                    }
                                    break;
                                }
                            default:
                                {
                                    Debug.Log("Unity: update playerDaoJuDt wrong! rv is " + rv);
                                    break;
                                }
                        }
                    }
                    return;
                }
            case 0x44174EE5://C_CallPlayerRefreshCoin CRC32HashCode   //直接刷新金币适用未在游戏中
                {
                    //修改玩家金币数，请修改界面
                    if ((int)args[1] == MainRoot._gPlayerData.nUserId)
                    {
                        MainRoot._gPlayerData.PlayerCoin = (int)args[2];
                        MainRoot._gPlayerData.UpdatePlayerDtInfo();
                    }
                    return;
                }
            case 0x8826D045://C_AnsGetRecordListOk CRC32HashCode  //获取战绩历史列表信息
                {
                    MainRoot._gUIModule.pUnModalUIControl.CheckIsSpawnPlayerZhanJiPanel(args);
                    return;
                }
            case 0x712E3BE2://C_CallPlayerGetRecordTotal CRC32HashCode    //返回战绩总场次信息
                {
                    MainRoot._gUIModule.pUnModalUIControl.pPlayerZhanJiPanelCtrl.OnReceivedPlayerTimeZhanJi(args);
                    return;
                }
			case 0xB68573B://C_AnsPlayerCurrentRoomInfoFailed CRC32HashCode    //发送当前牌局信息 没有找到房间
				{
					//MainRoot._gMainRoot.ChangeScene("GameHall");//请求的复盘房间不存在
					if (MainRoot._gPlayerData.relinkUserRoomData != null)
					{
						MainRoot._gPlayerData.relinkUserRoomData = null; //清除当前牌局信息.
					}
					GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/RoomCardUI/ED-PaiJuBuZaiDlg"), MainRoot._gUIModule.pMainCanvas.transform, false);
					EnsureDlg temp = test.GetComponent<EnsureDlg>();
					temp.Initial(EnsureDlg.EnsureKind.PaiJuBuZaiDlg);
					return;
				}
			case 0x84511863://C_CallPlayerGoToMainUIView CRC32HashCode	//服务端强制用户返回主界面
				{
					MainRoot._gMainRoot.ChangeScene("GameHall");
					return;
				}
			case 0x817AD282://C_CallPlayerGoToOut CRC32HashCode		//服务端强制用户下线
				{
					RoomCardNeDevice.IsExceptionPlayer = true;
					RoomCardNetClientModule.netModule.ClosePlayerLinkClient();
					GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-WLYC-CXDL"), MainRoot._gUIModule.pMainCanvas.transform, false);
					EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
					dlg.Initial(EnsureDlg.EnsureKind.GameTuiGuangDlg);
					return;
				}
			case 0xA5CCEE9A://C_CallPlayerGoToRestartGame CRC32HashCode	//服务端强制用户重新启动游戏
				{
					MainRoot._gMainRoot.ChangeScene("GameHall");
					return;
				}
			case 0x521CDD29://C_CallPlayerServerClose 游戏开始维护
				{
					RoomCardNetClientModule.netModule.ClosePlayerLinkClient();
					GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-GameWH-QJKXX"), MainRoot._gUIModule.pMainCanvas.transform, false);
					EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
					dlg.Initial(EnsureDlg.EnsureKind.Game_WEIHU_Dlg);
					return;
				}
            case 0xF782BBA9://C_AnsPlayerSetBuff CRC32HashCode   //修改玩家buff信息返回
                {
                    MainRoot._gPlayerData.playerBuff = (string)args[2];
                    string refreshFlag = (string)args[3];
                    string[] tempFlag = refreshFlag.Split(':');
                    int flag;
                    for (int i = 0; i < tempFlag.Length; i++)
                    {
                        flag = System.Convert.ToInt32(tempFlag[i]);
                        MainRoot._gPlayerData.RefreshPlayerBuff((PlayerData.BUFFBIN)flag);

                        //非要加提示
                        if ((PlayerData.BUFFBIN)flag == PlayerData.BUFFBIN.BIN_ACTIVATIONCODE)
                        {//已经领取50万金币和35房卡
                            if (!"".Equals(MainRoot._gPlayerData.sUserPromotionCode))
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(52, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);
                            }
                        }
                        if ((PlayerData.BUFFBIN)flag == PlayerData.BUFFBIN.BIN_ISGETSTARTGIFT)
                        {//已经领取30房卡
                            if (MainRoot._gPlayerData.isUserGetCarGift)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(51, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);
                            }
                        }
                    }
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.FreshPromotionGiftData();
                    return;
                }
			case 0xC06F544B://C_CallInitialTipsMessage CRC32HashCode 初始化客户端系统消息
				{
					int n = 0;
					object[] args1 = new object[args.Length];
					for (int i = 3; i < args.Length; i+=3)
					{
						if ((int)args[i+2]!=2)//如果是公告
						{
							args1[n] = args[i];
							args1[n+1] = args[i+1];
							args1[n+2] = (int)args[i+2] == 0 ? false : true ;
							n += 3;
						}
						else //专题比赛的活动详情
						{
							MainRoot._gPlayerData.sThemeRace_HuoDongXiangQing = (string)args[i];
						}
					}
					object[] args2 = new object[n];
					System.Array.Copy(args1, args2, n);

					if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pTipsMsgText!=null)
					{
						MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pTipsMsgText.SetTipsMsgTextByServerPort(args2);
					}
					return;
				}
			case 0x4F631BC://C_CallPlayerSendGlobalChatMessage_toClient 玩家发送世界广播
				{
					if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pTipsMsgText != null)
					{
						MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pTipsMsgText.SetTipsMsgText((string)args[4], (string)args[5], (float)args[6], (bool)args[7]);
					}
					break;
				}
            case 0xC7A4348A://Net_CallCheckInputCodeOk CRC32HashCode //短信验证码返回
                {
                    MainRoot._gPlayerData.TelInfo = (string)args[4];
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShouJiHaoYanZhengDlg.OnReceivedQueRenBtMsg((int)(args[3]));
                    break;
                }
            case 0xD2C44DB9://Net_AnsPlayerLoginReturnFailed_Audit CRC32HashCode    ////审核玩家登录注册失败的消息
				{
					//下标3的参数是返回错误码：0用户不存在 2密码错（提示用户名密码错，用于登录界面）， 1用户已存在 用于注册界面
					switch ((int)args[3])
					{
						case 0:
							MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);//用户不存在
							break;
						case 1:
							MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(59, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);//用户已存在
							break;
						case 2:
							MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(58, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);//密码错
							break;
						default:
							break;
					}
					break;
				}
            case 0xEF65FD46://C_GetHaiXuanChartsList CRC32HashCode 获取海选排行榜
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.ShowHaiXuanRuKouPaiHangInfo(args);
                    }
                    break;
                }
            case 0xABD91C9A://C_GetMultRaceHaiXuanChartsList CRC32HashCode 获取 社区 海选排行榜
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.ShowHaiXuanRuKouPaiHangInfo(args);
                    }
                    break;
                }
            case 0x6B38587://C_AnsThemeRace_RaceState CRC32HashCode    //玩家请求专题赛状态信息 返回
                {
                    //args[2]: 比赛类型(0 比赛时间未到, 1 海选赛, 2 小组赛, 3 决赛)
                    //args[3]: 比赛状态(0 比赛时间未到, 1 海选赛开始, 2 小组赛准备, 3 小组赛开始)
                    //args[4]: 比赛场次信息(-1/0 第一场, 1 第二场, 2 第三场, 3 第四场)
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.OnReceivedZhuTiSaiBtMsg((int)args[2], (int)args[3], (int)args[4]);
                        //MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.OnReceivedZhuTiSaiBtMsg((int)args[2], (int)args[3], 0); //test
                    }
                    break;
                }
            case 0xD62E05FB://C_AnsMultRace_RaceState CRC32HashCode    //玩家请求 社区 赛状态信息 返回
                {
                    //args[2]: 比赛类型(0 比赛时间未到, 1 海选赛, 2 小组赛, 3 决赛)
                    //args[3]: 比赛状态(0 比赛时间未到, 1 海选赛开始, 2 小组赛准备, 3 小组赛开始)
                    //args[4]: 比赛场次信息(-1/0 第一场, 1 第二场, 2 第三场, 3 第四场)
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.OnReceivedZhuTiSaiBtMsg((int)args[2], (int)args[3], (int)args[4]);
                        //MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.OnReceivedZhuTiSaiBtMsg((int)args[2], (int)args[3], 0); //test
                    }
                    break;
                }
            case 0xF2EC866B://C_AnsPlayerAddThemeRace_HaiXuan_Failed CRC32HashCode    //玩家加入华商海选房失败消息
                {
                    //args[2]: 0 比赛时间未到, 1 海选入场券界面, 2 用户已在其他轮次获得淘汰赛资格, 3 允许玩家进入海选比赛游戏.
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.OnReceivedStartBtMsg((int)args[2]);
                    }
                    break;
                }
            case 0xBFEC4FB0://C_AnsThemeRace_GroupRaceInfo CRC32HashCode CRC32HashCode 玩家专题赛小组赛  返回所有晋级玩家信息列表
                {
                    //args[3, 3 + 59]: 60个单元的玩家Id, args[3 + 60, 3 + 60 + (32 * 4) - 1]: 32强玩家的详细信息(id, name, url, sex).
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg.ShowTaoTaiSaiPlayerInfo(args);
                    }
                    break;
                }
            case 0x3D0C6D97://C_AnsMultRace_GroupRaceInfo CRC32HashCode 玩家 社区赛小组赛  返回玩家信息列表
                {
                    //args[3, 3 + 59]: 60个单元的玩家Id, args[3 + 60, 3 + 60 + (32 * 4) - 1]: 32强玩家的详细信息(id, name, url, sex).
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg.ShowTaoTaiSaiPlayerInfo(args);
                    }
                    break;
                }
            case 0xF338D622://C_AnsPlayerJoinThemeRace_GroupRoom_ReturnState CRC32HashCode   //玩家加入专题小组赛 返回状态
                {
                    //args[3] 比赛状态: 0 比赛未开始, 1 比赛准备阶段, 2 比赛期间.
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg.OnReceivedStartBtMsg((int)args[3]);
                    }

                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg.OnReceivedStartBtMsg((int)args[3]);
                    }
                    break;
                }
            case 0x75AAD1C5://C_AnsPlayerJoinMultRace_GroupRoom_ReturnState CRC32HashCode   //玩家加入 社区 小组赛 返回状态
                {
                    //args[3] 比赛状态: 0 比赛未开始, 1 比赛准备阶段, 2 比赛期间.
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pTaoTaiSaiRuKouDlg.OnReceivedStartBtMsg((int)args[3]);
                    }

                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg.OnReceivedStartBtMsg((int)args[3]);
                    }
                    break;
                }
            case 0x35EE041E://C_AnsThemeRace_GroupRaceInfo_Final CRC32HashCode 玩家专题赛小组赛 决赛  返回玩家信息列表
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg.ShowZongJueSaiPlayerInfo(args);
                    }
                    break;
                }
            case 0xA088C322://C_AnsMultRace_GroupRaceInfo_Final CRC32HashCode 玩家社区 赛小组赛 决赛  返回玩家信息列表
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null &&
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pZongJueSaiRuKouDlg.ShowZongJueSaiPlayerInfo(args);
                    }
                    break;
                }
            case 0x50150D4D://C_AnsPlayerLoginReturn_Second CRC32HashCode 玩家登陆的时候 二次给的玩家数据
                {
                    if (MainRoot._gPlayerData != null)
                    {
                        MainRoot._gPlayerData.InitialPlayerDataSecond(args);
                    }
                    break;
                }
            case 0x6DC8CD9B://C_AnsCircleBingo CRC32HashCode  //转盘返回
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pHuaShangDaZhuanPan != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pHuaShangDaZhuanPan.InitMovePoint((int)args[3]);
                    }
                    break;
                }
            case 0x4982D2DD://C_AnsOfficialRoomListInfo CRC32HashCode //返回红包赛的信息
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg.ShowQinRenBiSaiChangInfo(args);
                    }
                    break;
                }
            case 0xAF8327FF://C_AnsPlayerJoinOfficialRoomReturn CRC32HashCode   //玩家加入系统红包赛返回
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg.OnReceivedBaoMingBtMsg((int)args[3]);
                    }
                    break;
                }
            default:
                break;
        }
    }
    private IEnumerator CaptureScreenshotNormal(Vector2 startPos, Vector2 endPos, string savePath, string fileName)
    {
        yield return new WaitForEndOfFrame();

        Texture2D screenShot = new Texture2D((int)Mathf.Abs(endPos.x - startPos.x - 2), (int)Mathf.Abs(endPos.y - startPos.y - 2), TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startPos.x < endPos.x ? startPos.x : endPos.x, startPos.y < endPos.y ? UnityEngine.Screen.height - endPos.y : UnityEngine.Screen.height - startPos.y, Mathf.Abs(endPos.x - startPos.x - 2), Mathf.Abs(endPos.y - startPos.y - 2)), 0, 0);
        screenShot.Apply();
        // 然后将这些纹理数据转成一个png图片文件  
        byte[] bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);
        //保存图片
        string finalPath = savePath + "//" + fileName;
        System.IO.File.WriteAllBytes(finalPath, bytes);
    }
    /// <summary>
    /// 截图，只能截界面的图
    /// </summary>
    /// <param name="startPos">起始位置</param>
    /// <param name="endPos">最终位置</param>
    /// <param name="savePath">路径</param>
    /// <param name="fileName">文件名</param>
    public void CaptureUIScreenshotByPoint(Vector2 startPos, Vector2 endPos, string savePath, string fileName)
    {
        StartCoroutine(CaptureScreenshotNormal(startPos, endPos, savePath, fileName));
    }

    public Texture2D ScreenshotZhanJiTexture;
    /// <summary>
    /// 截取玩家战绩图片.
    /// </summary>
    public void CaptureScreenshotZhanJi(Vector2 startPos, Vector2 endPos, Material mat)
    {
        StartCoroutine(CaptureScreenshotZhanJiImg(startPos, endPos, mat));
    }

    IEnumerator CaptureScreenshotZhanJiImg(Vector2 startPos, Vector2 endPos, Material mat)
    {
        yield return new WaitForEndOfFrame();

        if (ScreenshotZhanJiTexture != null) {
            Destroy(ScreenshotZhanJiTexture);
            ScreenshotZhanJiTexture = null;
        }
        Texture2D screenShot = new Texture2D((int)Mathf.Abs(endPos.x - startPos.x - 2), (int)Mathf.Abs(endPos.y - startPos.y - 2), TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startPos.x < endPos.x ? startPos.x : endPos.x, startPos.y < endPos.y ? UnityEngine.Screen.height - endPos.y : UnityEngine.Screen.height - startPos.y, Mathf.Abs(endPos.x - startPos.x - 2), Mathf.Abs(endPos.y - startPos.y - 2)), 0, 0);
        screenShot.Apply();
        ScreenshotZhanJiTexture = screenShot;
        mat.mainTexture = screenShot;

        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pGaoJiJieSuanCtrl.DestroyThis();
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnShareGaoJiJieSuanObj();
    }

    /// <summary>
    /// 销毁ScreenshotZhanJiTexture对象.
    /// </summary>
    public void DestroyScreenshotZhanJiTexture()
    {
        if (ScreenshotZhanJiTexture != null) {
            Destroy(ScreenshotZhanJiTexture);
            ScreenshotZhanJiTexture = null;
        }
    }

    /// <summary>
    /// 截取玩家战绩图片到微信.
    /// </summary>
    public void CaptureScreenshotZhanJiToWeChat(Vector2 startPos, Vector2 endPos, ShareZhanJiCtrl.ShareZhanJiEnum eShareState)
    {
        StartCoroutine(CaptureScreenshotZhanJiImgToWeChat(startPos, endPos, eShareState));
    }

    IEnumerator CaptureScreenshotZhanJiImgToWeChat(Vector2 startPos, Vector2 endPos, ShareZhanJiCtrl.ShareZhanJiEnum eShareState)
    {
        yield return new WaitForEndOfFrame();
		string finalPath = "";
		byte[] bytes  = null;
		Texture2D screenShot = new Texture2D((int)Mathf.Abs(endPos.x - startPos.x - 2), (int)Mathf.Abs(endPos.y - startPos.y - 2), TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startPos.x < endPos.x ? startPos.x : endPos.x, startPos.y < endPos.y ? UnityEngine.Screen.height - endPos.y : UnityEngine.Screen.height - startPos.y, Mathf.Abs(endPos.x - startPos.x - 2), Mathf.Abs(endPos.y - startPos.y - 2)), 0, 0);
        screenShot.Apply();
		try
		{
			// 然后将这些纹理数据转成一个png图片文件.
			bytes = screenShot.EncodeToPNG();
			Destroy(screenShot);

			Debug.Log("Unity:保存图片");

			finalPath = Application.temporaryCachePath + "/sharezhanji" + System.DateTime.Now.ToFileTime().ToString() + Time.realtimeSinceStartup.ToString() + ".png";
			Debug.Log("Unity:" + "path -- " + finalPath);
			//System.IO.File.Delete(Application.temporaryCachePath + "/sharezhanji.png");
			System.IO.File.WriteAllBytes(finalPath, bytes);
		}
		catch (System.Exception e)
		{
			Debug.Log("Unity:"+e.Message.ToString());
		}


        switch (eShareState) {
            case ShareZhanJiCtrl.ShareZhanJiEnum.WeChatFriend:
                if (MainRoot._gWeChat_Module != null) {
                    MainRoot._gWeChat_Module.ShareGaoJiJieSuanZhanJiToWeChat(finalPath);
                }
                break;
            case ShareZhanJiCtrl.ShareZhanJiEnum.WeChatFriendMoments:
                if (MainRoot._gWeChat_Module != null) {
                    MainRoot._gWeChat_Module.ShareGaoJiJieSuanZhanJiToWeChatMoments(finalPath);
                }
                break;
        }
    }
}
