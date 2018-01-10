using UnityEngine;
using System.Collections;
using System;
using cn.sharesdk.unity3d;
using RoomCardNet;
using System.Net;
using System.IO;

class PlayerData : MonoBehaviour {
    public enum BUFFBIN
    {
        BIN_ACTIVATIONCODE,//激活码
        BIN_ISFIRSTCHARGE,//是否使用过激活码
        BIN_ISGETSTARTGIFT//是否领取开服免费礼包
    }
    // Use this for initialization
    public int nUserId;
	public string sWeChat_Unionid = "";
	public string sWeChat_Openid = "";
    public string sUserName;
    /// <summary>
    /// 玩家性别: 1->男,2->女，0->女.
    /// </summary>
	public int nSex = 0;
    /// <summary>
    /// 关闭异常用户.
    /// </summary>
    public bool IsExitAbnormalPlayer;
    /// <summary>
    /// 动态下载玩家微信头像的控制对象.
    /// </summary>
    public AsyncImageDownload pAsyncImageDownload;
	/// <summary>
	/// 是否登录成功
	/// </summary>
   	public bool isLoginSucceed = false;
	/// <summary>
	/// 玩家微信头像的Url信息.
	/// </summary>
	public string sHeadImgUrl = "";
	public enum PlayerSexEnum
	{
		SEXNULL,
		BOY,
		GIRL
	}
	/// <summary>
    /// 微信用户是否授权
    /// </summary>
	public bool isUserAuthorized = false;
	/// <summary>
	/// 微信用户信息
	/// </summary>
	public Hashtable WeChatUserInfo;
    public string playerBuff;
    public string PlayerIP;//玩家当前登录ip
	/// <summary>
	/// 用户首充活动状态,-1未参与，0已参与未领取，1已领取
	/// </summary>
	public int isUserPromotioned=-1;
	/// <summary>
	/// 用户首充活动码
	/// </summary>
	public string sUserPromotionCode;
	/// <summary>
	/// 用户是否领取了车展礼包
	/// </summary>
	public bool isUserGetCarGift;
	/**
     * 福利金数量.
     */
    public int FuLiJinNum = 4000;
	/**
	 * 玩家最大金币数量.
     */
	public static int PlayerCoinMax = 2000000000;
    /**
     * 玩家的金币信息.
     */
    public int nCoinNum = 200;
    /**
     * 金币房最少金币数量数组.
     */
    int[] CoinNumMin;
    /**
     * 金币房最多金币数量数组.
     */
    int[] CoinNumMax;
    public MainPlayer pMainPlayer;
    public int nFangKaCount = 0;
    /// <summary>
    /// 手机号信息.
    /// </summary>
    [HideInInspector]
    public string TelInfo = "";
    /**
     * 鲜花数量.
     */
    public int FlowerNum;
    /**
     * 牛粪数量.
     */
    public int NiuFenNum;
    /**
     * 点赞数量.
     */
    public int DianZanNum;
    private int JiuJiCount;//玩家救济金领取次数
    private DateTime JiuJiTime;//领取救济金时间
    //托管
    public bool m_bTrustee=false;//是否托管

    public int selfCardJiFenData = 0;//玩家创建房卡房自己的积分数量，切完场景使用其赋值

