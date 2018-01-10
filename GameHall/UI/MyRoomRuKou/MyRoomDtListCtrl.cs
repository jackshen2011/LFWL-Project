using UnityEngine;
using System.Collections;

class MyRoomDtListCtrl : ShowListUiDtCtrl
{
    /// <summary>
    /// 显示自建比赛排行榜数据信息.
    /// </summary>
    public override void ShowListUiDt(Transform[] objTrAy)
    {
        object[] uiDt = new object[6];
        object[] listUiDt = MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pMyRoomRuKouDlg.ListUiDt;
        MyRoomDtUI listUiDtCom = null;
        for (int i = 0; i < objTrAy.Length; i++)
        {
            //uiDt[0] = i + 1;
            //uiDt[1] = listUiDt[(i * 3) + 4];
            //uiDt[2] = listUiDt[(i * 3) + 5];
            //uiDt[3] = listUiDt[(i * 3) + 5];
            //uiDt[4] = listUiDt[(i * 3) + 5];
            //uiDt[5] = listUiDt[(i * 3) + 5];
            listUiDtCom = objTrAy[i].GetComponent<MyRoomDtUI>();
            listUiDtCom.ShowListUiDt(uiDt);
        }
    }
}