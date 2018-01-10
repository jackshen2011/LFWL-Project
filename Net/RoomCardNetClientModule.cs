using FTLibrary.Net;
using FTLibrary.Time;
//using FTLibrary.XML;
using System;
using System.IO;
using System.Xml;
using UnityEngine;

namespace RoomCardNet
{
    class RoomCardNetClientModule : UpdateTcpNetModule
    {
        //网络模块句柄
        //public static RoomCardNetClientModule netModule = null;
        //工作路径
        private string workspacePath = Application.dataPath+ "\\";
        const string RoomCardDataCenterXmlFile = "RoomCardClientDefine.xml";

        //网络刷新时间
        private long netUpdateTimes = 0;

        //网络刷新上一次的刷新时间,单位是毫秒
        private long networkUpdateTimes = 0;
        //本地网络状态，刷新周期
        const long NETWORKVALID_UPDATETIME = 5000;
        private long networkValid_PrveUpdateTimes = 0;
        //本地网络是否可用,这里使用变量缓冲，而不应该直接获取，直接获取会比较耗费
        private bool m_IsNetworkValid = false;
        public bool isNetworkValid { get { return m_IsNetworkValid; } }

        //是否打开报错输出
        public bool isExportErr { get; set; }
        //是否打开跟踪输出
        public bool isExportTrace { get; set; }


        //登录过程锁
        private bool loginWanLocker = false;
        private TimeLocker loginWanErrLocker = new TimeLocker(5000);
        //现在这个仅作为刷新周期使用
        private System.Timers.Timer m_NetworkCycleTimer = null;
        private double m_dNetworkCycleTime = 0.033;//33毫秒
        public double networkCycleTime
        {
            set { m_dNetworkCycleTime = value; if (m_NetworkCycleTimer != null) { m_NetworkCycleTimer.Interval = m_dNetworkCycleTime * 1000; } }
            get { return m_dNetworkCycleTime; }
        }


        //private ServerAddress[] m_PlayerLinkServerAddress = null;
        private PlayerLinkClient m_PlayerLinkClient = null;
        //当前游戏连接的服务器索引
        private int m_LinkServerIndex = 0;

        public PlayerLinkClient playerLinkClient { get { return m_PlayerLinkClient; } }
        public bool isWanMainClientConnect { get { return m_PlayerLinkClient == null ? false : m_PlayerLinkClient.IsConnected; } }
        public PlayerLinkClientPlayer wanMainClientPlayer { get { return m_PlayerLinkClient == null ? null : (PlayerLinkClientPlayer)m_PlayerLinkClient.player; } }

        /// <summary>
        /// //连接指定服务器
        /// </summary>
        /// 
        private ServerAddress m_serverLinkerverAddress = null;
        private ServerLinkClient m_serverLinkClient = null;
        public ServerLinkClient serverLinkClient { get { return m_serverLinkClient; } }
        public ServerLinkClientPlayer serverLinkClientPlayer { get { return m_serverLinkClient == null ? null : (ServerLinkClientPlayer)m_PlayerLinkClient.player; } }


        public double wanMainClientTime { get { return m_PlayerLinkClient == null ? 0.0 : m_PlayerLinkClient.time; } }

        public static RoomCardNetClientModule netModule;
        public RoomCardNetClientModule()
           : base()
        {
        }

       public static void CreateRoomCardNetClientModule()
        {
            if (RoomCardNetClientModule.netModule == null)
            {
                RoomCardNetClientModule.netModule = new RoomCardNetClientModule();
                RoomCardNetClientModule.netModule.Initialization();
            }
        }


        public static int testdebug = 0;
        //测试登录

        public virtual PlayerLinkClient CreateBridgeClient()
        {
            return new PlayerLinkClient();
        }
        public virtual ServerLinkClient CreateServerLinkClient()
        {
            return new ServerLinkClient();
        }

        //关闭网络
        public void ClosePlayerLinkClient()
        {
            m_PlayerLinkClient.CloseClient();
            m_PlayerLinkClient = null;
        }

