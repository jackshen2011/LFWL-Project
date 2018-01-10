using UnityEngine;
using System.Collections;

class MyRoomPlayerDtListCtrl : ShowListUiDtCtrl
{
    /// <summary>
    /// 显示自建比赛玩家数据信息.
    /// </summary>
    public override void ShowListUiDt(Transform[] objTrAy)
    {
        object[] uiDt = new object[4];
        object[] listUiDt = MainRoot._gUIModule.pUnModalUIControl.pMyRoomPlayerPanelDlg.ListUiDt;
        MyRoomPlayerDtUI listUiDtCom = null;
        bool isHiddenTiChuBt = MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null ? true : false;
        for (int i = 0; i < objTrAy.Length; i++)
        {
            //uiDt[0] = i + 1;
            //uiDt[1] = listUiDt[(i * 3) + 4];
            //uiDt[2] = listUiDt[(i * 3) + 5];
            //uiDt[3] = listUiDt[3];
            //uiDt[4] = isHiddenTiChuBt == false ? 0 : 1;
            listUiDtCom = objTrAy[i].GetComponent<MyRoomPlayerDtUI>();
            listUiDtCom.ShowListUiDt(uiDt);
        }
    }
}