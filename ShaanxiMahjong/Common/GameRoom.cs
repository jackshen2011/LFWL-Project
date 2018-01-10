using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class MsgInfo
{
    //废弃,委托函数参数类

    public int status;
    public int headCode;
    public string message;
    public int length;
    
    public MsgInfo()
    {
        status = 0;
        headCode = 0;
        message = "测试";
        length = message.Length;
    }
}
/// <summary>
/// 用来记录每个游戏界面用户头像的位置
/// </summary>
class InfoPanelPos
{
    public Vector3 vPos;
    public int nsit;
    public bool bIsUsed =false;
    public InfoPanelPos(int t, Vector3 pos)
    {
        if (!bIsUsed)
        {
            nsit = t;
            vPos = pos;
        }
    }
}
public enum PLAYERTYPE
{
    NULL,
    MAIN_USER,
    OTHER_USER,
    WATCH_USER,
    DIAOYU_USER
}
/// <summary>
/// 游戏房间对象
/// </summary>
class GameRoom : UnModalUIBase
{
    //-------------------其他声明----------------
	/// <summary>
	/// 牌桌上的所有用户对象列表
	/// </summary>
    public List<PlayerBase> m_PlayerList = new List<PlayerBase>();

	/// <summary>
	/// 牌桌上用户详细信息界面列表
	/// </summary>
    public List<InfoPanelPos> PlayerInfoVec = new List<InfoPanelPos>();

	/// <summary>
	/// 客户端对应自己的牌桌玩家对象
	/// </summary>
    public PlayerBase pMainPlayer;
	/// <summary>
	/// 庄家用户的数字id
	/// </summary>
    int nZhuangJiaUserId = -1;
    public int MAX_USER = 4;
    public int GetZhuangJiaUserId
    {
        get {return nZhuangJiaUserId; }
    }
    public int SetZhuangJiaUserId
    {
        set { nZhuangJiaUserId = value; }
    }

