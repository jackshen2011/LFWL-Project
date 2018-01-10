using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FTLibrary.Net;

namespace RoomCardNet
{
    public class AudioClientPlayer : PublicClientPlayer
    {
        public AudioClient m_pParentClientObj = null;
        public AudioClientPlayer(AudioClient parent)
            :base()
        {
            m_pParentClientObj = parent;
            m_pParentClientObj.m_pAudioClientPlayerObj = this;
        }
        public override void OnInitialize()
        {
            Net_CallNetTest();
        }
        public override FuntionRet OnCallNetFuntion(uint nFunId, object[] args)
        {
            try
            {
                switch(nFunId)
                {
                    case 0x93EF2BA://C_AnsNetTest CRC32HashCode
                        {
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

        //¡¨Ω”≤‚ ‘
        public void Net_CallNetTest()
        {
            //case 0xA011D748://S_CallNetTest CRC32HashCode
            Tos(0xA011D748, "test_C");
        }
    }
}
