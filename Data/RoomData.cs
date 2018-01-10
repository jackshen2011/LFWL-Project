using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 房间信息类
/// </summary>
public class OneRoomData
{
    /// <summary>
    /// 红包/钻石赛的房间类型.
    /// </summary>
    public enum OfficialRoomType
    {
        Type_Jewel, //钻石赛
        Type_MultiplayerRedPacket,  //多用户红包
        Type_SingleplayerRedPacket,  //单用户红包
    }
    /// <summary>
    /// 秦人麻将官方比赛的子房间类型.
    /// </summary>
    public OfficialRoomType eOfficialRoomType = OfficialRoomType.Type_MultiplayerRedPacket;
    /// <summary>
    /// 官方比赛(红包赛/钻石赛)名称.
    /// </summary>
    public string OfficialRoomName = "";

    /// <summary>
    /// 房间状态
    /// </summary>
    public enum RoomStat
	{
		NULL,		//未初始化
		WAIT_JOIN,	//等待加入
		WAIT_START,	//等待开始
		PLAYING,	//游戏中
        PLAYINGWAIT,//房卡房的中间状态
		WAIT_NEXT,	//等待下一局开始
		WAIT_COLSE,	//等待解散
		COLSE		//已经解散
	}

	/// <summary>
	/// 房间类型
	/// </summary>
	public enum RoomType
	{
		RoomType_NULL,		//未初始化
		RoomType_Gold,	//金币房间
		RoomType_RoomCard,	//房卡房间
		RoomType_TwoHumanRoom,	//二人房间
        RoomType_ThemeRace_HaiXuan, //海选赛房间
        RoomType_ThemeRace_Group,   //小组赛淘汰赛房间
        RoomType_MyRoom,                //自建房间
        RoomType_OfficialRoom,  //官方比赛
        RoomType_MultRace_HaiXuan,  //社区的海选比赛
        RoomType_MultRace_Group,    //社区的小组赛
    }

    /// <summary>
    /// 比赛类型.
    /// MultiRaceType_ThemeRace 华商专题赛.
    /// MultiRaceType_Room1     社区麻将赛.
    /// </summary>
    public enum DataDefine_MultiRaceType
    {
        MultiRaceType_NULL = -1,          //未知
        MultiRaceType_ThemeRace = 0,      //华商
        MultiRaceType_Room1 = 1,          //社区1
        MultiRaceType_Room2 = 2,
    }
    /// <summary>
    /// 主题比赛状态.
    /// </summary>
    public enum ThemeRaceState
    {
        ShiJianWeiDao, //比赛时间未到
        HaiXuanStart, //海选赛开始
        GroupZhunBei, //小组赛准备
        GroupStart, //小组赛开始
    }
    /// <summary>
    /// 小组赛比赛状态.
    /// </summary>
    public enum ThemeRace_GroupState
    {
        Game_NotStart,  //比赛未开始
        Game_Ready, //比赛准备阶段
        Game_Runing, //比赛期间
    }

    /// <summary>
    /// 房间号
    /// </summary>
    public int nRoomId = 0;
	/// <summary>
	/// 房间类型
	/// </summary>
	public RoomType eRoomType = RoomType.RoomType_NULL;