    public GameRoom()
    {
        
    }
	/// <summary>
	/// 根据id从牌桌用户列表中找到玩家对象
	/// </summary>
	/// <param name="nid"></param>
	/// <returns></returns>
    public PlayerBase GetPlayerByUserId(int nid)
    {
        PlayerBase p;
        p = m_PlayerList.Find(delegate (PlayerBase player) { return player.nUserId.Equals(nid); });
        return p;
    }
	/// <summary>
	/// 根据昵称从牌桌用户列表中找到玩家对象
	/// </summary>
	/// <param name="sname"></param>
	/// <returns></returns>
	public PlayerBase GetPlayerByUserName(string sname)
    {
        PlayerBase p;
        p = m_PlayerList.Find(delegate (PlayerBase player) { return player.sUserName.Equals(sname); });
        return p;
    }
	/// <summary>
	/// 初始化详情列表所在位置
	/// </summary>
	/// <param name="args"></param>
    public void Initial()
    {
        //UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneManager_sceneUnloaded; ;
        float height = Screen.height;
        if (SystemSetManage.ResolutionIndex != -1)
            height =  SystemSetManage.initializeSystemSetManage.CanvasValueList[SystemSetManage.ResolutionIndex];

        float width = (float)Screen.width / (float)Screen.height * (float)height;

        float IconWidth = 170f / 1920f * width;
        float IconHeight = 60f / 1080f * height;

        //根据屏幕坐标算
        PlayerInfoVec.Add(new InfoPanelPos(0, new Vector3((-(float)width / 2.0f + IconWidth) , -290.0f / 1080f * height, 0f)));   //主家
        PlayerInfoVec.Add(new InfoPanelPos(1, new Vector3((-(float)width / 2.0f + IconWidth), 159.0f / 1080f * height, 0f)));   //上家
        PlayerInfoVec.Add(new InfoPanelPos(2, new Vector3(446.0f / 1920f * width, 454.0f / 1080f * height, 0f)));    //对家
        PlayerInfoVec.Add(new InfoPanelPos(3, new Vector3(((float)width / 2.0f - IconWidth), 168.0f / 1080f * height, 0f)));    //下家

        MainRoot._gMainRoot.RoomCenterInitEnd();
    }
    /// <summary>
    /// 根据指定的userid获取玩家是当前客户端主家的上下对家
    /// </summary>
    /// <returns>0主家自己,1上家,2对家，3下家</returns>
    public int GetUserUISitByUserID(int nid)
    {
        int n = 0;
        PlayerBase tempplayer;
        tempplayer = GetPlayerByUserId(nid);
        if (tempplayer!=null)
        {
            n = tempplayer.nUserSit< pMainPlayer.nUserSit?(tempplayer.nUserSit+4 - pMainPlayer.nUserSit):(tempplayer.nUserSit - pMainPlayer.nUserSit);
			tempplayer.nUIUserSit = n;
			//Debug.Log("GetUserUISitByUserID:" + tempplayer.nUserId.ToString() + "/" + tempplayer.nUserSit.ToString() + "/" + pMainPlayer.nUserSit.ToString()+"/"+ tempplayer.nUIUserSit.ToString());
		}
		return n;
    }
	/// <summary>
	/// 根据指定的sit获取对应玩家的对象
	/// </summary>
	/// <param name="nSit">0主家自己,1上家,2对家，3下家</param>
	/// <returns></returns>
	public PlayerBase GetUserByServerSit(int nSit)
	{
		PlayerBase p;
		p = m_PlayerList.Find(delegate (PlayerBase player) { return player.nUserSit.Equals(nSit); });
		return p;
	}
	/// <summary>
	/// 切换场景前删除不保留的对象
	/// </summary>
	/// <param name="arg0"></param>
    private void SceneManager_sceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
    {
        Transform _canvas = GameObject.Find("Canvas").transform;
        foreach (Transform item in _canvas)
        {
            GameObject.Destroy(item.gameObject);
        }
        return;
        //throw new System.NotImplementedException();
    }
	/// <summary>
    /// 初始化客户端自己对应在牌桌中的对象
    /// urlHead -> 玩家微信头像的url.
	/// </summary>
	/// <param name="nUserID"></param>
	/// <param name="nSit">座次</param>
    public void InitialMainPlayer(int nUserID, string sName, string urlHead, int nSit,int nGold,int nSex, int nDifen, int[] daoJuDt =null)
    {
		GameObject temp;
		if (nUserID<0)
		{
			Debug.Log("Unity:"+ "InitialMainPlayer id Error！："+ nUserID.ToString());
			return;
		}
		pMainPlayer = GetPlayerByUserId(nUserID);
		if (pMainPlayer==null)
		{
			if (m_PlayerList.Count >= MAX_USER)
			{
				return;
			}
			if (m_PlayerList.Exists(delegate (PlayerBase m) { return (m.playertype == PLAYERTYPE.MAIN_USER); }) != false)
			{
				Debug.Log("Unity:" + "Exist MainPlayer!");
				return;
			}

			temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/MainPlayer"), MainRoot._gGameRoomCenter.gGameRoom.gameObject.transform, false);
			pMainPlayer = temp.GetComponent<MainPlayer>();
			m_PlayerList.Add(pMainPlayer);
		}
		pMainPlayer.Initial(sName, nUserID, nSit, nGold, nDifen, nSex, urlHead);
		//pMainPlayer.InitPlayerDaoJuDt(daoJuDt);
		((MainPlayer)pMainPlayer).Initial();

	}
	/// <summary>
	/// 初始化其他玩家在牌桌中的对象
	/// urlHead -> 玩家微信头像的url.
	/// </summary>
	/// <param name="nUserID"></param>
	/// <param name="nSit"></param>
	public void InitialOneOtherPlayer(int nUserID, string sName, string urlHead, int nSit, int nGold, int nSex, int nDifen, int[] daoJuDt=null)
    {
		GameObject temp;
		PlayerBase tempplayer;
		tempplayer = GetPlayerByUserId(nUserID);
		if (tempplayer==null)
		{
			if (m_PlayerList.Count >= MAX_USER)
			{
				Debug.Log("Unity:" + "Player is Full!");
				return;
			}
			List<PlayerBase> templist = m_PlayerList.FindAll(delegate (PlayerBase m) { return (m.playertype == PLAYERTYPE.OTHER_USER); });
			if (templist.Count >= MAX_USER - 1)
			{
				Debug.Log("Unity:" + "Other Player is Full!");
				return;
			}

			temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/OtherPlayer"), MainRoot._gGameRoomCenter.gGameRoom.gameObject.transform, false);
			tempplayer = temp.GetComponent<OtherPlayer>();
			m_PlayerList.Add(tempplayer);
		}
		tempplayer.Initial(sName, nUserID, nSit, nGold, nDifen, nSex, urlHead);
		//tempplayer.InitPlayerDaoJuDt(daoJuDt);
		((OtherPlayer)tempplayer).Initial();

	}
    public void SetTablePlayerIp(int nUserID,string sIp)
    {
        PlayerBase tempplayer;
        tempplayer = GetPlayerByUserId(nUserID);
        if (tempplayer != null)
        {
            tempplayer.SetIp(sIp);
        }

    }
    /// <summary>
	/// 根据座次，取得一个弹出的详情界面的坐标
	/// </summary>
	/// <param name="nUserSit"></param>
	/// <returns></returns>
	public Vector3 GetOneInfoPanelPos(int nUserSit)
    {
        Vector3 pos = Vector3.zero;
        InfoPanelPos n = PlayerInfoVec.Find(delegate (InfoPanelPos m) { return (m.bIsUsed == false) && (m.nsit == nUserSit);  });
        if (n != null)
        {
            pos = n.vPos;
            n.bIsUsed = true;
            PlayerInfoVec[PlayerInfoVec.IndexOf(n)] = n;
        }
        return pos;
    }
	public void ResetOneInfoPanelPos(int nUserSit)
	{
		InfoPanelPos n ;
		if (nUserSit>=0 && nUserSit<4)
		{
			n = PlayerInfoVec[nUserSit];
			n.bIsUsed = false;
			PlayerInfoVec[nUserSit] = n;
		}
	}

	void Update () {
	

	}
}
