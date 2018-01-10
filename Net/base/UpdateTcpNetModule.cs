using System;
using FTLibrary.Net;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

namespace RoomCardNet
{
    public class UpdateTcpNetModule : TcpNetModule
    {
        public override void PrintTRACE(string s)
        {
            Debug.Log("Unity:" + s);
        }
    }
}