	/// <summary>
	/// 房间状态
	/// </summary>
	public RoomStat eRoomState = RoomStat.NULL;
    /// <summary>
    /// 比赛类型.
    /// </summary>
    public DataDefine_MultiRaceType eMultiRaceType = DataDefine_MultiRaceType.MultiRaceType_NULL;
    /// <summary>
    /// 当前打到第几局
    /// </summary>
    public int nCurRound = 0;
	/// <summary>
	/// 总局数
	/// </summary>
	public int nMaxRound = 0;
	/// <summary>
	/// 房主ID
	/// </summary>
	public int nRoomOwnerId = 0;
	/// <summary>
	/// 房主昵称
	/// </summary>
	public string sRoomOwnerName = "";
	/// <summary>
	/// 金币房间等级
	/// </summary>
	public int nGoldRoomLevel = 0;
	/// <summary>
	/// 底分
	/// </summary>
	public int nDiFen = 0;
	/// <summary>
	/// 房间设置字符串
	/// </summary>
	public string sRoomSetting = "";
	/// <summary>
	/// 房间设置数组
	/// </summary>
	public int[] vRoomSetting;
	/// <summary>
	/// 金币房间金币限制
    /// vGoldRoomSetting[i] -> 金币房最小金币值.
    /// vGoldRoomSetting[i+1] -> 金币房最大金币值.
    /// vGoldRoomSetting[i+2] -> 金币房底分.
	/// </summary>
	public int[] vGoldRoomSetting;
    /// <summary>
    /// 比赛开始时间.
    /// </summary>
    public System.DateTime TimeBiSaiStart;
    /// <summary>
    /// 用户加入房卡房间前的其他用户信息缓存
    /// </summary>
    public object[] tempUserJoinInfo;
    public OneRoomData()
    {
        vGoldRoomSetting = new int[9];
		tempUserJoinInfo = new object[29];//2018-1-5增加4ip 放在最后
        nCurChangCi = 1;
    }

	public bool bIsErRenMjRoom = false;
	/// <summary>
	/// 是否是二人麻将房间
	/// </summary>
	/// <returns></returns>
	public bool IsErRenCardRoom()
	{
		return bIsErRenMjRoom;
    }
    /// <summary>
    /// 是否是总决赛房间
    /// </summary>
    /// <returns></returns>
    public bool IsZongJueSaiRoom;
    /// <summary>
    /// 当前比赛场次信息.
    /// </summary>
    public int nCurChangCi;
    public void Initial()
	{
		sRoomOwnerName = "";
		nRoomId = 0;
		eRoomState = RoomStat.NULL;
		nMaxRound = 0;
		nDiFen = 0;
        //vGoldRoomSetting = new int[9];
    }
	/// <summary>
	///  设置房间信息
	/// </summary>
	/// <param name="nroomid"></param>
	/// <param name="stat"></param>
	/// <param name="ncurround"></param>
	/// <param name="nmaxround"></param>
	/// <param name="nroomownid"></param>
	/// <param name="sroomownname"></param>
	/// <param name="ndifen"></param>
	/// <param name="eroomtype"></param>
	/// <param name="ngoldroomtype"></param>
	public void SetRoomInfo(int nroomid, RoomStat stat,int ncurround,int nmaxround,int nroomownid,string sroomownname,int ndifen, RoomType eroomtype,int ngoldroomlev)
	{
        nRoomId = nroomid;
        eRoomState = stat;
        nCurRound = ncurround;
        nMaxRound= nmaxround;
        nRoomOwnerId = nroomownid;
        sRoomOwnerName = sroomownname;
        nDiFen = ndifen;
        eRoomType = eroomtype;
        nGoldRoomLevel = ngoldroomlev;
	}


}
/// <summary>
/// 用于存储房间信息的类
/// </summary>
class RoomData : UnModalUIBase
{
	/// <summary>
	/// 当前房间信息
	/// </summary>
	public OneRoomData cCurRoomData;
	/// <summary>
	/// 即将加载的新房间数据
	/// </summary>
	public OneRoomData cCacheRoomData;

	public OneRoomData cGoldRoomSetting;
	public void Initial()
	{
		DontDestroyOnLoad(gameObject);
		cCurRoomData = new OneRoomData();
		cCacheRoomData = new OneRoomData();
		cGoldRoomSetting = new OneRoomData();
    }
	/// <summary>
	/// 将房间信息从缓存加载到当前房间去
	/// </summary>
	public void LoadCacheRoomDataToCurData()
	{
		cCurRoomData = cCacheRoomData;
		cCacheRoomData = new OneRoomData();
	}
	/// <summary>
	/// 将当前房间信息存入缓存中
	/// </summary>
	public void UnLoadCacheRoomDataToCache()
	{
		cCacheRoomData = cCurRoomData;
		cCurRoomData = new OneRoomData();

	}
	public void ResetCurRoom()
	{
		cCurRoomData = null;
		cCurRoomData = new OneRoomData();
	}
}