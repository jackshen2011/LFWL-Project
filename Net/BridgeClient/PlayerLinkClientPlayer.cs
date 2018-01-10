using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FTLibrary.Net;
using UnityEngine;
using Mahjong;

namespace RoomCardNet
{
    class PlayerLinkClientPlayer : PublicClientPlayer
    {
        public PlayerLinkClient m_pParentClientObj = null;

        //当前用户的ID(这个ID是在数据库用户表里获得的， 唯一编号)
        public int m_PlayerID;

        //标记当前用户是否已经登陆过 如果登陆过 断线重连后 在登陆一遍
        public static bool m_IsPlayerLogin = false;
        //标记当前登陆的是
        public static bool m_IsWeChatPlayer = true;

        //记录当前登陆的用户
        public static string m_sName = "";
        public static string m_spassword = "";

        //测试索引
        //public RoomCardNetClientModule m_testModule = null;

        public PlayerLinkClientPlayer(PlayerLinkClient parent)
            :base()
        {
            m_pParentClientObj = parent;
            m_pParentClientObj.m_pBridgeClientPlayerObj = this;
        }
        public override void OnInitialize()
        {
            Debug.Log("Unity:" + "net Init OK !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            //Net_CallNetTest();
            string ss = DateTime.Now.Minute.ToString() + ":";
            ss += DateTime.Now.Second.ToString() + ":";
            ss += DateTime.Now.Millisecond.ToString() + ":";
            UnityEngine.Debug.Log("Unity:" + "timer  net ok =======================:" + ss);
            UnityEngine.Debug.Log("Unity:" + "timer  net ok count=======================:" + RoomCardNetClientModule.testdebug.ToString());
            
            try
            {
                //如果玩家登陆过 则表示断线重连
                if (MainRoot._gPlayerData.isLoginSucceed == true)
                {
                    if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
                    {
                        Net_CallPlayerLogin_Audit(m_sName, m_spassword);
                    }
                    //有微信的授权
                    else if (m_IsWeChatPlayer == true)
                    {
                        Net_CallWeChatPlayerLogin(MainRoot._gPlayerData.sWeChat_Unionid, MainRoot._gPlayerData.sWeChat_Openid, MainRoot._gPlayerData.sUserName, MainRoot._gPlayerData.nSex, MainRoot._gPlayerData.sHeadImgUrl);
                    }
                    else
                    {
                        //内部登录入口，继续
                        Net_CallPlayerLogin(m_sName, m_spassword);
                    }
                }
                else
                {
                    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDengLuLianJieChengGong();
                    }
                }
            }
            catch
            {
                //授权失效，退出到游戏主界面
                if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.OnQuit_Click();
                } 
            }
            
		}
        public override FuntionRet OnCallNetFuntion(uint nFunId, object[] args)
        {
            try
            {
                Debug.Log("Unity:"+"---------------msg Retrun --------------------==" + nFunId.ToString("X"));

                switch (nFunId)
                {
					case 0xFE2F02F8://C_CheckClientVersion 客户端版本号对比检查
						{
							C_CheckClientVersion(args);

							return FuntionRet.FuntionRet_OK;
						}

					case 0x55CB9860://C_AnsPlayerLoginReturn CRC32HashCode  //玩家登陆后面
                        {
							Debug.Log("Unity:"+args.ToString());
                            IAPInterface.lockIAPVerify = false;
                            bool isLoginTemp = false;
                            if(MainRoot._gPlayerData != null)
                            {
                                isLoginTemp = MainRoot._gPlayerData.isLoginSucceed;
                            }
                            Net_AnsPlayerLoginReturn(args);
                            Net_SendSceneTypeForServer(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("ShaanxiMahjong")?1:0);//登录成功将当前场景发送给服务端
                            MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);
                            PlayerLinkClientPlayer.m_IsPlayerLogin = true;

                            //表示之前登录过 是断线重连状态
                            if(isLoginTemp == true)
                            {
                                if (MainRoot._gUIModule.pUnModalUIControl != null)
                                {
                                    MainRoot._gUIModule.pUnModalUIControl.SpawnSMWangLuoChongXinLianJie();
                                }
                            }
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0xD2C44DB9://Net_AnsPlayerLoginReturnFailed_Audit CRC32HashCode    ////审核玩家登录注册失败的消息
                        {
							//下标3的参数是返回错误码：0用户不存在 2密码错（提示用户名密码错，用于登录界面）， 1用户已存在 用于注册界面
							MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);
							return FuntionRet.FuntionRet_OK;
                        }
                    case 0xC3C57B47://C_AnsCloseExceptionPlayer CRC32HashCode //关闭异常用户
                        {
                            MainRoot._gPlayerData.IsExitAbnormalPlayer = true;
                            Net_AnsCloseExceptionPlayer();
							MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);
							return FuntionRet.FuntionRet_OK;
                        }
                    case 0xAD2B2DA2://C_AnsPlayerGameEnd CRC32HashCode// 一局结束 结算
                        {
                            //Net_AnsCommonMahjongNetMessage(nFunId, args);
                            if ((int)args[28] == 0)//CMD_SXMJ.INVALID_CHAIR)  //流局
                            {
                                nFunId = 15;
                            }
                            MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0xF9CE85AB://C_AnsPlayerOperateResult CRC32HashCode //其他用户操作 包括 吃 碰 杠
                        {
                            Net_AnsCommonMahjongNetMessage(nFunId, args);
                            if (((byte)(args[4]) & CMD_SXMJ.WIK_GANG) == CMD_SXMJ.WIK_GANG)
                            {
                                MainRoot._gMainRoot.OnNetCallByServer(10, args);//直杠牌
                            }
                            if (((byte)(args[4]) & CMD_SXMJ.WIK_PENG) == CMD_SXMJ.WIK_PENG)
                            {
                                MainRoot._gMainRoot.OnNetCallByServer(9, args);//碰牌
                            }
                            
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0x5738E7C8://C_AnsPlayerSendCardData CRC32HashCode //发牌
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中
                            MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);//发牌
                            if (((byte)(args[4]) & CMD_SXMJ.WIK_GANG) == CMD_SXMJ.WIK_GANG || ((byte)(args[4]) & CMD_SXMJ.WIK_CHI_HU) == CMD_SXMJ.WIK_CHI_HU)
                            {
                                MainRoot._gMainRoot.OnNetCallByServer(0xE1E04923, args);//提示，暗杠或续杠牌或自摸
                            }
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0x50BFA852://C_AnsSendGameStart CRC32HashCode  //游戏开始
                    case 0x2EF741A7://C_AnsSendCardRoomGameStart CRC32HashCode    //发送房卡房间开始游戏的消息
                    case 0xDC2A3C19://C_AnsPlayerInitCardData CRC32HashCode 游戏开始给玩家发牌
                    case 0xE03BFA1F://C_AnsPlayerOutCardData CRC32HashCode //用户出牌
                    //case 0x5D50A1C4://C_AnsPlayerChiHuOrZha CRC32HashCode //用户操作 胡牌
                    case 0xE1E04923://C_AnsPlyaerNotify CRC32HashCode //给用户的提示
                    case 0x424B68D://C_AnsPlayerChangeCoin CRC32HashCode  //游戏内玩家金币的增减显示 这里发送总量客户端自行处理增减积分
                    case 0xB1A6BBC5://C_AnsCreateCardRoomFailed CRC32HashCode //返回创建房卡失败的消息
                    case 0x5D5E830E://C_AnsJoinCardRoomFailed CRC32HashCode 加入房卡房间失败
                    case 0xA90A82DE://C_AnsOneselfJoinCardRoom CRC32HashCode //自己加入房卡房间
                    case 0x8CB85B3E://C_AnsPlayerJoinCardRoom CRC32HashCode   //有玩家加入房卡房间
                    case 0x8485466C://C_AnsPlayerQuitCardRoom CRC32HashCode   //有玩家退出房卡房间
                    case 0x692C5020://C_AnsPlayerQuitGoldRoom CRC32HashCode   //有玩家退出金币房间
                    case 0xC2C24BF2://C_CallGetPlayerCardRoomList CRC32HashCode //取玩家历史列表信息
                    case 0xEB581999://C_AnsPlayerReadyNextGame CRC32HashCode  //玩家准备下一局
                    case 0x3519FA42://C_AnsPlayerCurrentTableDataFail CRC32HashCode   //返回断线重连后 获取房间数据失败的消息，
                    case 0xD7453B78://C_AnsPlayerCurrentTableData CRC32HashCode   //返回断线重连后 获取房间数据的消息，
                    case 0x7AC2B4F7://C_AnsPlayerAskDissolutionCardRoom CRC32HashCode    //玩家申请退出 
                    case 0xE4EA55DF://C_AnsPlayerAnsDissolutionCardRoom CRC32HashCode  //玩家同意退出
                    case 0x27E918BA://C_AnsPlayerReadyGameStart CRC32HashCode   //第一局的时候有玩家准备房卡房间
                    case 0x12F1E88://C_AnsPlayerStrandGame CRC32HashCode  //玩家登录 滞留游戏信息
                    case 0x4A372289://C_AnsPlayerCurrentRoomInfo CRC32HashCode    //发送当前牌局信息
                    case 0xB68573B://C_AnsPlayerCurrentRoomInfoFailed CRC32HashCode    //发送当前牌局信息 没有找到房间
					case 0x6523BAF3://C_AnsPlayerAllGameEnd CRC32HashCode	//房卡房间总局数已满，进行高级结算
                    case 0x4CEB16F9://C_AnsPlayerCurrentRunRoomInfo CRC32HashCode    //发送当前牌局信息
                    case 0x1B8D30F4://c_AnsPlayerReadyFail CRC32HashCode    //金币房准备失败，金币不满足条件
                    case 0x7A139A77://C_AnsPlayerMatchingfailed CRC32HashCode //匹配失败的消息， 谁先给处理一下
                    case 0xC565E781://C_AnsUpdatePlayerCardNum CRC32HashCode //更新玩家的房卡.
                    case 0x8826D045://C_AnsGetRecordListOk CRC32HashCode  //获取战绩历史列表信息
                    case 0x712E3BE2://C_CallPlayerGetRecordTotal CRC32HashCode    //返回战绩总场次信息
                    case 0x9C096062://C_AnsRoomOwnerDissolutionCardRoom CRC32HashCode //房主解散房间
					case 0xB638A6C5://C_NetCallPlayerSendEmotion CRC32HashCode    //发表情
					case 0x5C6AE2FD://C_AnsSetPlayerTrusteeState CRC32HashCode    //设置玩家的托管状态
                    case 0x13123362://C_CallPlayerNoMoneyForMatch CRC32HashCode   准备金币房间，但条件不足匹配
                    case 0x44174EE5://C_CallPlayerRefreshCoin CRC32HashCode   //直接刷新金币适用未在游戏中
					case 0x84511863://C_CallPlayerGoToMainUIView CRC32HashCode	//服务端强制用户返回主界面 GM操作
					case 0x817AD282://C_CallPlayerGoToOut CRC32HashCode		//服务端强制用户下线 GM操作
                    case 0xA5CCEE9A://C_CallPlayerGoToRestartGame CRC32HashCode	//服务端强制用户重新启动游戏  GM操作
                    case 0x521CDD29://C_CallPlayerServerClose 游戏开始维护 GM操作
                    case 0x2C1395E://C_AnsGoldPlayerReadyNextGame CRC32HashCode  //金币房间 玩家准备下一局
                    case 0xF782BBA9://C_AnsPlayerSetBuff CRC32HashCode   //修改玩家buff信息返回
					case 0xC06F544B://C_CallInitialTipsMessage CRC32HashCode 初始化客户端系统消息 GM操作
					case 0x4F631BC://C_CallPlayerSendGlobalChatMessage_toClient 玩家发送世界广播
					case 0xEF65FD46://C_GetHaiXuanChartsList CRC32HashCode 获取海选排行榜
                    case 0xABD91C9A://C_GetMultRaceHaiXuanChartsList CRC32HashCode 获取 社区 海选排行榜
                    case 0xC7A4348A://Net_CallCheckInputCodeOk CRC32HashCode //短信验证返回
                    case 0xF2EC866B://C_AnsPlayerAddThemeRace_HaiXuan_Failed CRC32HashCode    //玩家加入华商海选房失败消息
                    case 0x6B38587://C_AnsThemeRace_RaceState CRC32HashCode    //玩家请求专题赛状态信息 返回
                    case 0xD62E05FB://C_AnsMultRace_RaceState CRC32HashCode    //玩家请求 社区 赛状态信息 返回
                    case 0xBFEC4FB0://C_AnsThemeRace_GroupRaceInfo CRC32HashCode CRC32HashCode 玩家专题赛小组赛  返回所有晋级玩家信息列表
                    case 0x3D0C6D97://C_AnsMultRace_GroupRaceInfo CRC32HashCode 玩家 社区赛小组赛  返回玩家信息列表
                    case 0xF338D622://C_AnsPlayerJoinThemeRace_GroupRoom_ReturnState CRC32HashCode   //玩家加入专题小组赛 返回状态
                    case 0x75AAD1C5://C_AnsPlayerJoinMultRace_GroupRoom_ReturnState CRC32HashCode   //玩家加入 社区 小组赛 返回状态
                    case 0x1C064E43://C_AnsSendThemeRace_GroupRoomGameStart CRC32HashCode //发送主题赛-小组赛开始游戏的消息 
                    case 0x2688719A://C_AnsSendMultRace_GroupRoomGameStart CRC32HashCode //发送社区 赛 淘汰赛房间开始游戏的消息
                    case 0x35EE041E://C_AnsThemeRace_GroupRaceInfo_Final CRC32HashCode 玩家专题赛小组赛 决赛  返回玩家信息列表
                    case 0xA088C322://C_AnsMultRace_GroupRaceInfo_Final CRC32HashCode 玩家社区 赛小组赛 决赛  返回玩家信息列表
                    case 0x50150D4D://C_AnsPlayerLoginReturn_Second CRC32HashCode 玩家登陆的时候 二次给的玩家数据
                    case 0x6DC8CD9B://C_AnsCircleBingo CRC32HashCode  //转盘返回
                    case 0x29701AD6://C_AnsPlayerPayOKReturn CRC32HashCode //玩家充值成功后返回的消息
                    case 0xAF8327FF://C_AnsPlayerJoinOfficialRoomReturn CRC32HashCode   //玩家加入系统红包赛返回
                    case 0x4982D2DD://C_AnsOfficialRoomListInfo CRC32HashCode //返回红包赛的信息
                        {
                            Net_AnsCommonMahjongNetMessage(nFunId, args);

                            MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0xC9D8E039://C_CallPlayerJiuJiOk CRC32HashCode
                        {
                            //Net_AnsCommonMahjongNetMessage(nFunId, args);
                            MainRoot._gMainRoot.OnNetCallByServer(nFunId, args);
                            //MainRoot._gPlayerData.PlayerCoin=(int)args[2];
                            //MainRoot._gPlayerData.SetPlayerJiuJiFlag((int)args[2],(DateTime)args[3]);
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0x6BEB5382://C_CallPlayerPayOk CRC32HashCode //玩家充值返回
                        {
                            Net_AnsGetPrepayOk(args);
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0xEF06FB8D://S_CallPlayerIAPRequestOk CRC32HashCode //玩家请求发起iap返回OK
                        {
                            Net_AnsIAPRequestOk(args);
                            return FuntionRet.FuntionRet_OK;
                        }
                    case 0x2CC830F6://Net_AnsVerifyIAPOk CRC32HashCode //验证IAP完成
                        {
                            Net_AnsVerifyIAPOk(args);
                            return FuntionRet.FuntionRet_OK;
                        }
                }
                return base.OnCallNetFuntion(nFunId, args);
            }
            catch (System.Exception ex)
            {
                ERR(ex.ToString());
            }
            return FuntionRet.FuntionRet_ERR;
        }

        //连接测试
        public void Net_CallNetTest()
        {
            //case 0xA011D748://S_CallNetTest CRC32HashCode
            Tos(0xA011D748, "test_C");
        }

		//////////////////////////////网络消息出发处理///////////////////////////////////////////
		/*
         * 
         * 消息的出发指的是：消息从客户端出发 到达最终处理结果之前的过程， 
         * 比如说是玩家打了一张牌 到达游戏服务处理：从 从客户端 到达桥接服务 到达游戏服务的过程都认为是出发过程
         *
         */
		///////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// 用户通过微信授权，进行登录
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public void Net_CallWeChatPlayerLogin(string sWeChatUnionId, string sWeChatOpenId,string sWeChatName,int sex,string sWeChatHeadUrl)
		{
			//case 0xCA6EDF94://S_CallWeChatPlayerLogin CRC32HashCode
			Debug.Log("Unity:"+"---------------------Net_CallWeChatPlayerLogin == ");
			object[] args = new object[6];
			args[0] = sWeChatUnionId;
			args[1] = sWeChatOpenId;
			args[2] = sWeChatName;
			args[3] = sex;
			args[4] = sWeChatHeadUrl;
            args[5] = SystemSetManage.ChannelsNumber;
            Tos(0xCA6EDF94, args);
            m_IsWeChatPlayer = true;

        }
		//用户登录
		public void Net_CallPlayerLogin(string userName, string password)
        {
            //case 0x9C793E9E://S_CallPlayerLogin CRC32HashCode
            Debug.Log("Unity:"+"---------------------Net_CallPlayerLogin == ");
            Tos(0x9C793E9E, userName, password);
            m_IsWeChatPlayer = false;
            m_sName = userName;
            m_spassword = password;
        }

        //审核用户登录
        public void Net_CallPlayerLogin_Audit(string userName, string password)
        {
            if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
            {
                Debug.Log("Unity:" + "---------------------Net_CallPlayerLogin == " + userName + "===" + password);
                //case 0x2A2E3B7://S_CallPlayerLogin_Audit CRC32HashCode  //审核用户登录
                Tos(0x2A2E3B7, userName, password);
                m_IsWeChatPlayer = false;
                m_sName = userName;
                m_spassword = password;

            }
        }

        //审核用户注册
        public void Net_CallPlayerRegister_Audit(string userName, string password)
        {
            if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
            {
                Debug.Log("Unity:" + "---------------------Net_CallPlayerRegister == " + userName + "===" + password);
                //case 0x4FBEA59://S_CallPlayerRegister_Audit CRC32HashCode   //审核用户注册
                Tos(0x4FBEA59, userName, password);
                m_IsWeChatPlayer = false;
                m_sName = userName;
                m_spassword = password;
            }
        }

        //通知用户退出登陆
        public void Net_CallPlayerQuitLogin()
        {
            //case 0x8100A439://S_CallPlayerQuitLogin CRC32HashCode     //玩家退出登陆
            Debug.Log("用户退出登陆");
            Tos(0x8100A439);
        }

        //用户加入金币房间
        public void Net_CallPlayerAddGoldRoom(int goldRoomLevel)
        {
            //case 0xCA9EB005://S_CallPlayerAddGoldRoom CRC32HashCode
            Debug.Log("Unity:"+"---------------------Net_CallPlayerAddGoldRoom == " + goldRoomLevel);
            Tos(0xCA9EB005, goldRoomLevel);
        }
        public void Net_CallPlayerSendPhoneMessage(string sPhone)
        {
            //case 0xEEB71923://Net_CallSendPhoneCode CRC32HashCode //发送验证码
            Tos(0xEEB71923, sPhone);
        }
        public void Net_CallPlayerCheckCode(string code)
        {
            //case 0x25CBF1A7://Net_CallCheckInputCode CRC32HashCode //检查验证码是否正确
            Tos(0x25CBF1A7, code);
        }
        public void Net_CallSetUserSheQuCode(string code)
        {
            //case 0x329E8CBE://S_SetUserSheQUCode CRC32HashCode 用户第一次填写社区代码
            //一个参数，社区代码，字符串，不超20字符。
            Tos(0x329E8CBE, code);
        }
        //领取救济金
        public void Net_CallPlayerJiuJiu()
        {
            //case 0xF7592DDC://S_CallPlayerJiuJiCoin CRC32HashCode   //玩家领取救济金
            Tos(0xF7592DDC);
        }
		//一局游戏结束后， 玩家点击下一局，准备开始游戏
        public void Net_CallPlayerReadyGame(bool isCardRoom)
        {
            //case 0xF7B3532://S_CallPlayerReadyGame CRC32HashCode   //玩家准备消息 用于一局结束后，玩家准备下一句
            Tos(0xF7B3532, isCardRoom);
        }
        //玩家准备OK 返回消息
        //stakeIndex 玩家赌注的索引
        public void Net_CallGameStartReadyOK(byte stakeIndex)
        {
            object[] objList = new object[1];
            objList[0] = (object)stakeIndex;
            Debug.Log("Unity:"+"---------------------Net_CallGameStartReadyOK == " + stakeIndex);
            //case 0xC66FA83A://S_CallGameStartReadyOK CRC32HashCode  //玩家准备OK
            Tos(0xC66FA83A, objList);
        }
        //玩家打牌消息
        // card 打的牌的类型码值
        public void Net_CallUserOutCard(byte card)
        {
            object[] objList = new object[1];
            objList[0] = (object)card;
            //Debug.Log("Unity:"+"---------------------Net_CallUserOutCard == " + card);
            //case 0x7B1EEE7E://S_CallUserOutCard CRC32HashCode   //玩家打牌消息
            Tos(0x7B1EEE7E, objList);
        }
        //玩家操作消息：吃 碰 杠 过 胡
        //OperateCode 操作的类型
        //OperateCard 操作的牌码
        public void Net_CallUserOperateCard(byte OperateCode, byte OperateCard)
        {
            object[] objList = new object[2];
            objList[0] = (object)OperateCode;
            objList[1] = (object)OperateCard;
            Debug.Log("Unity:"+"---------------------Net_CallUserOperateCard == " + OperateCode + "  =====  " + OperateCard);
            //case 0x72E08EBB://S_CallUserOperateCard CRC32HashCode //玩家操作的消息
            Tos(0x72E08EBB, objList);
        }



        //玩家创建房卡房间
        public void Net_CallPlayerCreateCardRoom(object[] var_Option)
        {
            //case 0xE110896E://S_CallPlayerCreateCardRoom CRC32HashCode  //用户创建房卡房间
            object[] param = new object[var_Option.Length + 1];
            for (int i = 0; i < var_Option.Length; i++)
            {
                param[i] = var_Option[i];
            }
            param[var_Option.Length] = false;
            Tos(0xE110896E, param);
		}
		//玩家创建二人麻将房间
		public void Net_CallPlayerCreateErRenRoom(object[] var_Option)
		{
			object[] param = new object[16];

			param[0] = 0;
			param[1] = -1;
			param[2] = 0;
			param[3] = -1;
			param[4] = 0;
			param[5] = 0;

			param[6] = 1;
			param[7] = 0;
			param[8] = 0;
			param[9] = -1;
			param[10] = 0;
			param[11] = 0;
			param[12] = 0;

			param[13] = MainRoot._gPlayerData.sUserName;
            
            param[14] = (object)MainRoot._gPlayerData.sUserName + "的麻将馆";


            param[15] = true;
            //case 0xE110896E://S_CallPlayerCreateCardRoom CRC32HashCode  //用户创建二人房间
            Tos(0xE110896E, param);
        }
        //二人麻将 玩家下注
        public void Net_CallPlayerStake_TwoHuman(int StakeValue)
        {
            //case 0x457FD4E5://S_CallPlayerStake_TwoHuman CRC32HashCode  //用于二人麻将的下注
            Tos(0x457FD4E5, StakeValue);
        }

        //请求加入历史
        public void Net_CallGetPlayerCardRoomList()
        {
            //case 0x9CB6E0A7://S_CallGetPlayerCardRoomList CRC32HashCode //请求玩家房卡房房间列表
            Tos(0x9CB6E0A7);
        }
		//用户退出
        public void Net_CallPlayerQuit()
		{
            //case 0x3115AF3F://S_CallPlayerQuit CRC32HashCode
            Tos(0x3115AF3F);
        }
        public void Net_CallPlayerGetRecordList(int roomKind)
        {
            //case 0x378CAAB6://C_CallGetRecordList CRC32HashCode //取战绩历史列表
            Tos(0x378CAAB6, roomKind);
        }
        public void Net_CallPlayerGetRecordTotal(int roomKind, byte time)
        {//time 0-一周 1-一月 2-三月 其他-不限
            //case 0xDB7C609A://S_CallPlayerGetRecordTotal CRC32HashCode  //取分时总战绩
            Tos(0xDB7C609A, roomKind, time);
        }
        //玩家加入房卡房间
        public void Net_CallPlayerJoinCardRoom(int RoomID)
        {
            //case 0x634283EF://C_CallPlayerJoinCardRoom CRC32HashCode 玩家加入房卡房间
            Tos(0x634283EF, RoomID);
        }

        //玩家离开房卡房间
        public void Net_CallPlayerQuitCardRoom(int RoomID)
        {
            //case 0x6B7F9EBD://C_CallPlayerQuitCardRoom CRC32HashCode 玩家离开房卡房间
            Tos(0x6B7F9EBD, RoomID);
        }

        //房卡房间的准备开始 仅限第一局调用
        public void Net_CallPlayerReadyCardRoom(int RoomID)
        {
            //case 0x14DDFCA8://C_AnsPlayerReadyCardRoom CRC32HashCode 玩家准备房卡房间
            Tos(0x14DDFCA8, RoomID);
        }
        //玩家掉线后如果在游戏中， 请求当前房间里面的信息
        public void Net_CallPlayerCurrentRoomInfo(int roomType, int roomID)
        {
            //RoomType_Gold, 1 
            //RoomType_RoomCard, 2
            //RoomType_ThemeRace_HaiXuan 4,  //华商专场的海选比赛
            //RoomType_ThemeRace_Group 5,    //华商专场的小组赛
            //RoomType_MyRoom 6,        //自建比赛
            //RoomType_OfficialRoom 7,  //官方比赛
            //RoomType_MultRace_HaiXuan,  //社区的海选比赛
            //RoomType_MultRace_Group,    //社区的小组赛
            if (roomType == 1 || roomType == 2 || roomType == 4 || roomType == 5 || roomType == 6 || roomType == 7 || roomType == 8 || roomType == 9 )
            {
                //case 0x8C818221://S_CallPlayerCurrentRoomInfo CRC32HashCode //请求当前房间的信息
                Tos(0x8C818221, roomType, roomID);
            }

        }
        //发送当前场景类型于服务器0-主UI场景 1-麻将桌场景
        public void Net_SendSceneTypeForServer(int sceneType)
        {
            //case 0x98159284://S_CallSceneTypeForServer CRC32HashCode  //发送玩家所在场景
            Tos(0x98159284, sceneType);
        }
        //玩家复盘调用 当前桌子上的数据
        public void Net_CallPlayerCurrentTableData(int roomType, int roomID)
        {
            //case 0xF8EA602://S_CallPlayerCurrentTableData CRC32HashCode 玩家请求当前局的数据
            Tos(0xF8EA602, roomType, roomID);
        }
        //房主解散未开始的房间
        public void Net_CallRoomOwnerDissolutionCardRoom(int roomType, int roomID)
        {
            //case 0x3DF0A0EB://S_CallRoomOwnerDissolutionCardRoom CRC32HashCode //房主解散未开始的房间
            Tos(0x3DF0A0EB, roomType, roomID);
        }
        
        /// <summary>
        /// 玩家请求系统红包赛的信息
        /// </summary>
        public void Net_CallOfficialRoomListInfo()
        {
            //case 0xF3FBC8E7://S_CallOfficialRoomListInfo CRC32HashCode    //玩家请求系统红包赛的信息
            Tos(0xF3FBC8E7);
        }


        /// <summary>
        /// 向服务端请求秦人比赛场(红包赛/官方比赛)数据.
        /// </summary>
        public void Net_CallQinRenBiSaiChangDlgDt(int biSaiId)
        {
            //case 0x6C8F7679://S_CallPlayerJoinOfficialRoom CRC32HashCode    //玩家请求加入系统红包赛
            Tos(0x6C8F7679, biSaiId);
        }

        /// <summary>
        /// 玩家确认退出金币房间，进入托管
        /// </summary>
        /// <param name="RoomID"></param>
        public void Net_CallPlayerLeaveGoldRoom(int nUserId)
		{
			Tos(0x9D31EAD1, nUserId); //case 0x9D31EAD1://S_CallUserStandOut CRC32HashCode //玩家点退出
		}
		/// <summary>
		/// 玩家主动开启金币房间托管
		/// </summary>
		/// <param name="nUserId"></param>
		public void Net_CallPlayerStartGoldRoomTuoGuan(int nUserId,bool trustee)
		{
			Tos(0xCE88A2B9, trustee);  //case 0xCE88A2B9://S_CallUserDeposit CRC32HashCode //玩家点托管
		}
        /// <summary>
        ///玩家申请解散房卡房间
        /// </summary>
        public void Net_CallPlayerAskDissolutionCardRoom(int RoomID)
        {
            //case 0xDB3B747E://S_CallPlayerAskDissolutionCardRoom CRC32HashCode 玩家申请解散房卡房间
            Tos(0xDB3B747E);
        }
        //玩家应答解散房间
        // 房号  是否同意
        public void Net_CallPlayerAnsDissolutionCardRoom(int RoomID, bool IsAgree)
        {
            //case 0x45139556://S_CallPlayerAnsDissolutionCardRoom CRC32HashCode 玩家应答解散房间
            Tos(0x45139556, RoomID, IsAgree);
        }
        public void Net_CallPlayerSendEmotion(int roomType,int roomID,int emotion)
        {
            //case 0xFE8D1096://S_CallPlayerSendEmotion CRC32HashCode //请求发表情
            Tos(0xFE8D1096,roomType,roomID,emotion);
        }
		/// <summary>
		/// 玩家发送世界广播
		/// </summary>
		public void Net_CallPlayerSendGlobalChatMessage(string msg)
		{
			//case 0xBE6F2E73://S_CallPlayerSendGlobalChatMessage 玩家发送世界广播
			Tos(0xBE6F2E73, msg);
		}
		/// <summary>
		/// 用户请求领取车展礼包
		/// </summary>
		public void Net_CallPlayerGetCarGifts()
		{
            //case 0x540C8D15://S_CallPlayerGetStartGift CRC32HashCode //用户请求领取车展礼包
            Tos(0x540C8D15);
		}
#region 商城相关操作
		/// <summary>
		/// 充值物品
		/// </summary>
		/// <param name="goodsKind">买的东西标记</param>
		/// <param name="channel">支付频道0微信1支付宝</param>
		public void Net_CallPlayerPayRequest(int goodsKind, CommonLibrary.PayDefine.PayChannel channel)
        {
            //if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
            //{
            //    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
            //    {
            //        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(46, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform);
            //        return;
            //    }
            //    else if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
            //    {
            //        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(46, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);
            //        return;
            //    }
            //    else
            //    {
            //        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
            //        {
            //            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(46, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);
            //            return;
            //        }
            //    }
            //}
            //else
            //{
            //    //case 0xE7C3E0BB://S_CallPlayerPayRequest CRC32HashCode    请求生成订单
            //    Tos(0xE7C3E0BB, goodsKind, (int)channel);
            //}
            //case 0xE7C3E0BB://S_CallPlayerPayRequest CRC32HashCode    请求生成订单
            Tos(0xE7C3E0BB, goodsKind, (int)channel);
            if (MainRoot._gUIModule.pUnModalUIControl != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.SpawnSMShangChengGouMaiDengDai();
            }
        }
        /// <summary>
        /// 服务端返回订单
        /// </summary>
        /// <param name="args"></param>
        public void Net_AnsGetPrepayOk(object[] args)
        {
            if (MainRoot._gUIModule.pUnModalUIControl != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.DeleteSMShangChengGouMaiDengDai();
            }
            GameCenterEviroment.currentGameCenterEviroment.OpenPlayerPayMoney(0, (string)args[3], PlayerPayOver);
            /*using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    jo.Call<bool>("OpenPlayerPayMoney", 0, (string)args[3]);
                }
            }*/
        }
        public void PlayerPayOver(int a,bool success)
        {
            if (success)
            {

            }
        }
        bool iapRequest=false;
        /// <summary>
        /// 充值物品
        /// </summary>
        /// <param name="goodsKind">买的东西标记</param>
        /// <param name="channel">支付频道0微信1支付宝</param>
        public void Net_CallPlayerIAPRequest(int goodsKind, CommonLibrary.PayDefine.PayChannel channel)
        {
#if UNITY_IPHONE
            if (iapRequest)
            {//同时只能发起一次支付
                return;
            }
            if (MainRoot._gIAPManager.isPaying())
	        {//正在请求支付
                return;
	        }
            //case 0x58B0B147://S_CallPlayerIAPRequest CRC32HashCode 请求发起IAP
            Tos(0x58B0B147, goodsKind, (int)channel);
            iapRequest = true;
#endif //UNITY_IPHONE
        }
        void Net_AnsIAPRequestOk(object[] args)
        {
            iapRequest = false;
            if (args.Length<4)
            {
                return;
            }
            MainRoot._gIAPManager.Pay((string)args[3],0);
        }
        public void Net_CallVerifyIAP(string payID,string receiptData)
        {//请求验证iap合法并发物品
            //case 0x79C73C5E://Net_CallVerifyIAP CRC32HashCode //请求iap验证
            Tos(0x79C73C5E, payID, receiptData);
        }
        void Net_AnsVerifyIAPOk(object[] args)
        {//验证完成
            if (args.Length<5)
            {
                return;
            }
            MainRoot._gIAPManager.endTransaction((string)args[3]);
            int error = (int)args[4];
            Debug.Log("IOS支付验证错误码："+error);
        }
#endregion
		/// <summary>
		/// 获取海选排行榜
		/// </summary>
		public void GetHaiXuanChartsList(OneRoomData.DataDefine_MultiRaceType multiRaceType)
		{
            switch (multiRaceType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        //0x4692B707://S_GetHaiXuanChartsList 获取海选排行榜
                        Tos(0x4692B707);
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        //0xFBFC86BE://S_GetMultHaiXuanChartsList CRC32HashCode 获取 社区 海选排行榜
                        Tos(0xFBFC86BE);
                        break;
                    }
            }
        }

        /// <summary>
        /// 向服务端请求进入海选赛.
        /// </summary>
        public void Net_CallHaiXuanSaiClickStartBiSaiBt(OneRoomData.DataDefine_MultiRaceType multiRaceType)
        {
            switch (multiRaceType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        //0x43AFE4A2://S_CallThemeRace_HaiXuan_JoinRoom CRC32HashCode //向服务端请求进入海选赛
                        Tos(0x43AFE4A2);
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        //0xB9652D14://S_CallMultRace_HaiXuan_JoinRoom CRC32HashCode  //向服务端请求进入 社区 海选赛
                        Tos(0xB9652D14);
                        break;
                    }
            }
        }

