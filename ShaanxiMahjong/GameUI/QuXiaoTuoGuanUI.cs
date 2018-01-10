using UnityEngine;
using System.Collections;

class QuXiaoTuoGuanUI : UnModalUIBase
{
    public void OnQuXuXiaoTuoGuanBtnClick()
    {
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerStartGoldRoomTuoGuan(MainRoot._gPlayerData.nUserId, false);
        }
        DestroyThis();
    }
}
