using UnityEngine;
using System.Collections;

class QinRenBiSaiPaiHangListCtrl : ShowListUiDtCtrl
{
    /// <summary>
    /// 显示秦人比赛场玩家排行数据信息列表.
    /// </summary>
    public override void ShowListUiDt(Transform[] objTrAy)
    {
        object[] uiDt = new object[5];
        //object[] listDt = MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg.QinRenBiSaiChangDt;
        QinRenBiSaiPaiHangUI listUiDtCom = null;
        for (int i = 0; i < objTrAy.Length; i++)
        {
            listUiDtCom = objTrAy[i].GetComponent<QinRenBiSaiPaiHangUI>();
            listUiDtCom.ShowListUiDt(uiDt);
        }
    }
}