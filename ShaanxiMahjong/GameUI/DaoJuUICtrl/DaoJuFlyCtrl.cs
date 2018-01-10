using UnityEngine;
using System.Collections;

class DaoJuFlyCtrl : UnModalUIBase
{
    /// <summary>
    /// 当道具飞至玩家头像后.
    /// indexVal -> 道具类型。
    /// playerUiSit -> 玩家在UI界面的座位编号.
    /// playerId -> 玩家Id.
    /// </summary>
    public void OnEndDaoJuFly(int[] args)
    {
        int indexVal = args[0];
        int playerUiSit = args[1];
        int playerId = args[2];
        int indexMsg = 43 + playerUiSit - 1;
        GameObject obj = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(indexMsg, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //产生玩家道具+1的消息.
        DaoJuAddUICtrl daoJuAddCom = obj.GetComponent<DaoJuAddUICtrl>();
        daoJuAddCom.InitDaoJuAddUI(indexVal, MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerId));
        DestroyThis();
    }
}