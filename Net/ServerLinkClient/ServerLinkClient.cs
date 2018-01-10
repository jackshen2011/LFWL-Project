using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Net;
using System.Net.Sockets;

namespace RoomCardNet
{
    public class ServerLinkClient : PublicSocketc
    {

        public ServerLinkClientPlayer m_pServerLinkClientPlayerObj = null;
        public ServerLinkClient()
            :base()
        {

        }
        public override NetClientPlayer AllocNetClientPlayer() { return new ServerLinkClientPlayer(this); }

        public virtual bool Initialization(string sServerIp, int nServerPort)
        {
            //设置局域网服务属性
            sendBufferSize = 1024 * 256;//256kb
            recvBufferSize = 1024 * 256;//256kb
            sendTimeout = 150;//单位是毫秒
            recvTimeout = 150;//单位是毫秒
            //设置断线重连
            isReConnect = true;
            IsUseEncrypt = true;
            //入站密码
            incomingPassword = "RoomCard ServerLinkServer Group!";
            //设置网络断线的时间
            NetConnectedTimeout = 10.0;

            SetUserInfo("", "ServerLinkPlayer", "0x9372D790", NetVersionDefine.GetCurrentVersionStr);
            //string msg = string.Format("sServerIp={0}  nServerPort={1}", sServerIp, nServerPort);
            //TRACE("player login!");

#if UNITY_IPHONE
             AddressFamily ipType;
            string IOS_serverIp = "";
            CompatibilityIP.GetIpType(sServerIp, nServerPort.ToString(), out IOS_serverIp, out ipType);
            return StartClient_IOS(IOS_serverIp, nServerPort, true, ipType);
#else
            return StartClient(sServerIp, nServerPort, true);
#endif

        }
        public override void OnConnect() { }
        public override void OnConnectFailed()
        {
            //RoomCardNetClientModule.netModule.loginWanLocker = false;
        }
        public override void OnDisconnect()
        {
            //RoomCardNetClientModule.netModule.loginWanLocker = false;
        }
    }
}
