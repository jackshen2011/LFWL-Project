using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SystemSetManage : MonoBehaviourIgnoreGui
{
    //设置分别率
    public string[] ResolutionStrList;    //分辨率的值
    public float[] MainCameraValueList;    //主摄像机分辨率的值
    public float[] HandCardCameraValueList;//手牌摄像机分辨率的值
    public float[] CanvasValueList;//手牌摄像机分辨率的值
    //设置牌墙的坐标
    public string[] HandCardPosList; //设置牌墙的坐标
    private float[] HandCardPos_XList;
    public float HandCardPos_X
    {
        get
        {
            if(ResolutionIndex != -1 && ResolutionIndex < HandCardPos_XList.Length)
            {
                return HandCardPos_XList[ResolutionIndex];
            }
            else
            {
                return 0;
            }
        }
    }
    private float[] HandCardPos_YList;
    public float HandCardPos_Y
    {
        get
        {
            if (ResolutionIndex != -1 && ResolutionIndex < HandCardPos_YList.Length)
            {
                return HandCardPos_YList[ResolutionIndex];
            }
            else
            {
                return 0;
            }
        }
    }

    public static SystemSetManage initializeSystemSetManage;

    /// <summary>
	/// 客户端版本号，正式发布前进行填写
	/// </summary>
	public const string ClientVersion = "1.3.20";

    //渠道号配置  发布的时候必须配上 渠道宏名， 不然分不清楚
#if CHANNELS_HUASHANG
    public const string ChannelsNumber = "HuaShang";
#elif UNITY_IPHONE
    public const string ChannelsNumber = "IOS";
#else 
    public const string ChannelsNumber = "LanFan";
#endif

    //当前分辨率的下标索引
    public int m_Index = -1;
    public static int ResolutionIndex
    {
        get {
            if (initializeSystemSetManage != null)
                return initializeSystemSetManage.m_Index;
            else
                return -1;
            }
    }

    public  delegate void DelegateResolutionSetFn();
    public static DelegateResolutionSetFn delegateDelegateResolutionSetFn = null;
    void Start ()
    {
        initializeSystemSetManage = this;
        Debug.Log("SystemSetManage！！！！");
        for (int i=0; i< ResolutionStrList.Length; i++)
        {
            string value = ResolutionStrList[i];
            string[] list = value.Split('*');
            if(list.Length == 2)
            {
                if((Convert.ToInt32(list[0]) > (Screen.width - 5) && Convert.ToInt32(list[0]) < (Screen.width + 5)) &&
                   (Convert.ToInt32(list[1]) > (Screen.height - 5) && Convert.ToInt32(list[1]) < (Screen.height + 5)) )
                {
                    m_Index = i;
                    Debug.Log("设置分别率！！！！");
                }
            }
        }

        HandCardPos_XList = new float[HandCardPosList.Length];
        HandCardPos_YList = new float[HandCardPosList.Length];
        for (int i = 0; i < HandCardPosList.Length; i++)
        {
            string value = HandCardPosList[i];
            string[] list = value.Split('*');
            if (list.Length == 2)
            {
                HandCardPos_XList[i] = (float)Convert.ToDouble(list[0]);
                HandCardPos_YList[i] = (float)Convert.ToDouble(list[1]);
            }
        }

        if (delegateDelegateResolutionSetFn != null)
        {
            delegateDelegateResolutionSetFn();
        }
    }

    public static void ResolutionSet(Canvas canvasObj)
    {
        return;
        float standard_width = 960f;        //初始宽度  
        float standard_height = 640f;       //初始高度  
        float device_width = 0f;                //当前设备宽度  
        float device_height = 0f;               //当前设备高度  
        float adjustor = 0f;         //屏幕矫正比例  
        //获取设备宽高  
        device_width = Screen.width;
        device_height = Screen.height;
        //计算宽高比例  
        float standard_aspect = standard_width / standard_height;
        float device_aspect = device_width / device_height;
        //计算矫正比例  
        if (device_aspect < standard_aspect)
        {
            adjustor = standard_aspect / device_aspect;
        }

        CanvasScaler canvasScalerTemp = canvasObj.transform.GetComponent<CanvasScaler>();
        if(ResolutionIndex != -1)
            canvasScalerTemp.referenceResolution = new Vector2(Screen.width, initializeSystemSetManage.CanvasValueList[ResolutionIndex]);
    }


    //版本相关代码
    public class GameVersion
    {
        public string netVersion;   //游戏的版本
        public string From;
        public string AuditOnOff;   //审核的开关
        public bool UpdateOnOff = false;  //更新开关
        public string UpdateVersion;    //更新的版本号
        public string DownloadURL;  //更新的下载地址
        public string DownloadURL_IOS;  //更新的下载地址
        public string LinkIp;
        public int[] LinkPort;

        //审核开关
        public bool AuditIOS_OnOff = false;
        public bool AuditMyApp_OnOff = false;
        public bool AuditCultureMinistry_OnOff = false;
        public GameVersion(string netVersion, string From, string AuditOnOff, bool UpdateOnOff, string UpdateVersion, string DownloadURL, string DownloadURL_IOS, string LinkIp, string LinkPort)
        {
            this.netVersion = netVersion;
            this.From = From;
            this.AuditOnOff = AuditOnOff;
            this.UpdateOnOff = UpdateOnOff;
            this.UpdateVersion = UpdateVersion;
            this.DownloadURL = DownloadURL;
            this.DownloadURL_IOS = DownloadURL_IOS;
            this.LinkIp = LinkIp;
            string[] portlist = LinkPort.Split(',');
            this.LinkPort = new int[portlist.Length];
            for (int i = 0; i < this.LinkPort.Length; i++)
            {
                this.LinkPort[i] = Convert.ToInt32(portlist[i]);
            }
            string[] list = AuditOnOff.Split(',');
            if(list.Length >= 2)
            {
                if(list[0].Equals("1") == true)
                {
                    AuditIOS_OnOff = true;
                    Debug.Log("审核IOS 版本 打开啦！！！！！！！！！！");
                }
                if (list[1].Equals("1") == true)
                {
                    AuditMyApp_OnOff = true;
                    Debug.Log("审核应用宝 版本 打开啦！！！！！！！！！！");
                }
                if (list[2].Equals("1") == true)
                {
                    AuditCultureMinistry_OnOff = true;
                    Debug.Log("审核版署 版本 打开啦！！！！！！！！！！");
                }
            }
        }

        //获得审核状态
        public bool IsAudit()
        {
            if (AuditIOS_OnOff == true || AuditMyApp_OnOff == true || AuditCultureMinistry_OnOff == true)
                return true;
            else
                return false;
        }


    }
    public static GameVersion[] GameVersionList;
    //当前版本
    public static GameVersion currentVersion = null;

    public static void SetGameVersion(string[] versionInfoList)
    {
        GameVersionList = new GameVersion[versionInfoList.Length];
        for (int i = 0; i < versionInfoList.Length; i++)
        {
            string str = versionInfoList[i];
            string[] strList = str.Split('&');
            if (strList.Length == 9)
            {
                if (strList[3].Equals("true"))
                {
                    GameVersionList[i] = new GameVersion(strList[0], strList[1], strList[2], true, strList[4], strList[5], strList[6], strList[7], strList[8]);
                }
                else
                {
                    GameVersionList[i] = new GameVersion(strList[0], strList[1], strList[2], false, strList[4], strList[5], strList[6], strList[7], strList[8]);
                }
            }
        }

        for (int i = 0; i < GameVersionList.Length; i++)
        {
            if (GameVersionList[i].netVersion.Equals(ClientVersion) == true && GameVersionList[i].From.Equals(ChannelsNumber) == true)
            {
                currentVersion = GameVersionList[i];
                break;
            }
        }

        if(currentVersion == null)
        {
            Debug.LogError("没有检测到对应的版本");
        }
    }

    //返回当前版本是否需要连接审核版本
    public static bool IsAuditVersion(string[] versionInfoList, out string Ip, out int[] port)
    {
        bool returnvalue = false;
        Ip = "";
        port = null;

        GameVersion[] GameVersionListTemp = new GameVersion[versionInfoList.Length];
        for (int i = 0; i < versionInfoList.Length; i++)
        {
            string str = versionInfoList[i];
            string[] strList = str.Split('&');
            if (strList.Length == 9)
            {
                if (strList[3].Equals("true"))
                {
                    GameVersionListTemp[i] = new GameVersion(strList[0], strList[1], strList[2], true, strList[4], strList[5], strList[6], strList[7], strList[8]);
                }
                else
                {
                    GameVersionListTemp[i] = new GameVersion(strList[0], strList[1], strList[2], false, strList[4], strList[5], strList[6], strList[7], strList[8]);
                }
            }
        }

        for (int i = 0; i < GameVersionListTemp.Length; i++)
        {
            if (GameVersionListTemp[i].netVersion.Equals(ClientVersion) == true && GameVersionListTemp[i].From.Equals(ChannelsNumber) == true)
            {
                //当前版本审核
                returnvalue = true;
                Ip = GameVersionListTemp[i].LinkIp;
                port = GameVersionListTemp[i].LinkPort;
                break;
            }
        }

        return returnvalue;
    }

    //设置审核状态
    public class AuditVersion
    {
        

        //IOS审核开关
        public static bool IsIOSAudit
        {
            get
            {
                if (SystemSetManage.currentVersion != null  && SystemSetManage.currentVersion.AuditIOS_OnOff == true)
                    return true;
                else
                    return false;
            }
        }
        //应用宝审核开关
        public static bool IsMyAppAudit
        {
            get
            {
                if (SystemSetManage.currentVersion != null && SystemSetManage.currentVersion.AuditMyApp_OnOff == true)
                    return true;
                else
                    return false;
            }
        }

		//版署审核开关
		public static bool IsCopyRightAudit
		{
            get
            {
                if (SystemSetManage.currentVersion != null && SystemSetManage.currentVersion.AuditCultureMinistry_OnOff == true)
                    return true;
                else
                    return false;
            }
        }

		/// <summary>
		/// 是否正常运营版本
		/// </summary>
		public static bool IsCommonRunning
		{
			get
			{
				if (SystemSetManage.currentVersion != null &&
                    (SystemSetManage.currentVersion.AuditCultureMinistry_OnOff == true ||
                    SystemSetManage.currentVersion.AuditIOS_OnOff == true ||
                    SystemSetManage.currentVersion.AuditMyApp_OnOff == true
                    ))
					return false;
				else
					return true;
			}
		}
	}
}
