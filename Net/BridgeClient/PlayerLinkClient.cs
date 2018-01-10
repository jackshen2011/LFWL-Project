using System;
using System.Collections.Generic;
using System.Text;
using FTLibrary.Net;
using UnityEngine;
using System.Net.Sockets;

namespace RoomCardNet
{
    class PlayerLinkClient : PublicSocketc
    {

        public PlayerLinkClientPlayer m_pBridgeClientPlayerObj = null;
        public PlayerLinkClient()
            :base()
        {

        }
        public override NetClientPlayer AllocNetClientPlayer() { return new PlayerLinkClientPlayer(this); }

        public virtual bool Initialization(string sServerIp, int nServerPort)
        {
            //���þ�������������
            sendBufferSize = 1024 * 256;//256kb
            recvBufferSize = 1024 * 256;//256kb
            sendTimeout = 1500;//��λ�Ǻ���
            recvTimeout = 1500;//��λ�Ǻ���
            //���ö�������
            isReConnect = true;
            //��վ����
            incomingPassword = "RoomCard PlayerLinkServer Group!";
            //����������ߵ�ʱ��
            NetConnectedTimeout = 20.0;
            IsUseEncrypt = true;

            SetUserInfo("", "PlayerLinkPlayer", "0x9372D790", NetVersionDefine.GetCurrentVersionStr);
            //string msg = string.Format("sServerIp={0}  nServerPort={1}", sServerIp, nServerPort);
            //TRACE("player login!");
            UnityEngine.Debug.Log("----------------------------Unity:"+"player login!  IP = " + sServerIp + " Port: " + nServerPort);
            string ss = DateTime.Now.Minute.ToString() + ":";
            ss += DateTime.Now.Second.ToString() + ":";
            ss += DateTime.Now.Millisecond.ToString() + ":";
            UnityEngine.Debug.Log("Unity:" + "timer =======================:" + ss);
            RoomCardNetClientModule.testdebug = 0;
#if UNITY_IPHONE
             AddressFamily ipType;
            string IOS_serverIp = "";
            CompatibilityIP.GetIpType(sServerIp, nServerPort.ToString(), out IOS_serverIp, out ipType);
            return StartClient_IOS(IOS_serverIp, nServerPort, true, ipType);
#else
            return StartClient(sServerIp, nServerPort, true);
#endif
        }
        public override void OnConnect()
        {
            Debug.Log("Unity:" + "net OnConnect!!@@@@@@@@@@@@@@@@@@@@@@@!!!!!!!!!!!!!!!!!!!");
            
        }
        public override void OnConnectFailed()
        {
            //RoomCardNetClientModule.netModule.loginWanLocker = false;
            Debug.Log("Unity:" + "net OnConnectFailed!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            //
            if (MainRoot._gPlayerData.IsExitAbnormalPlayer)
            {
                return;
            }
            if (MainRoot._gUIModule.pUnModalUIControl != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.SpawnSMWangLuoBuWenDing();
            }

        }
        public override void OnDisconnect()
        {
            Debug.Log("Unity:" + "net OnDisconnect!!#####################!!!!!!!!!!!!!!!!!!");
            //RoomCardNetClientModule.netModule.loginWanLocker = false;
            
        }
	}
}
