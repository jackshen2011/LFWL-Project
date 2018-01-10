using UnityEngine;
using System.Collections;

/// <summary>
/// 牌桌上的玩家的基类
/// </summary>
abstract class PlayerBase : UnModalUIBase
{
	/// <summary>
	/// 用户的数字id
	/// </summary>
    public int nUserId;
	/// <summary>
	/// 用户昵称
	/// </summary>
    public string sUserName;
    /// <summary>
    /// 玩家微信头像的Url信息.
    /// </summary>
    public string sHeadImgUrl = "";
	/// <summary>
	/// 玩家性别信息.1男2女
	/// </summary>
	public int nSex = 0;
    public string sUserIp="";//显示玩家ip
	/// <summary>
	/// 用户在服务器上的座次，东北西南，逆时针，出牌顺序
	/// </summary>
	public int nUserSit;
	/// <summary>
	/// 用户在UI界面上的座次，0主家自己,1上家,2对家，3下家
	/// </summary>
	public int nUIUserSit;
	/// <summary>
	/// 房卡房间的用户底分
	/// </summary>
	public int nUserDiFen;
	/// <summary>
	/// 用户类型
	/// </summary>
    public PLAYERTYPE playertype = PLAYERTYPE.NULL;
	/// <summary>
	/// 是否庄家
	/// </summary>
    public bool isZhuangJia = false;
    /// <summary>
    /// 房卡房间里玩家是否"已经准备".
    /// </summary>
    public bool IsYiZhunBei = false;
	/// <summary>
	/// 下了几个炮
	/// </summary>
    public int nXiaPaoType = 0;
	/// <summary>
	/// 自己相关的用户详情界面
	/// </summary>
    public UserInfoPanel pInfoPanel;
    /**
     * 玩家的金币数量.
     */
    public int nCoinNum = 0;
    public int nCardCoinNum=0;
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
    public abstract void LinkMyDelegate();  //必须实现将自己的函数挂到GameCenter委托上
    /*{
        GameCenter.getInstance().mytest += MyTest333;
    }*/
    public bool isUse = false;
	/// <summary>
	/// 初始化函数
	/// </summary>
	/// <param name="nUserID"></param>
	/// <param name="nSit"></param>
    public void Initial(string userName, int nUserID,int nSit,int nGold,int nDiFen,int nsex,string headurl)
    {
        sUserName = userName;
        nUserId = nUserID;
        nUserSit = nSit;
		nUserDiFen = nDiFen;
        IsYiZhunBei = false;

        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
        {
            nCoinNum = nGold;
        }
        else //if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.CARDROOM)
        {
            nCardCoinNum = nGold;
        }
        
        sHeadImgUrl = headurl;
		nSex = nsex;
		//Debug.Log("Unity:PlayerBase:" + nUserId.ToString() + "/" + nUserSit.ToString());
	}