        public void CloseServerLinkClient()
        {
            m_serverLinkClient.CloseClient();
            m_serverLinkClient = null;
        }

        //初始化整个网络模块
        public virtual void Initialization()
        {
            //检测一次本地网络状态
            if (LoadXmlFile() == false)
                return;

            m_IsNetworkValid = (FTNetInterface.NetWorkStatus == FTNetWorkStatus.Status_Up);

            ConnectServerLinkServer();

            //设置刷新周期
            //networkCycleTime = 0.033;//33毫秒
            //m_NetworkCycleTimer = new System.Timers.Timer();
            //m_NetworkCycleTimer.Elapsed += new System.Timers.ElapsedEventHandler(NetworkUpdate);
            //m_NetworkCycleTimer.Interval = networkCycleTime * 1000;
            //m_NetworkCycleTimer.AutoReset = false;
            //m_NetworkCycleTimer.Enabled = true;
        }

        //读取xml配置
        public virtual bool LoadXmlFile()
        {
            try
            {
                //读取当前配置信息
                //string xmlFilePath = "Assets\\" + RoomCardDataCenterXmlFile;

                // Debug.Log("Unity:"+xmlFilePath);
                string xmlFilePath = RoomCardDataCenterXmlFile;
                Debug.Log("Unity:"+RoomCardDataCenterXmlFile);
                //XmlReader xmlReader = new XmlReader();
                //XmlDocument doc = xmlReader.LoadXml(xmlFilePath);

                XmlDocument doc = new XmlDocument();

                TextAsset test = (TextAsset)Resources.Load(xmlFilePath);
                //Debug.LogError("Unity: zhl-----" + test.ToString());
                byte[] xmldata = test.bytes;
                MemoryStream reader = new MemoryStream(xmldata);
                //XmlReader reader = XmlReader.Create(xmlFilePath, set);
                doc.Load(reader);

                if (doc == null)
                {
                    Debug.Log("Unity:"+(string.Format("Load Xml File Err!{0}", xmlFilePath)));
                    return false;
                }

                XmlNode root = doc.SelectSingleNode("RoomCardClientDefine");

                XmlNode node = root.SelectSingleNode("ServerLinkClient");
                XmlElement xe_1 = (XmlElement)node;
                m_serverLinkerverAddress = new ServerAddress(
                                                             Convert.ToString(xe_1.GetAttribute("ip").ToString()),
                                                             Convert.ToInt32(xe_1.GetAttribute("port").ToString()));

                //node = root.SelectSingleNode("RoomCardClient");

                //node = node.SelectSingleNode("bridgeClient");
                //XmlNodeList list = node.SelectNodes("bridgeClient");

                //XmlElement xe = (XmlElement)i;
                //int n = Convert.ToInt32(xe.GetAttribute("nIndex"));
                //m_PlayerLinkServerAddress = new ServerAddress[list.Count];
                //for (int i = 0; i < m_PlayerLinkServerAddress.Length; i++)
                //{
                //    XmlElement xe = (XmlElement)list[i];
                //    m_PlayerLinkServerAddress[i] = new ServerAddress(
                //                                              Convert.ToString(xe.GetAttribute("ip").ToString()),
                //                                              Convert.ToInt32(xe.GetAttribute("port").ToString()));
                //}

//#if !_ROOMCARDDEBUG
//                System.Random rand = new System.Random();
//                m_LinkServerIndex = rand.Next(0, m_PlayerLinkServerAddress.Length-1);
//#endif


            }
            catch (System.Exception ex)
            {
               Debug.LogError("Unity:"+ex.Message);
               Debug.LogError("Unity:"+RoomCardDataCenterXmlFile);
               Debug.LogError("Unity:"+"Load Xml File Err!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return false;
            }
            return true;
        }

        //public bool ConnectWanMainServer()
        //{
        //    //断开登录服务
        //    if (m_PlayerLinkClient != null)
        //    {
        //        m_PlayerLinkClient.CloseClient();
        //        m_PlayerLinkClient = null;
        //    }
        //    //解除锁定
        //    loginWanLocker = false;
        //    if (m_PlayerLinkServerAddress == null)
        //        return false;
        //    if (m_PlayerLinkClient == null)
        //    {
        //        m_PlayerLinkClient = CreateBridgeClient();
        //    }
        //    if (!m_PlayerLinkClient.Initialization(m_PlayerLinkServerAddress[m_LinkServerIndex].IP, m_PlayerLinkServerAddress[m_LinkServerIndex].Port))
        //    {
        //        TcpNetModule.ERR("Initialization WanMainClient Err!");
        //        return false;
        //    }

        //    PlayerLinkClientPlayer.m_IsPlayerLogin = false;
        //    //锁定等待连接，如果不锁定，很可能由会启动一次登录
        //    //这个锁永远加上，也就是说以后都不用再登录了。
        //    loginWanLocker = true;
        //    return true;
        //}

        //连接服务器控制服务， 取当前应该连接的服务器地址， 用于版本审核的时候连接指定版本
        public bool ConnectServerLinkServer()
        {
            //断开登录服务
            if (m_serverLinkClient != null)
            {
                m_serverLinkClient.CloseClient();
                m_serverLinkClient = null;
            }
            //解除锁定
            if (m_serverLinkerverAddress == null)
                return false;
            if (m_serverLinkClient == null)
            {
                m_serverLinkClient = CreateServerLinkClient();
            }
            if (!m_serverLinkClient.Initialization(m_serverLinkerverAddress.IP, m_serverLinkerverAddress.Port))
            {
                TcpNetModule.ERR("Initialization WanMainClient Err!");
                return false;
            }

            return true;
        }

        public bool ConnectWanMainServer_Audit(string ip, int port)
        {
            //断开登录服务
            if (m_PlayerLinkClient != null)
            {
                m_PlayerLinkClient.CloseClient();
                m_PlayerLinkClient = null;
            }
            //解除锁定
            loginWanLocker = false;
            if (m_PlayerLinkClient == null)
            {
                m_PlayerLinkClient = CreateBridgeClient();
            }
            if (!m_PlayerLinkClient.Initialization(ip, port))
            {
                TcpNetModule.ERR("Initialization WanMainClient Err!");
                return false;
            }
            //锁定等待连接，如果不锁定，很可能由会启动一次登录
            //这个锁永远加上，也就是说以后都不用再登录了。
            loginWanLocker = true;
            return true;
        }

        public void NetworkUpdate()
        {
           try
            {
                //计算新时间
                networkUpdateTimes = DateTime.Now.Ticks / 10000;
                if ((networkUpdateTimes - netUpdateTimes) <= 33)
                    return;
                netUpdateTimes = networkUpdateTimes;
                //刷新本地网络状态
                if ((networkUpdateTimes - networkValid_PrveUpdateTimes) >= RoomCardNetClientModule.NETWORKVALID_UPDATETIME)
                {
                    networkValid_PrveUpdateTimes = networkUpdateTimes;
                    m_IsNetworkValid = (FTNetInterface.NetWorkStatus == FTNetWorkStatus.Status_Up);
                }

                if(isWanMainClientConnect == false)
                {
                    PlayerLinkClientPlayer.m_IsPlayerLogin = false;
                }

                if(serverLinkClient != null)
                {
                    serverLinkClient.NetworkUpdate();
                }

                if (playerLinkClient != null)
                {
                    playerLinkClient.NetworkUpdate();
                    RoomCardNetClientModule.testdebug += 1;
                    //if(wanMainClientPlayer != null && wanMainClientPlayer.m_testModule == null)
                    //{
                    //    wanMainClientPlayer.m_testModule = this;
                    //}
                }
            }
            catch (System.Exception ex)
            {
                ERR(ex.ToString());
            }
           
            //m_NetworkCycleTimer.Enabled = true;
        }
        //释放网络模块
        public virtual void Release()
        {
          
        }
    }
}