	/// <summary>
	/// 用户钻石数量 
	/// </summary>
	public int GemCount;	
	/// <summary>
	/// 免费次数是否用完.
	/// </summary>
	[HideInInspector]
    public bool IsMianFeiBiSaiOver = false;
    /// <summary>
    /// 玩家登录上来，上次的房间信息
    /// 如果有信息，则不可进入新房间，会提示重连
    /// </summary>
    public class RelinkUserRoomData
    {
        /*public enum RoomType
        {
            RoomType_NULL,
            RoomType_Gold,          //金币麻将
            RoomType_RoomCard,      //房卡麻将
            RoomType_TwoHumanRoom,  //二人麻将
            RoomType_ThemeRace_HaiXuan,  //华商专场的海选比赛
            RoomType_ThemeRace_Group,    //华商专场的小组赛
            RoomType_MyRoom,        //自建比赛
            RoomType_OfficialRoom,  //官方比赛
            RoomType_MultRace_HaiXuan,  //社区的海选比赛
            RoomType_MultRace_Group,    //社区的小组赛
        }*/
        public OneRoomData.RoomType roomType;
        public int roomID;
        public int roomLevel;
        public RelinkUserRoomData(int type,int id,int lv)
        {
            roomType = (OneRoomData.RoomType)type;
            roomID = id;
            roomLevel = lv;
        }
    }
    public RelinkUserRoomData relinkUserRoomData=null;
	/// <summary>
	/// 主题赛活动详情
	/// </summary>
	public string sThemeRace_HuoDongXiangQing = "";