	/// <summary>
	/// 点击头像后创建一个用户详情界面
	/// </summary>
    public void InitialLinkPanel()
    {
        if (pInfoPanel != null)
        {
            pInfoPanel.Initial(this);
            return;
        }
        GameObject temp;
        temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/UserInfoPanel"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
        pInfoPanel = temp.GetComponent<UserInfoPanel>();
        pInfoPanel.Initial(this);
        Vector3 pos;
        pos = MainRoot._gGameRoomCenter.gGameRoom.GetOneInfoPanelPos(MainRoot._gGameRoomCenter.gGameRoom.GetUserUISitByUserID(nUserId));
        ((RectTransform)temp.transform).anchoredPosition = pos;
    }
    public void SetIp(string sIp)
    {
        sUserIp = sIp;
    }
	/// <summary>
	/// 我是庄家
	/// </summary>
	/// <param name="isZhuang"></param>
    public void SetZhuangjia(bool isZhuang)
    {
        isZhuangJia = isZhuang;
        if (pInfoPanel)
        {
            pInfoPanel.SetZhuangjia(isZhuang);
        }
    }
	/// <summary>
	/// 设定该玩家是否已经准备
	/// </summary>
	/// <param name="isZhunBei"></param>
	public void SetReadyState(bool isZhunBei)
	{
		IsYiZhunBei = isZhunBei;
		if (IsYiZhunBei)	//已准备
		{
			//0主家自己,1上家,2对家，3下家
			if (nUIUserSit!=0)//自己不显示准备中
			{
				MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(nUIUserSit + 5);//删除准备中
			}
			MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(nUIUserSit +29, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //显示已准备.
		}
		else
		{
			MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(nUIUserSit + 29);//删除已准备
			if (nUIUserSit != 0)//自己不显示准备中
			{
				MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(nUIUserSit + 5, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //显示准备中.
			}
		}
	}
	/// <summary>
	/// 关闭已准备和准备中显示
	/// </summary>
	public void CloseAllReadyMsgTip()
	{
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(nUIUserSit + 5);//删除准备中
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(nUIUserSit + 29);//删除已准备
	}
	/// <summary>
	/// 我下了几炮
	/// </summary>
	/// <param name="ntype"></param>
	public void SetXiaPaoImg(int ntype)
    {
        nXiaPaoType = ntype;
        if (pInfoPanel)
        {
            pInfoPanel.SetXiaPaoImg(ntype);
        }
    }
    public virtual void OnUseItem(int ntype)
    {
 
    }
    /**
     * 初始化玩家道具信息.
     * daoJuDt[0] -> 金币数量.
     * daoJuDt[1] -> 鲜花数量.
     * daoJuDt[2] -> 牛粪数量.
     * daoJuDt[3] -> 点赞数量.
     * daoJuDt[4] -> 玩家性别,daoJuDt[4] == 0 -> boy, daoJuDt[4] == 1 -> girl.
     */
    public void InitPlayerDaoJuDt(int[] daoJuDt)
    {
        Debug.Log("Unity:"+"InitPlayerDaoJuDt...");
        FlowerNum = daoJuDt[0];
        NiuFenNum = daoJuDt[1];
        DianZanNum = daoJuDt[2];
        //PlayerSex = (PlayerData.PlayerSexEnum)daoJuDt[4];
    }
    /// <summary>
    /// 在这里检测PlayerBase的id是否和玩家id相同,如果相同则更新pBase的玩家数据信息.
    /// MainRoot._gPlayerData是客户端玩家数据的存储对象.
    /// GameRoom.m_PlayerList存储了当前牌局里4个玩家的数据信息.
    /// </summary>
    public void CheckPlayerDataFromPlayerBase()
    {
        if (MainRoot._gPlayerData == null || nUserId != MainRoot._gPlayerData.nUserId) {
            return;
        }
        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
        {
            nCoinNum = MainRoot._gPlayerData.nCoinNum;
        }
        
        FlowerNum = MainRoot._gPlayerData.FlowerNum;
        NiuFenNum = MainRoot._gPlayerData.NiuFenNum;
        DianZanNum = MainRoot._gPlayerData.DianZanNum;
        sHeadImgUrl = MainRoot._gPlayerData.sHeadImgUrl;
    }
    /// <summary>
    /// 玩家增加金币
    /// </summary>
    /// <param name="add"></param>
    public void PlayerAddCoin(int add)
    {
        int coin=0;
        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
        {
            nCoinNum += add;
            coin = nCoinNum;
        }
        else //if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.CARDROOM)
        {
            nCardCoinNum += add;
            coin = nCardCoinNum;
        }
        
        
        if (pInfoPanel)
        {
            pInfoPanel.ShowPlayerCoinNumInfo(coin);

		}
    }
    /// <summary>
    /// 游戏中的玩家积分显示量
    /// </summary>
    public int PlayerCoin
    {
        get {
            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
            {
                return nCoinNum;
            }
            else
            {
                return nCardCoinNum;
            }
        }
        set {
            int coin=0;
            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
            {
                coin=nCoinNum = value;
            }
            else //if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.CARDROOM)
            {
                coin = nCardCoinNum = value;
            }


            if (pInfoPanel)
            {
                pInfoPanel.ShowPlayerCoinNumInfo(coin);

            }
        }
    }
	public override void DestroyThis()
	{
		MainRoot._gGameRoomCenter.gGameRoom.m_PlayerList.Remove(this);
		base.DestroyThis();
	}
}