        /// <summary>
        /// 向服务端请求进入淘汰赛/总决赛.
        /// biSaiType: 0 淘汰赛, 1 总决赛.
        /// </summary>
        public void Net_CallTaoTaiSaiClickStartBiSaiBt(int biSaiType, OneRoomData.DataDefine_MultiRaceType multiRaceType)
        {
            switch (multiRaceType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        //0x3128E447://S_CallPlayerJoinThemeRace_GroupRoom CRC32HashCode //玩家向服务端请求进入淘汰赛/总决赛.
                        Tos(0x3128E447, biSaiType);
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        //0x4C49790://S_CallPlayerJoinMultRace_GroupRoom CRC32HashCode  //玩家向服务端请求进入 社区 小组赛 //社区
                        Tos(0x4C49790, biSaiType);
                        break;
                    }
            }
        }

        /// <summary>
        /// 向服务端请求比赛房间面板主题赛/社区麻将赛按键的消息.
        /// 需要返回打开主题赛的面板状态arg: 0 海选赛, 1 淘汰赛, 2 总决赛.
        /// </summary>
        public void Net_CallBiSaiRoomZhuTiSaiBt(OneRoomData.DataDefine_MultiRaceType multiRaceType)
        {
            switch (multiRaceType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        //0xAF9FBA73://S_CallThemeRace_RaceState CRC32HashCode //向服务端请求专题赛的状态信息
                        Tos(0xAF9FBA73);
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        //0x46BFA217://S_CallMultRace_RaceState CRC32HashCode  //向服务端请求 社区 专题赛的状态信息
                        Tos(0x46BFA217);
                        break;
                    }
            }
        }

        /// <summary>
        /// 向服务端请求华商大转盘指针停留的索引.
        /// 需要服务端返回转盘指针停留的索引[0, 11].
        /// </summary>
        public void Net_CallHuaShangZhuanPan_GetPointIndex()
        {
            //case 0x99EC527C://S_CallHuaShangZhuanPan_PointIndex CRC32HashCode //向服务端请求华商大转盘指针停留的索引
            Tos(0x99EC527C);
        }
        //////////////////////////////网络消息返回处理/////////////////////////////////////////////////
        /*
		 * 
		 * 消息的返回指的是：最终处理结果处返回到客户端的过程
		 * 比如说是  游戏服务处理后 发送给桥接 在转发给玩家
		 *
		 */
        ///////////////////////////////////////////////////////////////////////////////////////////////////


        //客户端版本号对比检查
        public void C_CheckClientVersion(object[] args)
		{
			string sVer = (string)args[0];
			string surl = (string)args[1];
            if(args.Length > 6)
            {
                //下标2~5是之前版本用的 屏蔽掉， 不用了
                string[] list = new string[args.Length-6];
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = (string)args[i+6];
                }
                SystemSetManage.SetGameVersion(list);
                //args[2] = RoomCardRootModule.IsOpenIOSAudit;
                //args[3] = RoomCardRootModule.IOSAuditVersionStr;
                //args[4] = RoomCardRootModule.IsOpenMyAppAudit;
                //args[5] = RoomCardRootModule.MyAppAuditVersionStr;
            }
            if(SystemSetManage.currentVersion != null)
            {
                if(SystemSetManage.currentVersion.UpdateOnOff == true)
                {
                    GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-BanBenGengXin"), MainRoot._gUIModule.pMainCanvas.transform, false);
                    EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
#if !UNITY_EDITOR && UNITY_ANDROID
                    dlg.Initial(EnsureDlg.EnsureKind.BanBenGengXin, SystemSetManage.currentVersion.DownloadURL);
#elif UNITY_IPHONE
                    dlg.Initial(EnsureDlg.EnsureKind.BanBenGengXin, SystemSetManage.currentVersion.DownloadURL_IOS);
#endif //UNITY_ANDROID

                }
                else
                {
					if (MainRoot._gPlayerData == null || !MainRoot._gPlayerData.isLoginSucceed)
					{
						MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.InitialLoginAndRegUI(true);
					}
				}
            }
		    else if (SystemSetManage.ClientVersion != sVer)
			{
				GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-BanBenGengXin"), MainRoot._gUIModule.pMainCanvas.transform, false);
				EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
				dlg.Initial(EnsureDlg.EnsureKind.BanBenGengXin, surl);
			}
		}


		//返回玩家登陆消息
		public void Net_AnsPlayerLoginReturn(object[] args)
        {
            m_PlayerID = (int)args[2];
            string name = (string)args[3];
            //string password = (string)args[2];
            bool isLoginSucceed = (bool)args[11];
            string ss = "玩家ID:" + m_PlayerID.ToString();
            Debug.Log("Unity:"+"---------------palyer  login succeed ---------------------------------");
            //m_testModule.OneselfPrintTRACE(ss);
        }
        //关闭异常用户，重复登录的客户端
        public void Net_AnsCloseExceptionPlayer()
        {
            Debug.Log("---------------Close Exception Player---------------------------------");
            RoomCardNeDevice.IsExceptionPlayer = true;
            RoomCardNetClientModule.netModule.ClosePlayerLinkClient();
            MainRoot._gUIModule.pUnModalUIControl.SpawnQiTaDengLuDlg();
        }

        //玩家的牌
        public List<byte> m_PlayerCardList = new List<byte>();

        //处理麻将房间内的打牌消息响应
        public void Net_AnsCommonMahjongNetMessage(uint nFunId, object[] args)
		{
            //消息类型定义
            //case 0xDC2A3C19://C_AnsPlayerInitCardData CRC32HashCode 游戏开始给玩家发牌
            //case 0x5738E7C8://C_AnsPlayerSendCardData CRC32HashCode //发牌
            //case 0xE03BFA1F://C_AnsPlayerOutCardData CRC32HashCode //用户出牌
            //case 0xF9CE85AB://C_AnsPlayerOperateResult CRC32HashCode //用户操作 包括 吃 碰 杠
            //case 0x5D50A1C4://C_AnsPlayerChiHuOrZha CRC32HashCode //用户操作 胡牌
            //case 0xE1E04923://C_AnsPlyaerNotify CRC32HashCode //给用户的提示
            //case 0x424B68D://C_AnsPlayerChangeCoin CRC32HashCode  //游戏内玩家金币的增减显示 这里发送总量客户端自行处理增减积分
            //case 0xB1A6BBC5://C_AnsCreateCardRoomFailed CRC32HashCode //返回创建房卡失败的消息
            //case 0x5D5E830E://C_AnsJoinCardRoomFailed CRC32HashCode 加入房卡房间失败
            //case 0x8CB85B3E://C_AnsPlayerJoinCardRoom CRC32HashCode   //有玩家加入房间
            //case 0xC2C24BF2://C_CallGetPlayerCardRoomList CRC32HashCode //取玩家历史列表信息
            //case 0xA90A82DE://C_AnsOneselfJoinCardRoom CRC32HashCode //自己加入房卡房间
            //case 0x6523BAF3://C_AnsPlayerAllGameEnd CRC32HashCode	//房卡房间总局数已满，进行高级结算
            //case 0x8826D045://C_AnsGetRecordListOk CRC32HashCode  //获取战绩历史列表信息
            //case 0x712E3BE2://C_CallPlayerGetRecordTotal CRC32HashCode    //返回战绩总场次信息
            //case 0x6BEB5382://C_CallPlayerPayOk CRC32HashCode //玩家充值返回
            //case 0xB638A6C5://C_NetCallPlayerSendEmotion CRC32HashCode    //发表情
			//case 0xEF65FD46://C_GetHaiXuanChartsList CRC32HashCode 获取海选排行榜
            try
            {
                switch (nFunId)
                {
                    case 0x50BFA852://C_AnsSendGameStart CRC32HashCode  //游戏开始
                        {
                            // case 0xC66FA83A://S_CallGameStartReadyOK CRC32HashCode  //玩家准备OK
                            String ss = "Game Start：Player1：";
                            ss += ((int)(args[2])).ToString();
                            ss += ":";
                            ss += ((int)(args[3])).ToString();
                            ss += "带入金币:";
                            ss += ((int)(args[4])).ToString();
                            ss += "Player2:";
                            ss += ((int)(args[5])).ToString();
                            ss += ":";
                            ss += ((int)(args[6])).ToString();
                            ss += "带入金币:";
                            ss += ((int)(args[7])).ToString();
                            ss += "Player3:";
                            ss += ((int)(args[8])).ToString();
                            ss += ":";
                            ss += ((int)(args[9])).ToString();
                            ss += "带入金币:";
                            ss += ((int)(args[10])).ToString();
                            ss += "Player4:";
                            ss += ((int)(args[11])).ToString();
                            ss += ":";
                            ss += ((int)(args[12])).ToString();
                            ss += "带入金币:";
                            ss += ((int)(args[13])).ToString();
                            ss += "BankerPalyer:";
                            ss += ((int)(args[14])).ToString();

                            //Debug.Log("Unity:"+ss);
                            //Program.ClientModulelist[i].PrintTRACE();
                            //TcpNetModule.TRACE(ss);
                            //m_testModule.OneselfPrintTRACE(ss);
                            return;
                        }
                    case 0xDC2A3C19://C_AnsPlayerInitCardData CRC32HashCode 游戏开始给玩家发牌
                        {

                            String ss = "其实发牌：塞子数：";
                            ss += ((int)(args[2])).ToString();
                            ss += "庄家ID:";
                            ss += ((int)(args[3])).ToString();
                            ss += "当前玩家ID";

                            ss += ((int)(args[4])).ToString();
                            ss += "用户操作:";
                            ss += ((byte)(args[5])).ToString();
                            ss += "牌:";
                            byte[] CardData = ((MemoryStream)(args[6])).ToArray();
                            System.Threading.Monitor.Enter(m_PlayerCardList);

                            for (int i=0; i<13; i++)
                            {
                                m_PlayerCardList.Add(CardData[i]);
                                ss = String.Format("{0}{1}:", ss, CardData[i]);
                            }
                            m_PlayerCardList.Sort();
                            System.Threading.Monitor.Exit(m_PlayerCardList);

                            ss += "剩余牌数";
                            ss += ((byte)(args[7])).ToString();
                            Debug.Log("Unity:"+ss);

                            ss = "玩家";
                            for (int index = 0; index < 4; index++)
                            {
                                ss += "ID:";
                                ss += ((int)(args[8 + index * 2])).ToString();
                                ss += "炮数";
                                ss += ((byte)(args[9 + index * 2])).ToString();

                            }
                            //Debug.Log("Unity:"+ss);
                            //m_testModule.OneselfPrintTRACE(ss);
                            return;
                        }
                    case 0x5738E7C8://C_AnsPlayerSendCardData CRC32HashCode //发牌
                        {
                            String ss = "发牌：接牌玩家ID:";
                            ss += ((int)(args[2])).ToString();
                            ss += "牌:";
                            ss += ((byte)(args[3])).ToString();
                            ss += "用户操作:";
                            ss += ((byte)(args[4])).ToString();
                            ss += "是否末尾";
                            ss += ((bool)(args[5])).ToString();

                            System.Threading.Monitor.Enter(m_PlayerCardList);
                            if ((int)(args[2]) == m_PlayerID && (byte)(args[3]) != 0)
                                m_PlayerCardList.Add((byte)(args[3]));
                            System.Threading.Monitor.Exit(m_PlayerCardList);
                            //Debug.Log("Unity:"+ss);
                            //m_testModule.OneselfPrintTRACE(ss);
                            return;
                        }
                    case 0xE03BFA1F://C_AnsPlayerOutCardData CRC32HashCode //用户出牌
                        {
                            String ss = "出牌：出牌玩家ID:";
                            ss += ((int)(args[2])).ToString();
                            ss += "牌:";
                            ss += ((byte)(args[3])).ToString();
                            System.Threading.Monitor.Enter(m_PlayerCardList);

                            if ((int)(args[2]) == m_PlayerID)
                                m_PlayerCardList.Remove((byte)(args[3]));

                            System.Threading.Monitor.Exit(m_PlayerCardList);
                            //Debug.Log("Unity:"+ss);
                            //m_testModule.OneselfPrintTRACE(ss);
                            return;
                        }
                    case 0xF9CE85AB://C_AnsPlayerOperateResult CRC32HashCode //用户操作 包括 吃 碰 杠
                        {
                            //MainRoot._pMJGameTable.PlayerDoXuGangAfter(0, MainRoot._pMJGameTable.GetRandomMaJiang());//某个玩家打牌
                            //
                            String ss = "碰杠：操作玩家ID:";
                            ss += ((int)(args[2])).ToString();
                            ss += "供应玩家ID:";
                            ss += ((int)(args[3])).ToString();
                            ss += "操作类型:";
                            ss += ((byte)(args[4])).ToString();
                            ss += "牌:";
                            ss += ((byte)(args[5])).ToString();
                            if ((int)(args[2]) == m_PlayerID)
                            {
                                System.Threading.Monitor.Enter(m_PlayerCardList);
                                if (((byte)(args[4]) & 0x08) == 0x08)
                                {
                                    m_PlayerCardList.Remove((byte)(args[5]));
                                    m_PlayerCardList.Remove((byte)(args[5]));
                                }
                                else if (((byte)(args[4]) & 0x10) == 0x10)
                                {
                                    m_PlayerCardList.Remove((byte)(args[5]));
                                    m_PlayerCardList.Remove((byte)(args[5]));
                                    m_PlayerCardList.Remove((byte)(args[5]));
                                }

                                System.Threading.Monitor.Exit(m_PlayerCardList);
                            }
                            //Debug.Log("Unity:"+ss);
                            //m_testModule.OneselfPrintTRACE(ss);
                            return;
                        }
                    case 0x5D50A1C4://C_AnsPlayerChiHuOrZha CRC32HashCode //用户操作 胡牌
                        {
                            //
                            String ss = "胡：胡牌玩家ID:";
                            ss += ((int)(args[2])).ToString();
                            ss += "供应玩家ID:";
                            ss += ((byte)(args[3])).ToString();
                            ss += "牌:";
                            ss += ((byte)(args[4])).ToString();
                            ss += "扑克数量:";
                            ss += ((byte)(args[5])).ToString();
                            //m_testModule.OneselfPrintTRACE(ss);
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0xE1E04923://C_AnsPlyaerNotify CRC32HashCode //给用户的提示
                        {
                            //
                            String ss = "提示：提示玩家ID:";
                            ss += ((int)(args[2])).ToString();
                            ss += "牌:";
                            ss += ((byte)(args[3])).ToString();
                            ss += "操作:";
                            ss += ((byte)(args[4])).ToString();
                            //m_testModule.OneselfPrintTRACE(ss);
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0xAD2B2DA2://C_AnsPlayerGameEnd CRC32HashCode// 一局结束 结算
                        {
							try
							{
								String ss = "";
								for (int index = 0; index < 4; index++)
								{
									ss = "玩家的ID:";
									ss += ((int)(args[2 + index * 6])).ToString();
									ss += "牌数:";
									ss += ((byte)(args[3 + index * 6])).ToString();

									ss += "牌::::";
									byte[] CardData = ((MemoryStream)(args[4 + index * 6])).ToArray();
									for (int i = 0; i < CardData.Length; i++)
									{
										ss = String.Format("{0}{1}:", ss, CardData[i]);
									}

									//ss += "积分:";
									//ss += ((UInt32)(args[5 + index * 4])).ToString();
									//netData[3 + index * 4] = GameEnd.cbCardCount[index];
									//netData[4 + index * 4] = new MemoryStream(GameEnd.cbCardData[index]);
									//netData[5 + index * 4] = GameEnd.lGameScore[index];
									Debug.Log("Unity:"+ss);
									//m_testModule.OneselfPrintTRACE(ss);
								}

								ss = "胡牌玩家：供应ID:";
								ss += ((int)(args[26])).ToString();
								ss += "胡牌类型:";
								ss += ((UInt32)(args[27])).ToString();
								ss += "胡牌玩家ID:";
								ss += ((int)(args[28])).ToString();
								ss += "胡牌的牌:";
								ss += ((byte)(args[29])).ToString();
								//Debug.Log("Unity:"+ss);
							}
							catch (Exception)
							{

								throw;
							}

                            //m_testModule.OneselfPrintTRACE(ss);
                            return;
                        }
                    case 0x424B68D://C_AnsPlayerChangeCoin CRC32HashCode  //游戏内玩家金币的增减显示 这里发送总量客户端自行处理增减积分
                        {
                            String ss = "";
                            for (int index = 0; index < 4; index++)
                            {
                                ss = "ID:";
                                ss += ((int)(args[2 + index * 2])).ToString();
                                ss += "总分:";
                                ss += ((int)(args[3 + index * 2])).ToString();
                                //Debug.Log("Unity:"+ss);
                            }
                            return;
                        }
                    case 0xB1A6BBC5://C_AnsCreateCardRoomFailed CRC32HashCode //返回创建房卡失败的消息
                        {
                            String ss = "";
                            ss += "创建房卡失败消息(0是没有钱，1是房间建满了):";
                            ss += ((int)(args[3])).ToString();
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0x5D5E830E://C_AnsJoinCardRoomFailed CRC32HashCode 加入房卡房间失败
                        {
                            String ss = "";
                            ss += "加入房卡失败消息(0是没有房间，1是房间人满了):";
                            ss += ((int)(args[3])).ToString();
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0x8CB85B3E://C_AnsPlayerJoinCardRoom CRC32HashCode   //有玩家加入房间
                        {
                            String ss = "";
                            ss += "房间ID:";
                            ss += ((int)(args[1])).ToString();
                            ss += "房间类型:";
                            ss += ((int)(args[2])).ToString();
                            ss += "房间名字:";
                            ss += args[3];
                            ss += "创建者者名字:";
                            ss += args[4];
                            ss += "房间设置:";
                            ss += args[5];
                            ss += "有玩家加入房间:";
                            for (int index = 0; index < 4; index++)
                            {
                                ss += String.Format("{0}{1}:", ss, ((int)(args[6 + index * 4])).ToString());
                                ss += String.Format("{0}{1}:", ss, ((string)(args[7 + index * 4])).ToString());
                                ss += String.Format("{0}{1}:", ss, ((string)(args[8 + index * 4])).ToString());
                                ss += String.Format("{0}{1}:", ss, ((string)(args[9 + index * 4])).ToString());
                            }
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0xC2C24BF2://C_CallGetPlayerCardRoomList CRC32HashCode //取玩家历史列表信息
                        {
                            String ss = "";
                            ss = "历史房间数："+args[1];
                            for (int i = 0; i < (int)args[1]; i++)
                            {
                                ss += args[2 + i * 6];//房间名字
                                ss += args[3 + i * 6];//创建者姓名
                                ss += args[4 + i * 6];//房间id
                                ss += args[5 + i * 6];//状态
                                ss += args[6 + i * 6];//局数 底分等信息
                            }
                           // Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0x7AC2B4F7://C_AnsPlayerAskDissolutionCardRoom CRC32HashCode    //玩家申请退出
                        {
                            String ss = "";
                            ss = "玩家申请退出P1：" + args[1];
                            ss += "状态：";
                            ss += args[2];//房间名字
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0xE4EA55DF://C_AnsPlayerAnsDissolutionCardRoom CRC32HashCode  //玩家同意退出
                        {
                            String ss = "";
                            ss = "玩家同意退出：";
                            for (int i = 0; i < (int)args[1]; i++)
                            {
                                ss = String.Format("{0}{1}:", ss, ((int)(args[1 + i * 2])).ToString()); //玩家id.
                                ss = String.Format("{0}{1}:", ss, ((int)(args[2 + i * 2])).ToString()); //解散状态：0未选择，1同意，2拒绝.
                            }
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
                    case 0x12F1E88://C_AnsPlayerStrandGame CRC32HashCode  //玩家滞留游戏信息
                        {
                            String ss = "";
                            ss = "玩家滞留游戏：";
                            ss += "房间类型：";
                            ss += ((int)args[2]).ToString();//房间名字
                            ss += "房间ID：";
                            ss += ((int)args[3]).ToString();//房间名字
                            ss += "房间等级：";
                            ss += ((int)args[4]).ToString();//房间名字
                            //Debug.Log("Unity:"+ss);
                            return;
                        }
					case 0x6523BAF3://C_AnsPlayerAllGameEnd CRC32HashCode	//房卡房间总局数已满，进行高级结算
                        {
                            String ss = "";
                            ss = "高级结算：";
                            ss += "房主id：";
                            ss += ((int)args[2]).ToString();//房主id
                            ss += "房主昵称：";
                            ss += ((string)args[3]).ToString();//房主昵称
                            ss += "房间名称：";
                            ss += ((string)args[4]).ToString();//房间名称
                            ss += "总局数：";
                            ss += ((int)args[5]).ToString();//总局数
							ss += "当前局数：";
							ss += ((int)args[6]).ToString();//总局数
							for (int i = 0; i < 4; i++)
							{
                                ss = String.Format("id{0},昵称{1},胜局数{2},总分值{3}", ((int)(args[7 + i * 4])).ToString(), ((string)(args[8 + i * 4])).ToString(), ((int)(args[9 + i * 4])).ToString(), ((int)(args[10 + i * 4])).ToString());
                            }
                            return;
                        }
                    case 0x4CEB16F9://C_AnsPlayerCurrentRunRoomInfo CRC32HashCode    ////发送当前有玩家进行的牌局信息
                        {
                            String ss = "";
                            ss = "当前有玩家进行的牌局";
                            //Debug.Log("Unity:" + ss);
                            return;
                        }
                    case 0x1B8D30F4://c_AnsPlayerReadyFail CRC32HashCode    //金币房准备失败，金币不满足条件
                        {
                            String ss = "";
                            ss += "有玩家准备下一局失败:";
                            ss += "剩余金币：";
                            ss += (string)args[2];
                            ss += "失败原因：(1-太少，2-太多)";
                            ss += (string)args[3];
                            //Debug.Log("Unity:" + ss);
                            return;
                        }
                    case 0x8826D045://C_AnsGetRecordListOk CRC32HashCode  //获取战绩历史列表信息
                        {
                            String ss = "";
                            ss = "玩家战绩历史列表。房间类型：";
                            ss += ((int)args[3]).ToString();//
                            ss += "总分：";
                            ss += ((int)args[4]).ToString();//
                            ss += "总场数：";
                            ss += ((int)args[5]).ToString();//
                            ss += "胜场：";
                            ss += ((int)args[6]).ToString();//
                            ss += "返回条数：";
                            ss += ((int)args[7]).ToString();//
                            //Debug.Log("Unity:" + ss);
                            return;
                        }
                    case 0x712E3BE2://C_CallPlayerGetRecordTotal CRC32HashCode    //返回战绩总场次信息
                        {
                            String ss = "";
                            ss = "战绩分时数据。房间类型：";
                            ss += ((int)args[3]).ToString();//
                            ss += "总分：";
                            ss += ((int)args[4]).ToString();//
                            ss += "总场数：";
                            ss += ((int)args[5]).ToString();//
                            ss += "胜场：";
                            ss += ((int)args[6]).ToString();//
                            //Debug.Log("Unity:" + ss);
                            return;
                        }
                    case 0x6BEB5382://C_CallPlayerPayOk CRC32HashCode //玩家充值返回
                        {
                            String ss = "";
                            ss = "充值订单信息：";
                            ss += (string)args[3];//
                            Debug.Log("Unity:" + ss);
                            return;
                        }
                    case 0xB638A6C5://C_NetCallPlayerSendEmotion CRC32HashCode    //发表情
                        {
                            String ss = "";
                            ss = "收到表情：发送人";
                            ss += ((int)args[1]).ToString();//
                            ss += "表情";
                            ss += ((int)args[2]).ToString();//
                            Debug.Log("Unity:" + ss);
                            return;
                        }
                    case 0xF782BBA9://C_AnsPlayerSetBuff CRC32HashCode   //修改玩家buff信息返回
                        {
                            String ss = "";
                            ss += "玩家ID:";
                            ss += ((int)(args[1])).ToString();
                            ss += "buff:";
                            ss += ((string)(args[2])).ToString();
                            ss += "需要刷新的位:";
                            ss += ((string)(args[3])).ToString();
                            Debug.Log("Unity:" + ss);
                            return;
                        }
					case 0x4F631BC://C_CallPlayerSendGlobalChatMessage_toClient 玩家发送世界广播
						{
							String ss = "";
							ss += "玩家ID:";
							ss += ((int)(args[0])).ToString();
							ss += "昵称:";
							ss += ((string)(args[1])).ToString();
							ss += "消息:";
							ss += ((string)(args[2])).ToString();
							ss += "持续时间:";
							ss += ((float)(args[3])).ToString();
							ss += "秒，系统:";
							ss += ((bool)(args[4])).ToString();

							Debug.Log("Unity:" + ss);
							return;
						}
					case 0xEF65FD46://C_GetHaiXuanChartsList CRC32HashCode 获取海选排行榜
						{
							string st= "0xEF65FD46获取海选排行榜:";
							for (int i = 0; i < args.Length; i++)
							{
								st += args[i].ToString()+"/";
							}
							return;
						}
						return;
                    case 0xC7A4348A://Net_CallCheckInputCodeOk CRC32HashCode //短信验证返回
                        {
                            String ss = "";
                            ss += "玩家ID:";
                            ss += ((int)(args[1])).ToString();
                            ss += "短信验证错误码:";
                            ss += ((int)(args[3])).ToString();

                            Debug.Log("Unity:" + ss);
                            return;
                        }
                        return;
				}
				return;
            }
            catch (System.Exception ex)
            {
                ERR("Unity:"+ex.ToString());
            }
            return;

        }
    }
}
