using UnityEngine;
using System.Collections;
using RoomCardNet;
using System.Xml;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;

class MainRoot : MonoBehaviour
{
	/// <summary>
	/// 房间的父对象，负责跟房间有关的网络消息处理，以及流程控制
	/// </summary>
    public static GameRoomCenter _gGameRoomCenter = null;
	/// <summary>
	/// 所有的界面根对象
	/// </summary>
    public static UIModule _gUIModule = null;
	/// <summary>
	/// 游戏的最顶层对象，控制初始化过程，网络消息由此进行分发，一切的开始
	/// </summary>
    public static MainRoot _gMainRoot = null;
	/// <summary>
	/// 客户端用户数据存放对象
	/// </summary>
    public static PlayerData _gPlayerData = null;

	public static RoomData _gRoomData = null;
	/// <summary>
	/// 麻将牌桌，处理所有跟牌有关的内容
	/// </summary>
	public static MJGameTable _pMJGameTable = null;
	/// <summary>
	/// 微信相关功能的SDk
	/// </summary>
	public static WeChat_Module _gWeChat_Module = null;
    /// <summary>
    /// 游戏音乐控制.
    /// </summary>
    public static GameAudioManage _gGameAudioManage = null;
    public static GameSetData _gGameSetData = null;
    /// <summary>
    /// iap支付管理
    /// </summary>
    public static IAPInterface _gIAPManager = null;
    /// <summary>
    /// 标记是否MainRoot已经进行过初始化
    /// </summary>
    public bool _bIsInitialed ;
	/// <summary>
	/// 从XML里加载的所有标准弹出提示的信息
	/// </summary>
	public Dictionary<int, SystemMsgText.SysMsgInfo> dSysMsgInfoDic = new Dictionary<int, SystemMsgText.SysMsgInfo>();
	
	/// <summary>
	/// 显示模式，二进制位为假时表示屏蔽显示 0:app store，1应用宝，2正常
	/// </summary>
	public int g_showmode = 0;
	/// <summary>
	/// 苹果审核模式
	/// </summary>
	public const int SHOWMODE_APP_STORE = 0x01;
	/// <summary>
	/// 应用宝审核模式
	/// </summary>
	public const int SHOWMODE_YingYongBao = 0x02;
	/// <summary>
	/// 正常显示模式
	/// </summary>
	public const int SHOWMODE_Nomal = 0x01;
    /// <summary>
    /// 海选赛是否结束.
    /// </summary>
    [HideInInspector]
    public bool IsSpawnHaiXuanOver = false;