    public void OnEnterRoom(MainPlayer p)
    {
        pMainPlayer = p;
    }
    void Start()
    {
        IsOpenQinRenBiSaiRuKouDlg = false;
    }
	/// <summary>
	/// 微信授权成功
	/// </summary>
	public void PlayerAuthorizeSuccess(Hashtable result)
	{
		isUserAuthorized = true;

		sWeChat_Openid = (string)result["openid"];
		Debug.Log("Unity:" + "WeChat_Openid:" + sWeChat_Openid);

		sWeChat_Unionid = (string)result["unionid"];
		//Debug.Log("Unity:" + "WeChat_Unionid:" + sWeChat_Unionid);

		sUserName = (string)result["nickname"];
		Debug.Log("Unity:" + "WeChat_sUserName:" + sUserName);

		sHeadImgUrl = (string)result["headimgurl"];
		Debug.Log("Unity:" + "WeChat_HeadImgUrl:" + sHeadImgUrl);

		nSex = System.Convert.ToInt32(result["sex"]);
		Debug.Log("Unity:" + "WeChat_nSex:" + nSex.ToString());


		if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
					 RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            //关闭微信登陆对话框.
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl.DestroyThis();
            }
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallWeChatPlayerLogin(sWeChat_Unionid, sWeChat_Openid, sUserName, nSex, sHeadImgUrl);
		}

		/*Debug.Log("Unity:"+"get user info result :");
		Debug.Log("Unity:"+"toJson:" + result.toJson());
		Debug.Log("Unity:"+"jsonCode:" + MiniJSON.jsonEncode(result));
		Debug.Log("Unity:"+"AuthInfo:" + MiniJSON.jsonEncode(MainRoot._gWeChat_Module.ssdk.GetAuthInfo(PlatformType.WechatPlatform)));
		*/

	}
	/// <summary>
	/// 微信授权失败
	/// </summary>
	public void PlayerAuthorizeFaild(string sMsg)
	{
		isUserAuthorized = false;
	}
	/// <summary>
	/// 微信授权取消
	/// </summary>
	public void PlayerAuthorizeCancel()
	{
		isUserAuthorized = false;
	}
	/// <summary>
	/// 分享到微信好友和朋友圈成功
	/// </summary>
	/// <returns></returns>
	public void PlayerShareToWeChatSucess(Hashtable result)
	{

	}
	/// <summary>
	/// 分享到微信好友和朋友圈失败
	/// </summary>
	public void PlayerShareToWeChatFaild(string sMsg)
	{

	}
	/// <summary>
	/// 分享到微信好友和朋友圈取消
	/// </summary>
	public void PlayerShareToWeChatCancel()
	{

	}
	/// <summary>
	/// 登陆成功后将数据存入PlayerData内
	/// </summary>
	/// <param name="args"></param>
    public void InitialPlayerData(params object[] args)
    {
		DontDestroyOnLoad(gameObject);
		SpawnAsyncImageDownload();
		CoinNumMin = new int[3];
		CoinNumMax = new int[3];
		try
        {
			isLoginSucceed = (bool)args[11];
            if (isLoginSucceed)
            {
                //关闭微信登陆对话框.
                if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl != null)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pWeiXinLoginCtrl.DestroyThis();
                }
				if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
				{
					MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.CloseAuditLogin_RegUI();
				}
			}
            nUserId = (int)args[2];
			sUserName = (string)args[3];
			nCoinNum = (int)args[4];
			nFangKaCount = (int)args[5];
			GemCount = (int)args[21];
            JiuJiCount = (int)args[12];
            JiuJiTime = (DateTime)args[13];
			sWeChat_Unionid = (string)args[14];
			sWeChat_Openid = (string)args[15];
			sHeadImgUrl = (string)args[16]; //这里是给玩家微信头像url赋值的地方.
			nSex = (int)args[17];
            playerBuff = (string)args[18];
            if (args.Length>22)
            {
                PlayerIP = (string)args[22];//这里是给玩家IP地址赋值的地方.
            }
            IsExitAbnormalPlayer = false;
            SpawnAsyncImageDownload();
            MainRoot._gUIModule.pUnModalUIControl.ShopDiamondToCoinCard();
            UpdatePlayerDtInfo();

			Debug.Log("Unity:"+"InitialPlayerData:"+isLoginSucceed.ToString()+ "\\"+nUserId.ToString());
			string stemp = ((string)args[19]).Replace("#", ":");
			string[] datas = new string[9];
			datas = stemp.Split(":".ToCharArray());
            int indexVal = 0;
			for (int i = 0; i < datas.Length; i++)
			{
                if (i % 4 != 0)
                {
                    MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[indexVal] = System.Convert.ToInt32(datas[i]);
                    indexVal++;
                }
			}
            RefreshPlayerBuff(BUFFBIN.BIN_ACTIVATIONCODE);
            RefreshPlayerBuff(BUFFBIN.BIN_ISFIRSTCHARGE);
            RefreshPlayerBuff(BUFFBIN.BIN_ISGETSTARTGIFT);

            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.FreshPromotionGiftData();
            //return;
            int minCoin = MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[0];
            int maxCoin = MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[1];
			MainRoot._gPlayerData.InitPlayerCoinNumLevel(0, minCoin, maxCoin);
            minCoin = MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[3];
            maxCoin = MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[4];
			MainRoot._gPlayerData.InitPlayerCoinNumLevel(1, minCoin, maxCoin);
            minCoin = MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[6];
            maxCoin = MainRoot._gRoomData.cGoldRoomSetting.vGoldRoomSetting[7];
			MainRoot._gPlayerData.InitPlayerCoinNumLevel(2, minCoin, maxCoin);
            return;
            
			int flowerNum = (int)args[4];
			int niuFenNum = (int)args[5];
			int dianZanNum = (int)args[6];
			InitPlayerDaoJuDt(flowerNum, niuFenNum, dianZanNum);
        }
		catch (System.Exception ex)
		{
            Debug.LogError("Unity:" + ex.ToString());
        }
    }

    /// <summary>
    /// 玩家登陆的时候 二次给的玩家数据
    /// args: 4 手机号string.
    /// </summary>
    public void InitialPlayerDataSecond(params object[] args)
    {
        Debug.Log("Unity: InitialPlayerDataSecond -> args[3] " + (int)args[3]);
        switch ((int)args[3])
        {
            case 1: //玩家手机号信息.
                {
                    TelInfo = (string)args[4];
                    break;
                }
            case 2: //是否产生华商大转盘或是否产生公众号领红包.
                {
                    bool isSpawnHuaShangDaZhuanPan = (bool)args[4];
                    bool isSpawnHuaShangHongBao = (bool)args[5];
                    //isSpawnHuaShangDaZhuanPan = true; //test.
                    if (isSpawnHuaShangDaZhuanPan)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgHuaShangDaZhuanPan();
                    }

                    if (isSpawnHuaShangHongBao)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgQRGongZhongHao();
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public void RefreshPlayerBuff(BUFFBIN flag)
    {
        try
        {
            if ("".Equals(playerBuff))
            {
                return;
            }
            string[] playerBuffArray = playerBuff.Split(':');
            switch (flag)
            {
                case BUFFBIN.BIN_ACTIVATIONCODE:
                    sUserPromotionCode = playerBuffArray[0];
                    if (!"".Equals(sUserPromotionCode))
                    {
                        if (isUserPromotioned == -1)
                        {
                            isUserPromotioned = 0;
                        }
                    }
                    break;
                case BUFFBIN.BIN_ISFIRSTCHARGE:
                    if (playerBuffArray.Length>1)
                    {
                        if ("".Equals(playerBuffArray[0]))
                        {
                            isUserPromotioned = -1;//无码
                        }
                        else
                        {
                            if ("".Equals(playerBuffArray[1]))
                            {
                                isUserPromotioned = 0;
                            }
                            else
                            {
                                isUserPromotioned = System.Convert.ToInt32(playerBuffArray[1]);
                            }
                        }
                    }
                    break;
                case BUFFBIN.BIN_ISGETSTARTGIFT:
                    if (playerBuffArray.Length > 2)
                    {
                        if ("".Equals(playerBuffArray[2]))
                        {
                            isUserGetCarGift = false;
                        }
                        else
                        {
                            isUserGetCarGift = System.Convert.ToInt32(playerBuffArray[2]) == 1 ? true : false;
                        }
                    }
                    break;
                default:
                    break;
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    /**
     * 初始化玩家道具信息.
     */
    public void InitPlayerDaoJuDt(int flower, int niuFen, int dianZan)
    {
        FlowerNum = flower;
        NiuFenNum = niuFen;
        DianZanNum = dianZan;
    }
    /**
     * 初始化金币房的所需金币数组信息.
     */
    public void InitPlayerCoinNumLevel(byte indexVal, int minCoin, int maxCoin)
    {
        CoinNumMin[indexVal] = minCoin;
        CoinNumMax[indexVal] = maxCoin;
    }
    /// <summary>
    /// 初始化复盘房间基本信息
    /// </summary>
    /// <param name="type">金币房卡</param>
    /// <param name="roomId">房间id</param>
    /// <param name="lv">房间等级</param>
    public void InitPlayerRelinkRoomInfo(int type,int roomId,int lv)
    {
        if (roomId == 0)
        {//删除房间滞留数据
            if (relinkUserRoomData != null)
            {
                relinkUserRoomData = null;
            }
            return;
        }
        relinkUserRoomData = new RelinkUserRoomData(type,roomId,lv);
    }
    public int[] GetGameCoinNumMinAy()
    {
        if (CoinNumMin == null || CoinNumMin.Length < 3)
		{
			Debug.Log("Unity:"+ "GetGameCoinNumMinAy Error!");
            CoinNumMin = new int[3]{2000, 20000, 50000};
        }
        return CoinNumMin;
    }
    public int[] GetGameCoinNumMaxAy()
    {
        if (CoinNumMax == null || CoinNumMax.Length < 3)
		{
			Debug.Log("Unity:" + "GetGameCoinNumMaxAy Error!");
			CoinNumMax = new int[3]{100000, 500000, 2000000000};
        }
        return CoinNumMax;
    }
    public int GetPlayerCoinNum()
    {
        Debug.Log("Unity:"+"GetPlayerCoinNum -> CoinNum " + nCoinNum);
        return nCoinNum;
    }
    public void AddPlayerCoinNum(int val)
    {
        nCoinNum += val;
        if (nCoinNum > PlayerCoinMax) {
            nCoinNum = PlayerCoinMax;
        }
    }
    public void SubPlayerCoinNum(int val)
    {
        nCoinNum -= val;
        if (nCoinNum < 0) {
            nCoinNum = 0;
        }
        Debug.Log("Unity:"+"SubPlayerCoinNum -> coinNum " + nCoinNum);
    }
    /// <summary>
    /// 玩家金币属性
    /// </summary>
    public int PlayerCoin
    {
        get { return nCoinNum; }
        set { nCoinNum = value; }
    }

    /// <summary>
    /// 是否打开秦人比赛场入口界面.
    /// </summary>
    public bool IsOpenQinRenBiSaiRuKouDlg = false;
    /// <summary>
    /// 设置救济金标记
    /// </summary>
    /// <param name="count">领取次数</param>
    /// <param name="time">领取时间</param>
    public void SetPlayerJiuJiFlag(int count,DateTime time)
    {
        JiuJiCount = count;
        JiuJiTime = time;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(35, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform); //救济金领取成功.
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(35, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //救济金领取成功.
        }
    }
    /// <summary>
    /// 获取当前是否可以领取救济金
    /// </summary>
    /// <returns>不可领取就弹充值界面</returns>
    public bool GetCanBringJiuJiCoin()
    {
        if (DateTime.Now.Date != JiuJiTime.Date)
        {
            return true;
        }
        else
        {
            if (JiuJiCount<2)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 获取玩家救济金领取次数.
    /// </summary>
    public int GetJiuJiCount()
    {
        if (DateTime.Now.Date != JiuJiTime.Date)
        {
            return 0;
        }
        return JiuJiCount;
    }

    //void Update()
    //{
    //    if (Time.frameCount % 15 == 0)
    //    {
    //        if (MainRoot._gPlayerData == null)
    //        {
    //            //Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
    //            return;
    //        }
    //        if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
    //        {
    //            //Debug.Log("Unity:" + "Please login!");
    //            return;
    //        }
    //        if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
    //        {
    //            //Debug.Log("Unity: player is not login!");
    //            return;
    //        }
    //        UpdatePlayerDtInfo();
    //    }
    //}

    /// <summary>
    /// 更新玩家UI界面的金币等信息.
    /// </summary>
    public void UpdatePlayerDtInfo()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pPlayerInfoCtrl.SetPlayerInfo(this);
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.pPlayerInfoCtrl.SetPlayerInfo(this);
            }
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pShopPanel.SetShopPanelInfo();
            }
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pPlayerInfoCtrl.SetPlayerInfo(this);
        }

        if (MainRoot._gGameRoomCenter != null && MainRoot._gGameRoomCenter.gGameRoom != null)
        {
            PlayerBase pBase = MainRoot._gGameRoomCenter.gGameRoom.pMainPlayer;
            if (pBase != null)
            {
                pBase.pInfoPanel.ShowPlayerCoinNumInfo(PlayerCoin);
            }
        }
    }
    /// <summary>
    /// 产生玩家微信头像控制逻辑.
    /// </summary>
    void SpawnAsyncImageDownload()
    {
        if (pAsyncImageDownload != null) {
            return;
        }
        Debug.Log("Unity:"+"SpawnAsyncImageDownload...");
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/AsyncImgDownload"), transform, false);
        pAsyncImageDownload = obj.GetComponent<AsyncImageDownload>();
    }
    /// <summary>
    /// 获取玩家的IP信息.
    /// </summary>
    public string getIpAddress()
    {
        string tempip = "";
        try
        {
            WebRequest wr = WebRequest.Create("http://1212.ip138.com/ic.asp");
            Stream s = wr.GetResponse().GetResponseStream();
            StreamReader sr = new StreamReader(s, System.Text.Encoding.Default);
            string all = sr.ReadToEnd(); //读取网站的数据

            int start = all.IndexOf("[") + 1;
            int end = all.IndexOf("]");
            int count = end - start;
            tempip = all.Substring(start, count);
            sr.Close();
            s.Close();
        }
        catch
        {
        }
        return tempip;
    }
}