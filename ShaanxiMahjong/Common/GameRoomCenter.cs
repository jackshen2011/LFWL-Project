using UnityEngine;
using System.Collections;
using Mahjong;
using System.IO;
using RoomCardNet;

class GameRoomCenter : MonoBehaviour
{
    //-------------------声明委托-----------------
    public delegate void DelegateTest(MsgInfo msg);
    //-------------------定义委托----------------
    public DelegateTest mytest;//委托测试/// <summary>
    /// CoinRoom -> 金币房.
    /// FriendRoom -> 好友房.
    /// </summary>
    public enum GameRoomType
    {
        Null,
        CoinRoom,
        FriendRoom,
    }
    bool bIsShow = false;
    /// <summary>
	/// 游戏房间对象
	/// </summary>
    public GameRoom gGameRoom = null;
    void Start()
    {

    }
	/// <summary>
	/// 游戏房间初始化过程
	/// </summary>
	/// <param name="args"></param>
    public void InitialGameRoom()
    {
        if (gGameRoom == null)
        {
            GameObject temp;
            temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameRoom"), MainRoot._gGameRoomCenter.gameObject.transform, false);
            gGameRoom = temp.GetComponent<GameRoom>();
            gGameRoom.Initial();
        }
    }
	/// <summary>
	/// 游戏房间内的消息都会转到这里执行
	/// </summary>
	/// <param name="nFunId"></param>
	/// <param name="args"></param>
	public void OnNetMessageEnterRoom(uint nFunId, params object[] args)    
    {
        try
        {
            if (args.Length == 0)
            {
                return;
            }
            int[] daoJuDt = new int[3]{10, 15, 20}; //从服务器获取.
			switch (nFunId)
            {
                case 0xF2EC866B://C_AnsPlayerAddThemeRace_HaiXuan_Failed CRC32HashCode    //玩家加入华商海选房失败消息
                    {
                        //args[2]: 0 比赛时间未到, 1 海选入场券界面, 2 用户已在其他轮次获得淘汰赛资格, 3 允许玩家进入海选比赛游戏.
                        if ((int)args[2] != 3 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ShaanxiMahjong")
                        {
                            MainRoot._gMainRoot.IsSpawnHaiXuanOver = true;
                            MainRoot._gUIModule.pUnModalUIControl.SpawnJinRiHaiXuanSaiOver();
                            MainRoot._gMainRoot.ChangeScene("GameHall");
                            return;
                        }
                        break;
                    }
                case 0x50BFA852://C_AnsSendGameStart CRC32HashCode  //金币麻将房，游戏开始
					{
						Debug.Log("Unity:"+"GoldRoom Start 1");
						if (MainRoot._gGameRoomCenter.gGameRoom != null)
						{
							Debug.Log("Unity:"+"GoldRoom Start 2");
							MainRoot._gRoomData.cCurRoomData.nRoomId = (int)args[2];
							MainRoot._gRoomData.cCurRoomData.eRoomType = (OneRoomData.RoomType)(int)args[3];
							MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel = (int)args[4];
							MainRoot._gRoomData.cCurRoomData.nDiFen = (int)args[7];
                            MainRoot._gRoomData.cCurRoomData.bIsErRenMjRoom = false;
                            MainRoot._pMJGameTable.RefreshRoomTabelLevelShow();
                            PlayerBase[] tempplay = new PlayerBase[4];
							PlayerBase tempb;
							bool isUserNeedChange = true;
							for (int m = 0; m < 4; m++)
							{
								MainRoot._gGameRoomCenter.gGameRoom.ResetOneInfoPanelPos(m);
							}

							for (int m = 0; m < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; m++)	//检查是否换了人...
							{
								isUserNeedChange = true;
								tempb = MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[m];
								
								/*if (tempb != null)
								{
									for (int n = 8; n < 32; n+=6)
									{
										if (tempb.nUserId == (int)args[n + 1] )
										{
											isUserNeedChange = false;
										}
									}*/
									if (isUserNeedChange)
									{
										tempb.IsYiZhunBei = false;
										//删除已准备或准备中消息.
										tempb.CloseAllReadyMsgTip();
										if (tempb.pInfoPanel.pDetail != null)
										{
											tempb.pInfoPanel.pDetail.DestroyThis();
											tempb.pInfoPanel.pDetail = null;
										}
										if (tempb.pInfoPanel != null)
										{
											tempb.pInfoPanel.DestroyThis();
											tempb.pInfoPanel = null;
										}
										MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Remove(tempb);
										m--;
										tempb.DestroyThis();
									}
								//}
							}
				
							MainRoot._gGameRoomCenter.gGameRoom.SetZhuangJiaUserId = (int)args[32];
							for (int i = 8; i < 32; i += 6)
							{
								if ((int)args[i + 1] == MainRoot._gPlayerData.nUserId)
								{
									/*tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
									if (tempplay[(i - 8) / 6] != null)    //修正加入金币房间的自己的部分数据
									{
										tempplay[(i - 8) / 6].nUserSit = (int)args[i];    //将以前随便填的的座位修正一下
										tempplay[(i - 8) / 6].nUserDiFen = MainRoot._gRoomData.cCurRoomData.nDiFen;   //以及底分
                                        tempplay[(i - 8) / 6].PlayerCoin = (int)args[i + 5];   //以及金币.
									}
									else
									{*/
										MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
											(int)args[i + 1],
											(string)args[i + 2],
											(string)args[i + 3],
											(int)args[i],
											(int)args[i + 5],
											(int)args[i + 4],
											MainRoot._gRoomData.cCurRoomData.nDiFen
											);
									//}
								}
                                Debug.LogWarning("Unity: 0x50BFA852 coin " + (int)args[i + 5] + ", id " + (int)args[i + 1]);
							}
							for (int i = 8; i < 32; i += 6)
							{
								tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i + 1]);
								if ((int)args[i+1] == MainRoot._gPlayerData.nUserId)
								{
                                    if (MainRoot._gRoomData.cCurRoomData.eRoomType==OneRoomData.RoomType.RoomType_Gold)
                                    {
                                        MainRoot._gPlayerData.PlayerCoin = (int)args[i + 5];
                                    }
									else
									{
										tempplay[(i - 8) / 6].PlayerCoin = (int)args[i + 5];
									}
								}
								else
								{
									//if (tempplay[(i - 8) / 6]==null)
									//{
										MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
													(int)args[i + 1],
													(string)args[i + 2],
													(string)args[i + 3],
													(int)args[i],
													(int)args[i + 5],
													(int)args[i + 4],
													MainRoot._gRoomData.cCurRoomData.nDiFen
													);
										tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i + 1]);
									/*}
									else
									{
										tempplay[(i - 8) / 6].Initial(
											(string)args[i + 2],
											(int)args[i + 1],
											(int)args[i],
											(int)args[i + 5],
											MainRoot._gRoomData.cCurRoomData.nDiFen,
											(int)args[i + 4],
											(string)args[i + 3]
											);
									}*/
		
								}
								if (tempplay[(i - 8) / 6] == null)
								{
									Debug.Log("Unity:"+"玩家没找到也没创建：id:"+ ((int)args[i + 1]).ToString());
									continue;
								}
							}
							for (int t = 0; t < 4; t++)
							{
								//if (tempplay[t].nUserId == MainRoot._gPlayerData.nUserId)	//自己的人物面板已经初始化过了
								//{
									tempplay[t].InitialLinkPanel();    //初始化人物面板
								//}
								tempplay[t].SetZhuangjia((MainRoot._gGameRoomCenter.gGameRoom.GetZhuangJiaUserId == tempplay[t].nUserId));
							}
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnMatchPlayerPanel(args);
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(1);//匹配成功
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//匹配成功
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中
                            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PiPeiShiBaiTuiChuDlg)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PiPeiShiBaiTuiChuDlg.DestroyThis();
                            }
							if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pMatchPlayerCtrl != null)
							{
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pMatchPlayerCtrl.ShowMatching();
							}
                            MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;//金币房开始
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
                            //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(23, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配成功牌局开始
                            MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.READY, args);
						}
						break;
					}
				case 0x2EF741A7://C_AnsSendCardRoomGameStart CRC32HashCode    //房卡房间开始游戏的消息
					{
                        PlayerBase tempa;
						Debug.LogWarning("Unity:"+"C_AnsSendCardRoomGameStart!");
                        MainRoot._pMJGameTable.ClearGame();
                        MainRoot._gGameRoomCenter.gGameRoom.SetZhuangJiaUserId = (int)args[14]; //房卡房玩家坐庄
						

						for (int i = 0; i < 4; i++)
						{
							tempa = gGameRoom.GetPlayerByUserId((int)args[3 + i * 3]);
							if (tempa == null) 
							{
								//人找不到了，不处理
								continue;
							}
                            tempa.PlayerCoin=(int)args[4 + i * 3];
							//tempa.nCardCoinNum = (int)args[3 + i * 3];
							tempa.SetZhuangjia((MainRoot._gGameRoomCenter.gGameRoom.GetZhuangJiaUserId == tempa.nUserId));
							tempa.CloseAllReadyMsgTip();
						}

                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMPaiJuKaiShi();
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[15]);
						//MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(23, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配成功牌局开始
						MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.READY, args);
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(29);//删除自己已准备
																									  //下炮界面
						if (!MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())  //正常房卡麻将  
						{
							if (System.Convert.ToInt32(MainRoot._gRoomData.cCurRoomData.vRoomSetting[4]) == 5)//自由下炮类型
							{
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaPaoUI();//自己下炮
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(9, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(10, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(11, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
							}
						}
						else //二人麻将
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaZhuUI();
						}
                        MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;//房卡房开始
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
                        break;
                    }
                case 0x1C064E43://C_AnsSendThemeRace_GroupRoomGameStart CRC32HashCode //发送主题赛-小组赛开始游戏的消息 
                    {
                        //主题赛-小组赛(淘汰赛)房间开始游戏的消息
                        Debug.LogWarning("Unity:" + "C_AnsSendThemeRace_GroupRoomGameStart 1");
                        if (MainRoot._gGameRoomCenter.gGameRoom != null)
                        {
                            Debug.LogWarning("Unity:" + "C_AnsSendThemeRace_GroupRoomGameStart 1");
                            MainRoot._pMJGameTable.ClearGame();
                            MainRoot._gRoomData.cCurRoomData.nRoomId = (int)args[2];
                            MainRoot._gRoomData.cCurRoomData.eRoomType = (OneRoomData.RoomType)(int)args[3];
                            MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel = (int)args[4];
                            MainRoot._gRoomData.cCurRoomData.nDiFen = (int)args[7];
                            MainRoot._gRoomData.cCurRoomData.bIsErRenMjRoom = false;
                            MainRoot._pMJGameTable.RefreshRoomTabelLevelShow();
                            PlayerBase[] tempplay = new PlayerBase[4];
                            PlayerBase tempb;
                            bool isUserNeedChange = true;
                            for (int m = 0; m < 4; m++)
                            {
                                MainRoot._gGameRoomCenter.gGameRoom.ResetOneInfoPanelPos(m);
                            }

                            for (int m = 0; m < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; m++)    //检查是否换了人...
                            {
                                isUserNeedChange = true;
                                tempb = MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[m];

                                /*if (tempb != null)
								{
									for (int n = 8; n < 32; n+=6)
									{
										if (tempb.nUserId == (int)args[n + 1] )
										{
											isUserNeedChange = false;
										}
									}*/
                                if (isUserNeedChange)
                                {
                                    tempb.IsYiZhunBei = false;
                                    //删除已准备或准备中消息.
                                    tempb.CloseAllReadyMsgTip();
                                    if (tempb.pInfoPanel.pDetail != null)
                                    {
                                        tempb.pInfoPanel.pDetail.DestroyThis();
                                        tempb.pInfoPanel.pDetail = null;
                                    }
                                    if (tempb.pInfoPanel != null)
                                    {
                                        tempb.pInfoPanel.DestroyThis();
                                        tempb.pInfoPanel = null;
                                    }
                                    MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Remove(tempb);
                                    m--;
                                    tempb.DestroyThis();
                                }
                                //}
                            }

                            MainRoot._gGameRoomCenter.gGameRoom.SetZhuangJiaUserId = (int)args[32];
                            for (int i = 8; i < 32; i += 6)
                            {
                                if ((int)args[i + 1] == MainRoot._gPlayerData.nUserId)
                                {
                                    /*tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
									if (tempplay[(i - 8) / 6] != null)    //修正加入金币房间的自己的部分数据
									{
										tempplay[(i - 8) / 6].nUserSit = (int)args[i];    //将以前随便填的的座位修正一下
										tempplay[(i - 8) / 6].nUserDiFen = MainRoot._gRoomData.cCurRoomData.nDiFen;   //以及底分
                                        tempplay[(i - 8) / 6].PlayerCoin = (int)args[i + 5];   //以及金币.
									}
									else
									{*/
                                    MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
                                        (int)args[i + 1],
                                        (string)args[i + 2],
                                        (string)args[i + 3],
                                        (int)args[i],
                                        (int)args[i + 5],
                                        (int)args[i + 4],
                                        MainRoot._gRoomData.cCurRoomData.nDiFen
                                        );
                                    //}
                                }
                            }
                            for (int i = 8; i < 32; i += 6)
                            {
                                tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i + 1]);
                                if ((int)args[i + 1] == MainRoot._gPlayerData.nUserId)
                                {
                                    if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
                                        MainRoot._gPlayerData.PlayerCoin = (int)args[i + 5];
                                }
                                else
                                {
                                    //if (tempplay[(i - 8) / 6]==null)
                                    //{
                                    MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
                                                (int)args[i + 1],
                                                (string)args[i + 2],
                                                (string)args[i + 3],
                                                (int)args[i],
                                                (int)args[i + 5],
                                                (int)args[i + 4],
                                                MainRoot._gRoomData.cCurRoomData.nDiFen
                                                );
                                    tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i + 1]);
                                    /*}
									else
									{
										tempplay[(i - 8) / 6].Initial(
											(string)args[i + 2],
											(int)args[i + 1],
											(int)args[i],
											(int)args[i + 5],
											MainRoot._gRoomData.cCurRoomData.nDiFen,
											(int)args[i + 4],
											(string)args[i + 3]
											);
									}*/

                                }
                                if (tempplay[(i - 8) / 6] == null)
                                {
                                    Debug.Log("Unity:" + "玩家没找到也没创建：id:" + ((int)args[i + 1]).ToString());
                                    continue;
                                }
                            }
                            for (int t = 0; t < 4; t++)
                            {
                                //if (tempplay[t].nUserId == MainRoot._gPlayerData.nUserId)	//自己的人物面板已经初始化过了
                                //{
                                tempplay[t].InitialLinkPanel();    //初始化人物面板
                                                                   //}
                                tempplay[t].SetZhuangjia((MainRoot._gGameRoomCenter.gGameRoom.GetZhuangJiaUserId == tempplay[t].nUserId));
                            }
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnMatchPlayerPanel(args);
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(1);//匹配成功
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//匹配成功
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中

                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[33]);
                            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PiPeiShiBaiTuiChuDlg)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PiPeiShiBaiTuiChuDlg.DestroyThis();
                            }
                            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pMatchPlayerCtrl != null)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pMatchPlayerCtrl.ShowMatching();
                            }
                            MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;//淘汰赛开始
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
                            //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(23, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配成功牌局开始
                            MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.READY, args);
                        }
                        break;
                    }
                case 0x2688719A://C_AnsSendMultRace_GroupRoomGameStart CRC32HashCode //发送社区 赛 淘汰赛房间开始游戏的消息
                    {
                        //社区赛-小组赛(淘汰赛)房间开始游戏的消息
                        Debug.LogWarning("Unity:" + "C_AnsSendThemeRace_GroupRoomGameStart 1");
                        if (MainRoot._gGameRoomCenter.gGameRoom != null)
                        {
                            Debug.LogWarning("Unity:" + "C_AnsSendThemeRace_GroupRoomGameStart 1");
                            MainRoot._pMJGameTable.ClearGame();
                            MainRoot._gRoomData.cCurRoomData.nRoomId = (int)args[2];
                            MainRoot._gRoomData.cCurRoomData.eRoomType = (OneRoomData.RoomType)(int)args[3];
                            MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel = (int)args[4];
                            MainRoot._gRoomData.cCurRoomData.nDiFen = (int)args[7];
                            MainRoot._gRoomData.cCurRoomData.bIsErRenMjRoom = false;
                            MainRoot._pMJGameTable.RefreshRoomTabelLevelShow();
                            PlayerBase[] tempplay = new PlayerBase[4];
                            PlayerBase tempb;
                            bool isUserNeedChange = true;
                            for (int m = 0; m < 4; m++)
                            {
                                MainRoot._gGameRoomCenter.gGameRoom.ResetOneInfoPanelPos(m);
                            }

                            for (int m = 0; m < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; m++)    //检查是否换了人...
                            {
                                isUserNeedChange = true;
                                tempb = MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[m];

                                /*if (tempb != null)
								{
									for (int n = 8; n < 32; n+=6)
									{
										if (tempb.nUserId == (int)args[n + 1] )
										{
											isUserNeedChange = false;
										}
									}*/
                                if (isUserNeedChange)
                                {
                                    tempb.IsYiZhunBei = false;
                                    //删除已准备或准备中消息.
                                    tempb.CloseAllReadyMsgTip();
                                    if (tempb.pInfoPanel.pDetail != null)
                                    {
                                        tempb.pInfoPanel.pDetail.DestroyThis();
                                        tempb.pInfoPanel.pDetail = null;
                                    }
                                    if (tempb.pInfoPanel != null)
                                    {
                                        tempb.pInfoPanel.DestroyThis();
                                        tempb.pInfoPanel = null;
                                    }
                                    MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Remove(tempb);
                                    m--;
                                    tempb.DestroyThis();
                                }
                                //}
                            }

                            MainRoot._gGameRoomCenter.gGameRoom.SetZhuangJiaUserId = (int)args[32];
                            for (int i = 8; i < 32; i += 6)
                            {
                                if ((int)args[i + 1] == MainRoot._gPlayerData.nUserId)
                                {
                                    /*tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
									if (tempplay[(i - 8) / 6] != null)    //修正加入金币房间的自己的部分数据
									{
										tempplay[(i - 8) / 6].nUserSit = (int)args[i];    //将以前随便填的的座位修正一下
										tempplay[(i - 8) / 6].nUserDiFen = MainRoot._gRoomData.cCurRoomData.nDiFen;   //以及底分
                                        tempplay[(i - 8) / 6].PlayerCoin = (int)args[i + 5];   //以及金币.
									}
									else
									{*/
                                    MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
                                        (int)args[i + 1],
                                        (string)args[i + 2],
                                        (string)args[i + 3],
                                        (int)args[i],
                                        (int)args[i + 5],
                                        (int)args[i + 4],
                                        MainRoot._gRoomData.cCurRoomData.nDiFen
                                        );
                                    //}
                                }
                            }
                            for (int i = 8; i < 32; i += 6)
                            {
                                tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i + 1]);
                                if ((int)args[i + 1] == MainRoot._gPlayerData.nUserId)
                                {
                                    if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
                                        MainRoot._gPlayerData.PlayerCoin = (int)args[i + 5];
                                }
                                else
                                {
                                    //if (tempplay[(i - 8) / 6]==null)
                                    //{
                                    MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
                                                (int)args[i + 1],
                                                (string)args[i + 2],
                                                (string)args[i + 3],
                                                (int)args[i],
                                                (int)args[i + 5],
                                                (int)args[i + 4],
                                                MainRoot._gRoomData.cCurRoomData.nDiFen
                                                );
                                    tempplay[(i - 8) / 6] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i + 1]);
                                    /*}
									else
									{
										tempplay[(i - 8) / 6].Initial(
											(string)args[i + 2],
											(int)args[i + 1],
											(int)args[i],
											(int)args[i + 5],
											MainRoot._gRoomData.cCurRoomData.nDiFen,
											(int)args[i + 4],
											(string)args[i + 3]
											);
									}*/

                                }
                                if (tempplay[(i - 8) / 6] == null)
                                {
                                    Debug.Log("Unity:" + "玩家没找到也没创建：id:" + ((int)args[i + 1]).ToString());
                                    continue;
                                }
                            }
                            for (int t = 0; t < 4; t++)
                            {
                                //if (tempplay[t].nUserId == MainRoot._gPlayerData.nUserId)	//自己的人物面板已经初始化过了
                                //{
                                tempplay[t].InitialLinkPanel();    //初始化人物面板
                                                                   //}
                                tempplay[t].SetZhuangjia((MainRoot._gGameRoomCenter.gGameRoom.GetZhuangJiaUserId == tempplay[t].nUserId));
                            }
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnMatchPlayerPanel(args);
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(1);//匹配成功
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(49);//匹配成功
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中

                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[33]);
                            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PiPeiShiBaiTuiChuDlg)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PiPeiShiBaiTuiChuDlg.DestroyThis();
                            }
                            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pMatchPlayerCtrl != null)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pMatchPlayerCtrl.ShowMatching();
                            }
                            MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;//淘汰赛开始
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
                            //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(23, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配成功牌局开始
                            MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.READY, args);
                        }
                        break;
                    }
                case 5: //都下炮了，开始掷骰子
					if (MainRoot._gGameRoomCenter.gGameRoom != null)
					{
						MainRoot._gMainRoot.Tos("Need Tos 已经下炮！");
					}
					break;
				case 0xDC2A3C19://C_AnsPlayerInitCardData CRC32HashCode 游戏开始给玩家发牌 ,都下炮了，掷骰子结果，刷牌墙
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.JieSanBtObj.SetActive(true);
                        }
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShengYuPaiShuObj.SetActive(true);
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan ||
                            MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                        {
                            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveExitBt(false);
                            }
                        }
                        else
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ChatBtObj.SetActive(true);
                        }
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaPaoUI!=null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaPaoUI.OnSysMsgWaitEventHappen(3);//下炮界面关闭
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaPaoUI = null;
						}

						PlayerBase tempplay;
						for (int i = 0; i < 4; i++)
						{
							tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i * 2 + 8]);
							if (tempplay)
							{
								if (!MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())  //正常房卡麻将  
								{
									tempplay.SetXiaPaoImg((int)args[i * 2 + 9]);
								}
								else //二人麻将，显示下注数
								{
									if (tempplay.pInfoPanel)
									{
										tempplay.pInfoPanel.ShowPlayerCoinNumInfo((int)args[i * 2 + 9]);
									}
								}
							}
                        }
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ClearXiaPaoZhong();//下炮完毕
                        MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.START, args);//服务端返回的桌面消息
                                                                                                           /*MainRoot._pMJGameTable.StartGame((int)DIRECTION.SOUTH,//初始化方位
																											   1,//设置庄家
																											   2, 3, 4, 5,//摇骰子
																											   1, 4,//抓牌位置
																											   new int[] { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5 });*///初始化自己手牌
                        MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;//金币房开始
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
                    }
					break;
				case 0x5738E7C8://C_AnsPlayerSendCardData CRC32HashCode //发牌

                        //Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@@@@@ = " + (int)args[2] + " === " + (byte)args[3] + " === " + (byte)args[4]);

                    if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						//MainRoot._pMJGameTable.UserTakeOneMaJiang((int)args[0], MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家接牌
						//MainRoot._pMJGameTable.UserTakeOneMaJiang(MainRoot._pMJGameTable.GetPlayerIndexByID((int)(args[2])), MainRoot._pMJGameTable.GetMaJiangByInt((byte)(args[3])));
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.GetChiPengHuTiShi().HideChiPengHuTips();//屏蔽碰杠吃
                        MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.TAKE, args);//服务端返回的桌面消息
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowCurOperatePlayerEff((int)args[2]);//头像转圈圈
                    }
					break;
				case 0xE03BFA1F://C_AnsPlayerOutCardData CRC32HashCode //用户出牌
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect!=null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						//MainRoot._gMainRoot.Tos("Need Tos 打牌 if it`s itself！");
						//MainRoot._pMJGameTable.PutOutMJ((int)args[0], (int)args[0] == 0 ? MainRoot._pMJGameTable.GetRecentPutOut() : MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家打牌
						MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.PUTOUT, args);//服务端返回的桌面消息
                    }
					break;
				case 0xF9CE85AB://C_AnsPlayerOperateResult CRC32HashCode //用户操作 包括 吃 碰 杠
					break;
				case 9://碰牌
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.PENG, args);//服务端返回的桌面消息
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialEff(3, (int)(args[2]));//碰特效
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowCurOperatePlayerEff((int)args[2]);//头像转圈圈
                    }
					break;
				case 10://直杠牌
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.GANG, args);//服务端返回的桌面消息
																										  //MainRoot._pMJGameTable.PlayerDoZhiGang((int)args[0], MainRoot._pMJGameTable.GetLastPutOut(), MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家打牌
																										  //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(18, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示下
																										  //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(19, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示左
																										  //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(20, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示右
																										  //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(21, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示上
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialEff(1, (int)(args[2]));//杠特效
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowCurOperatePlayerEff((int)args[2]);//头像转圈圈
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
					}
					break;
				case 11://直接续杠牌成功
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						MainRoot._pMJGameTable.PlayerDoXuGang((int)args[0], MainRoot._pMJGameTable.GetLastPutOut(), MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家打牌
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(18, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示下
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(19, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示左
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(20, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示右
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(21, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示上
					}
					break;
				case 12://续杠后别的玩家放弃胡时续杠成功
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						MainRoot._pMJGameTable.PlayerDoXuGangAfter((int)args[0], MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家打牌
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(18, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示下
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(19, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示左
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(20, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示右
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(21, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示上
					}
					break;
				case 13://暗杠牌成功
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						MainRoot._pMJGameTable.PlayerDoAnGang((int)args[0], MainRoot._pMJGameTable.GetSelfChoice(), MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家打牌
																																							   //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(18, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示下
																																							   //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(19, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示左
																																							   //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(20, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示右
																																							   //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(21, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示上
					}
					break;
				//case 0x5D50A1C4://C_AnsPlayerChiHuOrZha CRC32HashCode //用户操作 胡牌
				case 0xAD2B2DA2://C_AnsPlayerGameEnd CRC32HashCode// 一局结束 结算
					if (MainRoot._gGameRoomCenter.gGameRoom != null && MainRoot._pMJGameTable)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						if (MainRoot._gPlayerData.relinkUserRoomData != null)
						{
							MainRoot._gPlayerData.relinkUserRoomData = null; //清除当前牌局信息.
						}
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowTuoGuanUI(false);
                        MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.HU, args);//服务端返回的桌面消息
																										//MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(18, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示下
																										//MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(19, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示左
																										//MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(20, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示右
																										//MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(21, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, args[0]);//杠牌分值提示上

						//Debug.LogError("Unity:"+"张雷看这里，记得给传分数，然后结算界面才能显示");
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.GetChiPengHuTiShi().HideChiPengHuTips();
						if ((int)(args[26]) == (int)(args[28]))
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialEff(4, (int)(args[28]));//自摸特效
						}
						else
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialEff(2, (int)(args[28]));//胡特效
						}
                        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.HideTingPaiTishi();	//关闭停牌提示
                        }
						
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialJieSuanUI(args);//结算界面 
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowCurOperatePlayerEff(0);//停止所有头像转圈圈
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowTuoGuanUI(false);//托管界面隐藏

						if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold ||
                            MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan)
						{
							MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.COLSE;
						}
                    }
					break;
				case 15://流局
					if (MainRoot._gGameRoomCenter.gGameRoom != null)
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowTuoGuanUI(false);//托管界面隐藏
                        MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.LIUJU, args);//服务端返回的桌面消息
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(22, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//流局，牌局结束
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialJieSuanUI(args, true);//结算界面 
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.HideTingPaiTishi();   //关闭停牌提示
						}

						//MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialJieSuanUI(args);//结算界面 
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowCurOperatePlayerEff(0);//停止所有头像转圈圈

						if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold ||
                            MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan)
						{
							MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.COLSE;
						}
					}
					break;
				case 0xE1E04923://C_AnsPlyaerNotify CRC32HashCode //给用户的提示
					{
                        //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.GetChiPengHuTiShi().ShowChiPengHuTips(((byte)args[4] & CMD_SXMJ.WIK_CHI_HU) == CMD_SXMJ.WIK_CHI_HU, ((byte)args[4] & CMD_SXMJ.WIK_GANG) == CMD_SXMJ.WIK_GANG, ((byte)args[4] & CMD_SXMJ.WIK_PENG) == CMD_SXMJ.WIK_PENG, false);
                        if (MainRoot._pMJGameTable != null)
                            MainRoot._pMJGameTable.SetChiPengHuTipsDelay((byte)args[4]);
                    }
					break;
				case 16://停牌
					if (MainRoot._gGameRoomCenter.gGameRoom != null)
					{
						if (!MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.activeSelf)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.SetActive(true);
							if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi==null)
							{
								GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/TingPaiTishiUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi = temp.GetComponent<TingPaiTishiUI>();
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.Initial();
							}

							Vector3[] tparams = new Vector3[4];
							for (int i = 0; i < tparams.Length; i++)
							{
								tparams[i] = new Vector3(i + 1, Mathf.Min(i * 2, 4), i % 9);
							}
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTishi(tparams);
						}
					}
					break;
				case 0xC9D8E039://C_CallPlayerJiuJiOk CRC32HashCode
					{
						//修改玩家数据包括其他玩家
						if ((int)args[1] == MainRoot._gPlayerData.nUserId)
						{
							MainRoot._gPlayerData.PlayerCoin += (int)args[2];
                            MainRoot._gPlayerData.UpdatePlayerDtInfo();
                            if (args.Length > 4)
							{
								MainRoot._gPlayerData.SetPlayerJiuJiFlag((int)args[3], (System.DateTime)args[4]);
							}
						}
						//修改界面
						if (gGameRoom != null)
						{
							for (int i = 0; i < gGameRoom.m_PlayerList.Count; i++)
							{
								if (gGameRoom.m_PlayerList[i].nUserId == (int)args[1])
								{
									gGameRoom.m_PlayerList[i].PlayerAddCoin((int)args[2]);
									break;
								}
							}
						}
						break;
					}
				case 0x424B68D://C_AnsPlayerChangeCoin CRC32HashCode  //游戏内玩家金币的增减 这里发送总量客户端自行处理增减积分
					{
						int minus = 0;
						if (gGameRoom == null)
						{
							break;
						}
                        PlayerBase player;
						for (int index = 0; index < 4; index++)
						{
                            player = gGameRoom.GetPlayerByUserId((int)args[2 + index * 2]);
                            if (player)
                            {
                                minus = (int)args[3 + index * 2] - player.PlayerCoin;
                                if (minus != 0)
                                {
                                    player.PlayerAddCoin(minus);
                                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(18 + player.nUIUserSit, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, minus);//分值提示
                                    if (player.nUserId == gGameRoom.pMainPlayer.nUserId)
                                    {//如果是自己则要刷自己的数据
                                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
                                        {
                                            MainRoot._gPlayerData.PlayerCoin += minus;
                                            MainRoot._gPlayerData.UpdatePlayerDtInfo();
                                            if (MainRoot._gPlayerData.PlayerCoin<=0)
                                            {
                                                //暂时不弹
                                                /*if (!MainRoot._gPlayerData.GetCanBringJiuJiCoin())
                                                {
                                                    MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01);
                                                }
                                                else {
                                                    MainRoot._gUIModule.pUnModalUIControl.SpawnFuLiJinDlg();
                                                }*/
                                            }
                                        }
                                    }
                                }
                            }
						}
						break;
					}

				case 0x8CB85B3E://C_AnsPlayerJoinCardRoom CRC32HashCode   //有玩家加入房卡房间
								//显示其他人，如满四人显示出现准备按钮及“准备中”
					{
						Debug.Log("Unity:"+ "有玩家加入房卡房间");
						int n = 2;
						bool isUserMax = true;
						int ncou8ntuser = 0;
						PlayerBase tempa;
						if ((int)args[1] == MainRoot._gRoomData.cCurRoomData.nRoomId)
						{
							for (int i = 0; i < 4; i++)
							{
								if ((int)args[n + i * 5] != -1) //不是空座位
								{
									tempa = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[n + i * 5]);
									if (tempa == null)	//新加入，出准备提示
									{
										MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
										(int)args[n + i * 5],
										(string)args[n + i * 5 + 1],
										(string)args[n + i * 5 + 2],
										i,  //sit
                                        0,	//gold
										(int)args[n + i * 5 + 3],	//sex
										MainRoot._gRoomData.cCurRoomData.vRoomSetting[13]	//difen
										);
                                        if (args.Length>25)
                                        {
                                            MainRoot._gGameRoomCenter.gGameRoom.SetTablePlayerIp((int)args[n + i * 5], (string)args[22 + i]);
                                        }
										tempa = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[n + i * 5]);
										if (tempa!=null)
										{
											Debug.Log("Unity:" + "加入时准备状态：" + ((int)args[n + i * 5]).ToString() + "," + ((int)args[n + i * 5 + 4]).ToString() + "," + (4 + i % 4).ToString());
											continue;
										}
									}
								}
								else
								{
									ncou8ntuser++;
									isUserMax = false;
								}
							}
							if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom() && ncou8ntuser == 2)   //如果二人麻将，则两人挤满
							{
								isUserMax = true;
							}
							for (int t = 0; t < MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Count; t++)
							{
								if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[t] != null)
								{
									MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[t].InitialLinkPanel();    //初始化人物面板
									if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[t].nUserId != MainRoot._gPlayerData.nUserId)
									{
										MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[t].SetReadyState(((int)args[n + t * 5 + 4]) == 2);
									}
									else
									{
										if (((int)args[n + t * 5 + 4]) != 2)
										{
											MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[t].SetReadyState(false);
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowReadyBtn(isUserMax);
										}
										else
										{
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowReadyBtn(false);//如果人已经满了，而且服务端状态是为准备，则显示准备按钮，关闭邀请好友按钮
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveInvitaBt(false);
											MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList[t].SetReadyState(true);
										}
									}
								}
							}
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMPersonJiaRu();
						}
						break;
					}
				case 0x8485466C://C_AnsPlayerQuitCardRoom CRC32HashCode   //有玩家退出房卡房间
					{
                        int n = 2;
                        byte countPlayer = 0;
						PlayerBase tempa;
						bool isDel = true;
						for (int i = 0; i < 4; i++)
                        {
							isDel = true;;
							tempa = MainRoot._gGameRoomCenter.gGameRoom.GetUserByServerSit(i);
							for (int t = 0; t < 4; t++)
							{
								if (tempa!=null && (int)args[n + t * 3] == tempa.nUserId)
								{
									isDel = false;
									break;
								}
							}
                            if (isDel) //是空座位
							{
                                //清空已经离开的玩家.
                                if (tempa != null)
                                {
									tempa.IsYiZhunBei = false;
									MainRoot._gGameRoomCenter.gGameRoom.ResetOneInfoPanelPos(tempa.nUIUserSit);
									//删除已准备或准备中消息.
									tempa.CloseAllReadyMsgTip();
									if (tempa.pInfoPanel.pDetail != null)
                                    {
                                        tempa.pInfoPanel.pDetail.DestroyThis();
                                        tempa.pInfoPanel.pDetail = null;
                                    }

                                    if (tempa.pInfoPanel != null)
                                    {
                                        tempa.pInfoPanel.DestroyThis();
                                        tempa.pInfoPanel = null;
                                    }
									tempa.DestroyThis();
								}
							}
                            else
							{
                                countPlayer++;
                                tempa.SetReadyState(false);
                            }
                        }

                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowReadyBtn(false);
                        if (countPlayer < 1) {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMPaiJuJieSan();
                        }
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMPersonLiKai();
						break;
					}
				case 0xEB581999://C_AnsPlayerReadyNextGame CRC32HashCode  //房卡玩家准备下一局
					{
						int nSit = -1;
						PlayerBase pBase = null;
						for (int i = 2; i < 10; i += 2)
						{
							Debug.Log("Unity:" + "下一局时准备状态：" + ((int)args[i ]).ToString() + "," + ((int)args[i + 1]).ToString() + "," + i.ToString());
							//nSit = MainRoot._gGameRoomCenter.gGameRoom.GetUserUISitByUserID((int)args[i]);
							pBase = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i]);
							pBase.SetReadyState((int)args[i + 1]==2);
							pBase.SetXiaPaoImg(-1 * i);
						}
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gReadyBtn.SetActive((!gGameRoom.pMainPlayer.IsYiZhunBei));
                        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong12, true);
                    }
					break;
				case 0x27E918BA://C_AnsPlayerReadyGameStart CRC32HashCode   //第一局的时候有玩家准备房卡房间
					{
						int nSit = -1;
                        PlayerBase pBase = null;
                        for (int i = 2; i < 10; i+=2)
						{
							Debug.Log("Unity:" + "第一局的时候有玩家准备房卡房间：" + ((int)args[i]).ToString() + "," + ((int)args[i+ 1]).ToString() + "," + (i).ToString());
							if ((int)args[i]!=-1)
							{
								if ((int)args[i+1]==2)//已准备
								{
									//删除准备中，显示已准备
									nSit = MainRoot._gGameRoomCenter.gGameRoom.GetUserUISitByUserID((int)args[i]);
                                    pBase = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[i]);
									pBase.SetReadyState(true);
								}
							}
						}
                        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong12, true);
                    }
					break;
				case 0xD7453B78://C_AnsPlayerCurrentTableData CRC32HashCode   //返回获取房间数据的消息
                    {
                        //初始玩家信息
                        if (MainRoot._gGameRoomCenter.gGameRoom != null)
                        {
                            PlayerBase[] tempplay = new PlayerBase[4];
                            MainRoot._gGameRoomCenter.gGameRoom.SetZhuangJiaUserId = (int)args[34];
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(1);
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中

                            for (int i = 0; i < 4; i += 1)
                            {
                                Debug.LogWarning("Unity: 0xD7453B78 coin " + (int)args[38 + (i * 5)] + ", id " + (int)args[34 + (i * 5)]);
                                if ((int)args[34 + i * 5] == MainRoot._gPlayerData.nUserId)
                                {
                                    tempplay[0] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(MainRoot._gPlayerData.nUserId);
                                    if (tempplay[0] != null)    //修正加入金币房间的自己的部分数据
                                    {
                                        tempplay[0].Initial(
                                            (string)args[35 + i * 5],
                                            (int)args[34 + i * 5],
                                            i,
                                            (int)args[38 + i * 5],
                                            MainRoot._gRoomData.cCurRoomData.nDiFen,
                                            (int)args[37 + i * 5],
                                            (string)args[36 + i * 5]
                                            );
                                    }
                                    else
                                    {
                                        MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
                                            (int)args[34 + i * 5],
                                            (string)args[35 + i * 5],
                                            (string)args[36 + i * 5],
                                            i,
                                            (int)args[38 + i * 5],
                                            (int)args[37 + i * 5],
                                            MainRoot._gRoomData.cCurRoomData.nDiFen
                                            );
                                    }
                                }
                            }
                            for (int i = 0; i < 4; i += 1)
                            {
								if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
								{
									if ((int)args[34 + i * 5] == 0)
										continue;
								}
                                if ((int)args[34 + i * 5] != MainRoot._gPlayerData.nUserId)
                                {
                                    MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
                                        (int)args[34 + i * 5],
                                        (string)args[35 + i * 5],
                                        (string)args[36 + i * 5],
                                        i,
                                        (int)args[38 + i * 5],
                                        (int)args[37 + i * 5],
                                        MainRoot._gRoomData.cCurRoomData.nDiFen//MainRoot._gRoomData.cGoldRoomSetting.vRoomSetting[MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel * 3 + 2]
                                        );
                                }
                                tempplay[i] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[34 + i * 5]);
                                if (tempplay[i] == null)
                                {
                                    continue;
                                }
                            }
                            PlayerBase player;
                            for (int t = 0; t < 4; t++)
                            {
								if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
								{
									if ((int)args[34 + t * 5] == 0)
										continue;
								}
								tempplay[t].InitialLinkPanel();    //初始化人物面板
                                tempplay[t].SetZhuangjia((MainRoot._gGameRoomCenter.gGameRoom.GetZhuangJiaUserId == tempplay[t].nUserId));
                                player = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(tempplay[t].nUserId);
                                if (player)
                                {
									if (!MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())  //正常房卡麻将  
									{
										player.SetXiaPaoImg(System.Convert.ToInt32(args[54 + t]));
									}
									else //二人麻将，显示下注数
									{
										if (player.pInfoPanel)
										{
											player.pInfoPanel.ShowPlayerCoinNumInfo(System.Convert.ToInt32(args[54 + t]));
										}
									}
                                }
                                if (args.Length > 63)
                                {
                                    MainRoot._gGameRoomCenter.gGameRoom.SetTablePlayerIp((int)args[34 + t * 5], (string)args[60 + t]);
                                }
                            }

                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveInvitaBt(false);
                            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan ||
                                MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                            {
                                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                                {
                                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveExitBt(false);
                                }
                            }
                            else
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ChatBtObj.SetActive(true);
                            }

                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShengYuPaiShuObj.SetActive(true);
                            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.JieSanBtObj.SetActive(true);
                                //房卡房局数设置
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[58]);
                            }
                            else
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(1);//删除匹配中.
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中
                            }

                            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                            {//淘汰赛局数设置
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[58]);
                            }
                        }
                        MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
                        MainRoot._pMJGameTable.PlayerNetTableOperation(MJGameTable.SMSG_TABLE.RELINK, args);
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowCurOperatePlayerEff((int)args[5]);//头像转圈圈
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.GetChiPengHuTiShi().ShowChiPengHuTips(((byte)args[7] & CMD_SXMJ.WIK_CHI_HU) == CMD_SXMJ.WIK_CHI_HU, ((byte)args[7] & CMD_SXMJ.WIK_GANG) == CMD_SXMJ.WIK_GANG, ((byte)args[7] & CMD_SXMJ.WIK_PENG) == CMD_SXMJ.WIK_PENG, false);
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowTuoGuanUI((int)args[59] == 2);

                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ClearXiaPaoZhong();//下炮完毕
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaPaoUI != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaPaoUI.OnSysMsgWaitEventHappen(3);//下炮界面关闭
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaPaoUI = null;
						}
						break;
                    }
                case 0x3519FA42://C_AnsPlayerCurrentTableDataFail CRC32HashCode   //返回断线重连后 获取房间数据失败的消息
                    {
						PlayerBase[] tempplay = new PlayerBase[4];
						if (MainRoot._gGameRoomCenter.gGameRoom != null)
                        {//理论上金币房进不来

							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetReadyAndInvitationFalse();

							for (int i = 0; i < 4; i += 1)
                            {
								if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
								{
									if ((int)args[3 + i * 6] == 0)
									{
										continue;
									}
								}
								if ((int)args[3 + i * 6]!=-1)
								{
									if ((int)args[3 + i * 6] == MainRoot._gPlayerData.nUserId)
									{
										if (MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Exists(delegate (PlayerBase m) { return (m.playertype == PLAYERTYPE.MAIN_USER); }) != false)
										{
											MainRoot._gGameRoomCenter.gGameRoom.pMainPlayer.Initial(
												(string)args[4 + i * 6],
												(int)args[3 + i * 6],
												i,
												(int)args[7 + i * 6],
												0,
												(int)args[6 + i * 6],
												(string)args[5 + i * 6]
												);
										}
										else
										{
											MainRoot._gGameRoomCenter.gGameRoom.InitialMainPlayer(
												(int)args[3 + i * 6],
												(string)args[4 + i * 6],
												(string)args[5 + i * 6],
												i,
												(int)args[7 + i * 6],
												(int)args[6 + i * 6],
												0//MainRoot._gRoomData.cGoldRoomSetting.vRoomSetting[MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel * 3 + 2]
												);
										}
									}
									else
									{
										MainRoot._gGameRoomCenter.gGameRoom.InitialOneOtherPlayer(
											(int)args[3 + i * 6],
											(string)args[4 + i * 6],
											(string)args[5 + i * 6],
											i,
											(int)args[7 + i * 6],
											(int)args[6 + i * 6],
											0//MainRoot._gRoomData.cGoldRoomSetting.vRoomSetting[MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel * 3 + 2]
											);
									}
									tempplay[i] = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[3 + i * 6]);
                                    if (args.Length > 32)
                                    {
                                        MainRoot._gGameRoomCenter.gGameRoom.SetTablePlayerIp((int)args[3 + i * 6], (string)args[29 + i]);
                                    }
                                }
                            }
                            for (int t = 0; t < 4; t++)
                            {
								if (tempplay[t]!=null)
								{
		                           tempplay[t].InitialLinkPanel();    //初始化人物面板
								}
                            }
                            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
                            {//房卡房局数设置
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[28]);
                            }
                            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                            {//淘汰赛局数设置
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowFangKaRoomJuShuInfo((ushort)args[28]);
                            }
                            if ((int)args[2] == 3)
							{//房间是准备状态
								for (int t = 0; t < 4; t++)
								{
									if (tempplay[t] != null)
									{
										tempplay[t].IsYiZhunBei = ((int)args[8 + t * 6] == 4);
										tempplay[t].SetReadyState(tempplay[t].IsYiZhunBei);
									}
								}
								if (!gGameRoom.pMainPlayer.IsYiZhunBei)
								{
									MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gReadyBtn.SetActive(true);
								}
								if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
								{//房卡房中间等待开始状态
									MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYINGWAIT;
								}
							}
							else if ((int)args[2] == 4)
							{//下炮状态
                                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
                                {
                                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(1);//删除匹配中.
                                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中
                                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(gGameRoom.pMainPlayer.nUIUserSit + 5);//删除准备中
									MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(gGameRoom.pMainPlayer.nUIUserSit + 29);//删除已准备
									MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(49, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);
								}
                                MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.PLAYING;//下炮就是开始状态
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.TuoGuanBtObj.SetActive(true);
								MainRoot._pMJGameTable.SetBankerByPlayerID(new int[] { (int)args[3], (int)args[9], (int)args[15], (int)args[21] }, (int)args[27]);
                                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
                                {
									if (!MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
									{
										if (System.Convert.ToInt32(MainRoot._gRoomData.cCurRoomData.vRoomSetting[4]) == 5)//自由下炮类型
										{
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaPaoUI();//自己下炮
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(9, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(10, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(11, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
										}
									}
									else
									{
										MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaZhuUI();
									}

                                }
							}
						}
						break;
                    }
                case 0x12F1E88://C_AnsPlayerStrandGame CRC32HashCode  //玩家登录 滞留游戏信息
                    {
                        if (MainRoot._gPlayerData)
                        {
                            MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[2], (int)args[3], (int)args[4]);
                        }
                        if (MainRoot._gPlayerData.relinkUserRoomData != null)
                        {//说明玩家有未完成的牌局

                            if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {//点击请求重新连入牌局
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentRoomInfo((int)MainRoot._gPlayerData.relinkUserRoomData.roomType, MainRoot._gPlayerData.relinkUserRoomData.roomID);
                            }
                        }
                        else
                        {//踢出游戏场景
                            MainRoot._gMainRoot.ChangeScene("GameHall");
                        }
                        break;
                    }
                case 0x4A372289://C_AnsPlayerCurrentRoomInfo CRC32HashCode    //发送当前牌局信息
                    {//请求房间信息返回成功，请处理界面显示
					 //args[2]-房间id
					 //args[3]-房间类型
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
						}
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
                            //MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");


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
                            //MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");

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
                            MainRoot._gRoomData.cCacheRoomData.sRoomSetting = "";
                            MainRoot._gRoomData.cCacheRoomData.bIsErRenMjRoom = false;
                            //分解房间设置信息
                            string[] vset = MainRoot._gRoomData.cCacheRoomData.sRoomSetting.Split(":".ToCharArray());
                            MainRoot._gRoomData.cCacheRoomData.vRoomSetting = new int[vset.Length];
                            for (int i = 1; i < vset.Length; i++)
                            {
                                MainRoot._gRoomData.cCacheRoomData.vRoomSetting[i - 1] = System.Convert.ToInt32(vset[i]);
                            }

                            MainRoot._gRoomData.cCacheRoomData.nMaxRound = 8;//最大局数
                                                                                                                                                                                                         //MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong");


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
                case 0x7AC2B4F7://C_AnsPlayerAskDissolutionCardRoom CRC32HashCode    //玩家申请退出 
                    {
                        int pId = (int)args[2];
                        Debug.Log("Unity:"+"pid is " + pId + ", curPlayerId is " + MainRoot._gPlayerData.nUserId);
                        if (MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(pId) != null)
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnJieSanTouPiao(args);
                        }
                        break;
                    }
                case 0xE4EA55DF://C_AnsPlayerAnsDissolutionCardRoom CRC32HashCode  //玩家同意退出
                    {
                        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null && MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom != null)
                        {
                            Debug.Log("Unity:" + "jieSan room state update!");
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom.UpdateJieSanFriendRoom(args);
                        }
                        break;
                    }
                case 0x6523BAF3://C_AnsPlayerAllGameEnd CRC32HashCode	//房卡房间总局数已满，进行高级结算
                    {
						if (MainRoot._gPlayerData.relinkUserRoomData != null)
						{
							MainRoot._gPlayerData.relinkUserRoomData = null; //清除当前牌局信息.
						}

						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
                        {
							if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
							{
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
							}
							if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom != null)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSanFriendRoom.DestroyThis();
                            }
							if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaZhuUI != null)
							{
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaZhuUI.DestroyThis();
								MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pXiaZhuUI = null;
							}
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnGaoJiJieSuanObj(args);
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
                            else if ((int)args[4] == (int)OneRoomData.RoomType.RoomType_RoomCard)
                            {
                                MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], 0);
                            }
                            else if ((int)args[4] == (int)OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan || (int)args[4] == (int)OneRoomData.RoomType.RoomType_MultRace_HaiXuan)
                            {
                                MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], 1);
                            }
                            else if ((int)args[4] == (int)OneRoomData.RoomType.RoomType_ThemeRace_Group || (int)args[4] == (int)OneRoomData.RoomType.RoomType_MultRace_Group)
                            {
                                MainRoot._gPlayerData.InitPlayerRelinkRoomInfo((int)args[4], (int)args[3], 0);
                            }
                        }
                        if (MainRoot._gPlayerData.relinkUserRoomData != null)
                        {//说明玩家有未完成的牌局
                            //if (!(bool)args[2])
                            //{//直接请求复盘
                                if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                                {//点击请求重新连入牌局
                                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCurrentRoomInfo((int)MainRoot._gPlayerData.relinkUserRoomData.roomType, MainRoot._gPlayerData.relinkUserRoomData.roomID);
                                }
                            //}
                            //else
                            //{
                            //    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnLoginPaiJuWeiWanChengDlg();//点击请求重新连入牌局
                            //}
                        }
                        return;
                    }
                case 0x1B8D30F4://c_AnsPlayerReadyFail CRC32HashCode    //金币房准备失败，金币不满足条件
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnPlayerClickNextGameFromGoldRoom(args);
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
                case 0x7A139A77://C_AnsPlayerMatchingfailed CRC32HashCode //匹配失败的消息， 谁先给处理一下
                    {
                        Debug.Log("Unity: Player matching failed!");
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnPiPeiShiBaiTuiChuDlg();
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
                case 0x9C096062://C_AnsRoomOwnerDissolutionCardRoom CRC32HashCode //房主解散房间
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnFriendRoomJieSan(); //当房主解散了好友房,在牌局未开始前(服务器返回消息时调用).

                        return;
                    }
				case 0x5C6AE2FD://C_AnsSetPlayerTrusteeState CRC32HashCode    //设置玩家的托管状态
					{
						for (int i = 0; i < 4; i++)
						{
							if ((int)args[2 + i * 2] == MainRoot._gPlayerData.nUserId)
							{
								if ((int)args[2 + i * 2 + 1] == 2)  //自己进入托管状态
								{
									MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowTuoGuanUI(true);
								}
								if ((int)args[2 + i * 2 + 1] == 0)  //自己退出托管状态
								{
									MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowTuoGuanUI(false);
								}
							}
						}
						return;
					}
                case 0xB638A6C5://C_NetCallPlayerSendEmotion CRC32HashCode    //发表情
                    {
                        PlayerBase tempa = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[1]);
                        if (tempa != null)
                        {
                            tempa.pInfoPanel.ShowEmotion((int)args[2]);
                        }
                        return;
                    }
				case 0x692C5020://C_AnsPlayerQuitGoldRoom CRC32HashCode   //有玩家退出金币房间
					{
						int n = 2;
						bool isDel = true;
						PlayerBase tempa;
						for (int i = 0; i < 4; i++)
						{
							isDel = true;
							tempa = MainRoot._gGameRoomCenter.gGameRoom.GetUserByServerSit(i);
							for (int t = 0; t < 4; t++)
							{
								if (tempa!=null && (int)args[n + t * 3] == tempa.nUserId)	//如果人还在
								{
									isDel = false;
									break;
								}
							}
							if (isDel) //是空座位
							{
								//清空已经离开的玩家.
								if (tempa != null)
								{
									Debug.Log("Unity:" + "清空已经离开的玩家:" + tempa.nUserSit.ToString() + "/" + tempa.nUserId.ToString());
									tempa.IsYiZhunBei = false;
									MainRoot._gGameRoomCenter.gGameRoom.ResetOneInfoPanelPos(tempa.nUIUserSit);
									//删除已准备或准备中消息.
									tempa.CloseAllReadyMsgTip();
									if (tempa.pInfoPanel.pDetail != null)
									{
										tempa.pInfoPanel.pDetail.DestroyThis();
										tempa.pInfoPanel.pDetail = null;
									}

									if (tempa.pInfoPanel != null)
									{
										tempa.pInfoPanel.DestroyThis();
										tempa.pInfoPanel = null;
									}
									tempa.DestroyThis();
								}
							}
						}

						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMPersonLiKai();
						break;
					}
                case 0x13123362://C_CallPlayerNoMoneyForMatch CRC32HashCode   准备金币房间，但条件不足匹配
                    {//(int)args[2] 0钱不够 1钱太多
                        if ((int)args[2] == 0)
                        {//钱不够了
                            if (MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel == (int)ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01)
                            {
                                if (!MainRoot._gPlayerData.GetCanBringJiuJiCoin())
                                {
                                    MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01);
                                }
                                else {
                                    MainRoot._gUIModule.pUnModalUIControl.SpawnFuLiJinDlg();
                                }
                            }
                            else
                            {
                                MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg((ShopCoinDlgCtrl.ShopCoinEnum)MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel);
                            }
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveStartGameBt(true);
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(29);//删除自己已准备
                        }
                        else
                        {
                            MainRoot._gUIModule.pUnModalUIControl.SpawnJinBiChaoChuDlg((MoleMole.QinRenSelectUIView.SelectRoomEnum)((int)MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel+1));
                        }
                        break;
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
						Application.Quit();
						return;
					}
				case 0x521CDD29://C_CallPlayerServerClose 游戏开始维护
					{
						RoomCardNetClientModule.netModule.ClosePlayerLinkClient();
						GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-GameWH-QJKXX"),  MainRoot._gUIModule.pMainCanvas.transform, false);
						EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
						dlg.Initial(EnsureDlg.EnsureKind.Game_WEIHU_Dlg);
						return;
					}
				case 0x55CB9860://C_AnsPlayerLoginReturn CRC32HashCode  //玩家登陆后面
                    if (args.Length>=21 && MainRoot._gRoomData != null && MainRoot._gRoomData.cCurRoomData != null)
                    {
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold && (int)args[20] != 5)
                        {
                            if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {
                                RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                                MainRoot._gMainRoot.ChangeScene("GameHall");//退出金币房间
                            }
                        }
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard && (int)args[20] != 6)
                        {
                            if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {
                                //RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gRoomData.cCurRoomData.nRoomId);
                                MainRoot._gMainRoot.ChangeScene("GameHall");//退出房卡房间
                            }
                        }
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan && (int)args[20] != 10)
                        {
                            if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {
                                RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLeaveGoldRoom(MainRoot._gPlayerData.nUserId);
                                MainRoot._gMainRoot.ChangeScene("GameHall");//退出金币房间
                            }
                        }
                    }
                    break;
				case 0x2C1395E://C_AnsGoldPlayerReadyNextGame CRC32HashCode  //金币房间 玩家准备下一局
					{
						if ((int)args[1] == MainRoot._gRoomData.cCurRoomData.nRoomId)
						{
							int n = 2;
							PlayerBase tempa;
							for (int i = 0; i < 4; i++)
							{
								if ((int)args[n + i * 2] != -1) //不是空座位
								{
									tempa = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)args[n + i * 2]);
									if (tempa != null) 
									{
										if ((int)args[n + i * 2 + 1] == 2) //显示已准备,删除准备中
										{
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(tempa.nUIUserSit + 29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //显示已准备.
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(tempa.nUIUserSit + 5);//删除准备中
										}
										else
										{
											if (tempa.nUIUserSit != 0)//自己不显示准备中
											{
												MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(tempa.nUIUserSit + 5, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //显示准备中.
											}
											MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(tempa.nUIUserSit + 29);//删除已准备
										}

									}
									else
									{
										Debug.LogError(string.Format("发生了奇怪的事情,{0},{1}", ((int)args[n + i * 2]).ToString(), ((int)args[n + i * 2 + 1]).ToString()));
									}
								}
							}
						}
                        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong12, true);
                        break;
					}
				case 0xC06F544B://C_CallInitialTipsMessage CRC32HashCode 初始化客户端系统消息
					{
						int n = 0;
						object[] args1 = new object[args.Length];
						for (int i = 3; i < args.Length; i += 3)
						{
							if ((int)args[i + 2] != 2)//如果是公告
							{
								args1[n] = args[i];
								args1[n + 1] = args[i + 1];
								args1[n + 2] = (int)args[i + 2] == 0 ? false : true;
								n += 3;
							}
							else //专题比赛的活动详情
							{
								MainRoot._gPlayerData.sThemeRace_HuoDongXiangQing = (string)args[i];
							}
						}
						object[] args2 = new object[n];
						System.Array.Copy(args1, args2, n);
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pTipsMsgText != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pTipsMsgText.SetTipsMsgTextByServerPort(args2);
						}
						break;
					}
				case 0x4F631BC://C_CallPlayerSendGlobalChatMessage_toClient 玩家发送世界广播
					{
						if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pTipsMsgText != null)
						{
							MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pTipsMsgText.SetTipsMsgText((string)args[4],(string)args[5],(float)args[6], (bool)args[7]);
						}
						break;
					}
				default:
                    break;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Unity:"+e.ToString());
        }
    }
}
