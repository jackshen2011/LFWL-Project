using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FTLibrary.Net;

namespace RoomCardNet
{
    public class ServerLinkClientPlayer : PublicClientPlayer
    {
        public ServerLinkClient m_pParentClientObj = null;
        public ServerLinkClientPlayer(ServerLinkClient parent)
            :base()
        {
            m_pParentClientObj = parent;
            m_pParentClientObj.m_pServerLinkClientPlayerObj = this;
        }
        public override void OnInitialize()
        {
           
        }
        public override FuntionRet OnCallNetFuntion(uint nFunId, object[] args)
        {
            try
            {
                switch(nFunId)
                {
                    case 0x93EF2BA://C_AnsNetTest CRC32HashCode
                        {
                            Net_CallNetTest(args);
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
        public void Net_CallNetTest(object[] args)
        {
            //判断当前客户端是那一版， 如果当前版本是审核状态 则连指定IP的服务
            string[] list = new string[args.Length];
            for(int i=0; i< list.Length; i++)
            {
                list[i] = (string)args[i];
            }
            string ip = "";
            int[] portlist = null; 
             bool isAudit = SystemSetManage.IsAuditVersion(list, out ip, out portlist);


            if(portlist != null)
            {
                System.Random rand = new System.Random();
                int portIndex = rand.Next(0, portlist.Length - 1);
                RoomCardNetClientModule.netModule.ConnectWanMainServer_Audit(ip, portlist[portIndex]);
            }
            else
            {
                //RoomCardNetClientModule.netModule.ConnectWanMainServer();
            }
            RoomCardNetClientModule.netModule.CloseServerLinkClient();
        }
    }
}