    /// <summary>
    /// Start函数，不解释
    /// </summary>
    void Awake()
	{
		_bIsInitialed = false;
		Debug.Log("Unity:"+"Start _bIsInitialed !!!!!!!"+ _bIsInitialed.ToString());
		if (!_bIsInitialed)
		{
			UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
			Debug.LogWarning("Unity:"+"MainRoot Start :" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
			MainRootInitial();
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}
	}
    public void MainRootInitial()
    {
		try
		{
			//对第一个界面，比如登录界面进行初始化
			switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
			{
				case "GameHall":
					if (!_bIsInitialed)
					{
						_bIsInitialed = true;
						Debug.Log("Unity:"+"_bIsInitialed true!!!!!!!!!!!!!!");
						_gMainRoot = this;
						InitialWeChat_Module();
                        InitailIAPManager();
                        CreatePlayerData();
						CreateRoomData();
						LoadAllSysMessageXML();
                        InitGameSetData();
                        InitGameAudioManage();
                        DontDestroyOnLoad(gameObject);
                        Debug.Log("Unity:"+"MainRoot Start :" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToString());
					}
                    InitialUI(0);
                    if (RoomCardNetClientModule.netModule != null && RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    {
                        RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_SendSceneTypeForServer(0);
                    }
                    break;
				case "ShaanxiMahjong":
					InitialUI(1);
                    if (RoomCardNetClientModule.netModule != null && RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    {
                        RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_SendSceneTypeForServer(1);
                    }
                    break;
				default:
					break;
			}

		}
		catch (System.Exception e)
		{
            Debug.LogError("Unity:"+e.ToString());
		}
    }
	/// <summary>
	/// 所有的服务端网络调用，进入此处，分情况处理或向下传递
	/// </summary>
	/// <param name="nFuntionId">网络消息id</param>
	/// <param name="args">参数</param>
	/// <returns>是否成功</returns>
	public bool OnNetCallByServer(uint nFuntionId, params object[] args)   
    {
        bool isOk = true;
		try
		{
			switch (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
			{
				case "Initial or LoadingScene or OtherScene":

					break;
				case "GameHall":
					if (_gUIModule)
					{
						_gUIModule.OnNetMessageEnterMainUI(nFuntionId, args);
					}
					else
					{
						Debug.Log("Unity:"+"GameHall NetMsg Not Handle:"+nFuntionId.ToString("X")+"\\"+args.ToString());
					}
					break;
				case "ShaanxiMahjong":
					if (_gGameRoomCenter)
					{
						_gGameRoomCenter.OnNetMessageEnterRoom(nFuntionId, args);
					}
					else
					{
						Debug.Log("Unity:"+"ShaanxiMahjong NetMsg Not Handle:" + nFuntionId.ToString("X") + "\\" + args.ToString());
					}
					break;
				default:
					switch (nFuntionId)
					{
						case 0:
							break;
						case 1:
							break;
						case 2:
							break;
						case 3:
							break;
						case 4:
							break;
						case 5:
							break;
						default:
							isOk = false;
							break;
					}
					break;
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Unity:"+ex.ToString());
		}

 
        return isOk;
    }
	/// <summary>
	/// 测试用，废弃
	/// </summary>
	/// <param name="s"></param>
    public void Tos(string s)
    {
        Debug.LogError("Unity:"+s);
    }
	/// <summary>
	/// 创建客户端用户数据对象并初始化
	/// </summary>
	/// <param name="id"></param>
	/// <param name="name"></param>
	/// <param name="pass"></param>
	/// <param name="isloginOK"></param>
    public void IntialPlayerData(params object[] args)
    {
        Debug.LogWarning("Unity:"+"Initial _gPlayerData!");
        if (_gPlayerData != null)
        {
            _gPlayerData.InitialPlayerData(args);
        }
    }
	/// <summary>
	/// 创建游戏房价的父对象并初始化
	/// </summary>
    public static void InitialGameRoomCenter()
    {
        Debug.LogWarning("Unity:"+"Initial GameRoomCenter!");
        if (_gGameRoomCenter == null)
        {
            GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameRoomCenter"));
            _gGameRoomCenter = temp.GetComponent<GameRoomCenter>();
            //_gGameRoomCenter.Initial();
        }
    }
	/// <summary>
	/// 废弃，游戏房间UI初始化完毕
	/// </summary>
    public void RoomCenterInitEnd()
    {
        
    }
	/// <summary>
	/// 根据场景不同，进行不同的UI初始化过程
	/// </summary>
	/// <param name="nSceneType"></param>
    public void InitialUI(int nSceneType)
    {
        switch (nSceneType)
        {
            case 0: //GameHall
                    Debug.LogWarning("Unity:"+"Initial UIModule 0!");
                    if (_gUIModule == null)
                    {
                        GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/UIModule"));
                        _gUIModule = temp.GetComponent<UIModule>();
                        _gUIModule.InitialUIModule(nSceneType);

                        MoleMole.GameRoot ptemproot =null;
                        ptemproot = _gUIModule.gameObject.GetComponent<MoleMole.GameRoot>();
                        ptemproot.Initial();
                    }
                break;
            case 1: //ShaanxiMahjong
                Debug.LogWarning("Unity:"+"Initial UIModule 1!");
				if (_gGameRoomCenter == null)
				{
					InitialGameRoomCenter();
				}
				if (_gUIModule == null)
                {
                    GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/UIModule"));
                    _gUIModule = temp.GetComponent<UIModule>();
                    _gUIModule.InitialUIModule(nSceneType);

                    GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameUIView"), MainRoot._gUIModule.pMainCanvas.transform, false);
                    GameUIView pGameUI = test.GetComponent<GameUIView>();
                    pGameUI.Initial();
                }
                break;
            default:
                break;
        }

        
    }
    /*public static UIModule GetUIModule()
    {
        if (_gUIModule == null)
        {
            MainRoot.InitialUI(0);
        }
        return _gUIModule;

    }*/
	/// <summary>
	/// UI初始化完毕，根据场景不同进行后续的一些处理
	/// </summary>
	/// <param name="nSceneType"></param>
    public void UIModuleInitEnd(int nSceneType)
    {
        switch (nSceneType)
        {
            case 0://GameHall
                break;
            case 1://ShaanxiMahjong

				_gUIModule.pUnModalUIControl.pGameUIView.InitialBaseRoomUI();   //初始化系统提示

                MJGameTableInitial();

				Debug.Log("Unity: eRoomType " + _gRoomData.cCurRoomData.eRoomType.ToString());

                switch (_gRoomData.cCurRoomData.eRoomType)
                {
                    case OneRoomData.RoomType.RoomType_Gold: //如果是金币房间
                        {
                            _gUIModule.pUnModalUIControl.pGameUIView.InitialGoldRoomUI(_gPlayerData.nCoinNum); //客户端启动等待匹配
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_RoomCard: //如果是房卡房间
                        {
                            Debug.Log("Unity:" + "RoomId " + _gRoomData.cCurRoomData.nRoomId);
                            _gUIModule.pUnModalUIControl.pGameUIView.InitialCardRoomUI();
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan: //如果是海选赛
                        {
                            _gUIModule.pUnModalUIControl.pGameUIView.InitialHaiXuanRoomUI();
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_ThemeRace_Group: //如果是淘汰赛
                        {
                            _gUIModule.pUnModalUIControl.pGameUIView.InitialThemeRaceRoomGroupUI();
                            break;
                        }
                    case OneRoomData.RoomType.RoomType_MyRoom: //如果是自建赛
                        {
                            _gUIModule.pUnModalUIControl.pGameUIView.InitialRaceMyRoomUI();
                            break;
                        }
                }
				_gUIModule.pUnModalUIControl.pGameUIView.InitialTestUI();	//初始化测试按钮
				break;
            default:
                break;
        }

    }
   	/// <summary>
   	/// 麻将牌桌初始化
   	/// </summary>
    public void MJGameTableInitial()
    {
        GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/MJGameTable"));
        if (!_pMJGameTable)
        {
            _pMJGameTable = test.GetComponent<MJGameTable>();
            _pMJGameTable.InitGameTable(_gRoomData.cCurRoomData.vRoomSetting);
            Debug.Log("Unity:"+"MJGameTable Initial");
        }
    }
	/// <summary>
	/// 创建客户端存储用户信息的对象
	/// </summary>
	public void CreatePlayerData()
	{
		if (_gPlayerData == null)
		{
			GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/PlayerData"));
			_gPlayerData = temp.GetComponent<PlayerData>();
		}
	}
	/// <summary>
	/// 创建存储房间信息的对象
	/// </summary>
	public void CreateRoomData()
	{
		if (_gRoomData == null)
		{
			GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/RoomData"));
			_gRoomData = temp.GetComponent<RoomData>();
			_gRoomData.Initial();
		}
	}
	/// <summary>
	/// 初始化微信SDK模块
	/// </summary>
	public void InitialWeChat_Module()
	{
		if (_gWeChat_Module==null)
		{
			_gWeChat_Module = gameObject.AddComponent<WeChat_Module>();
			_gWeChat_Module.InitialWeChatSDK();
		}
	}
    /// <summary>
    /// 初始化ios支付
    /// </summary>
    public void InitailIAPManager()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        if (_gIAPManager == null)
		{
            _gIAPManager = gameObject.AddComponent<IAPInterface>();
            _gIAPManager.Init();
		}
#endif
	}
	/// <summary>
	/// 让客户端切换场景
	/// </summary>
	/// <param name="sSceneName"></param>
	public void ChangeScene(string sSceneName)
	{
		switch (sSceneName)
		{
			case "ShaanxiMahjong":
				_gRoomData.LoadCacheRoomDataToCurData();
				UnityEngine.SceneManagement.SceneManager.LoadScene("ShaanxiMahjong");
				
				break;
			case "GameHall":
				_gRoomData.UnLoadCacheRoomDataToCache();
				UnityEngine.SceneManagement.SceneManager.LoadScene("GameHall");

				break;
			default:
				break;
		}

	}

    //延迟启动网络
    

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		MainRootInitial();
		//Debug.Log("Unity:"+"OnSceneLoaded: " + scene.name+":mode "+ mode.ToString());
        //Debug.Log("OnSceneLoaded: " + scene.name+":mode "+ mode.ToString());

	}
	/// <summary>
	/// 主界面初始化的时候加载所有标准提示XML
	/// </summary>
	public void LoadAllSysMessageXML()
	{
		try
		{
			/*
            XmlDocument doc = GameRoot.gameResource.LoadResource_XmlFile("systemmessagetext");

            XmlNode root = doc.SelectSingleNode("SystemMsgText");
            XmlNodeList nodelist = root.SelectNodes("MessageInfo");
            foreach (XmlNode i in nodelist)
            {
                int n = Convert.ToInt32(i.Attribute("nIndex"));
                string s = Convert.ToString(i.Attribute("Prefab"));
                float t = Convert.ToSingle(i.Attribute("Time"));
            Debug.Log("Unity:"+n.ToString()+" "+s.ToString()+" " +t.ToString());
                dSysMsgInfoDic.Add(n, (new SysMsgInfo(n, s, t)));
            }
            */
			XmlDocument xmldoc = new XmlDocument();
			
            TextAsset test = (TextAsset)Resources.Load("systemmessagetext.xml");
            
            byte[] xmldata = test.bytes;
            MemoryStream reader = new MemoryStream(xmldata);
            xmldoc.Load(reader);

            XmlNode root = xmldoc.SelectSingleNode("SystemMsgText");
			XmlNodeList nodelist = root.ChildNodes;

			foreach (XmlNode i in nodelist)
			{
				XmlElement xe = (XmlElement)i;
				int n = Convert.ToInt32(xe.GetAttribute("nIndex"));
				string s = Convert.ToString(xe.GetAttribute("Prefab"));
				float t = Convert.ToSingle(xe.GetAttribute("Time"));
				//Debug.Log("Unity:"+n.ToString() + " " + s.ToString() + " " + t.ToString());
				dSysMsgInfoDic.Add(n, (new SystemMsgText.SysMsgInfo(n, s, t)));
			}
		}
		catch (Exception e)
		{
			Debug.Log("Unity:"+e.Message.ToString());
		}

    }
    void InitGameAudioManage()
    {
        if (_gGameAudioManage != null)
        {
            return;
        }
        _gGameAudioManage = new GameAudioManage();
        _gGameAudioManage.InitGameAudioManage();
    }
    void InitGameSetData()
    {
        if (_gGameSetData != null)
        {
            return;
        }
        _gGameSetData = new GameSetData();
        _gGameSetData.InitGameSetData();
    }
    public void Update()
	{
		// 返回键
		if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Escape)))
		{
			Application.Quit();
		}
		// Home键
		if (Application.platform == RuntimePlatform.Android && (Input.GetKeyDown(KeyCode.Home)))
		{
			Application.Quit();
		}
	}
	/// <summary>
	/// 是否在车展时间范围内
	/// </summary>
	/// <returns></returns>
	public bool IsDateInCarPromotionDate()
	{
		DateTime dt0 = new DateTime(2017, 9, 14, 0, 0, 0);
		DateTime dtlater = new DateTime(2017, 10, 9, 0, 0, 0);
		DateTime dt = DateTime.Now;
		if (dt0 < dt && dt < dtlater)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
