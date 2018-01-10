using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;

namespace MoleMole
{
    class FaceUserIpContext : BaseContext
    {
        public FaceUserIpContext()
            : base(UIType.UserIpFace)
        {

        }
    }

    class FaceUserIpView : AnimateView
    {
        public ImageBase face;
        public Text username;
        public Text userid;
        public Text userip;
        public override void OnEnter(BaseContext context)
        {
            base.OnEnter(context);
            //测试代码 设置基本信息和ip
            SetUserInfo("Image/shareIcon", "名字","帐号");
            SetUserIp();
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public void BackCallBack()
        {
            Singleton<ContextManager>.Instance.Pop();
        }
        /// <summary>
        /// 设置基本信息
        /// </summary>
        /// <param name="face"></param>
        /// <param name="username"></param>
        /// <param name="userid"></param>
        public void SetUserInfo(string face,string username,string userid)
        {
            this.face.overrideSprite = Resources.Load(face, typeof(Sprite)) as Sprite;
            this.username.text = username;
            this.userid.text = "ID:"+userid;
        }
        /// <summary>
        /// 设置ip
        /// </summary>
        public void SetUserIp()
        {
            this.userip.text = "IP:" + GetAddressIP();
        }
        /// <summary>
        /// <span style="font-family: Arial, Helvetica, sans-serif;">获取局域网的IP</span>
        /// </summary>
        /// <returns></returns>
        public string GetAddressIP()
        {
            string AddressIP = string.Empty;
#if UNITY_IPHONE
        System.Net.NetworkInformation.NetworkInterface[] adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        for (int i = 0; i < adapters.Length; i++)
        {
            if (adapters[i].Supports(System.Net.NetworkInformation.NetworkInterfaceComponent.IPv4))
            {
                System.Net.NetworkInformation.UnicastIPAddressInformationCollection uniCast = adapters[i].GetIPProperties().UnicastAddresses;
                if (uniCast.Count > 0)
                {
                    for (int j = 0; j < uniCast.Count; j++)
                    {
                        //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4
                        if (uniCast[j].Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            AddressIP = uniCast[j].Address.ToString();
                        }
                    }
                }
            }
        }
#endif
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            ///获取本地的IP地址
            for (int i = 0; i < Dns.GetHostEntry(Dns.GetHostName()).AddressList.Length; i++)
            {
                if (Dns.GetHostEntry(Dns.GetHostName()).AddressList[i].AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList[i].ToString();
                }
            }
            //foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            //{
            //    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
            //    {
            //        AddressIP = _IPAddress.ToString();
            //    }
            //}
#endif
            return AddressIP;
        }
    }
}

