using UnityEngine;
using System.Collections;

class QinRenBiSaiChangListCtrl : ShowListUiDtCtrl
{
    /// <summary>
    /// 显示秦人比赛场(红包赛)数据信息列表.
    /// </summary>
    public override void ShowListUiDt(Transform[] objTrAy)
    {
        object[] uiDt = new object[9];
        object[] listDt = MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg.QinRenBiSaiChangDt;
        QinRenBiSaiChangUI listUiDtCom = null;
        int startIndex = 4;
        int valUnit = 10;
        for (int i = 0; i < objTrAy.Length; i++)
        {
            uiDt[0] = listDt[(i * valUnit) + startIndex + 1];
            uiDt[1] = listDt[(i * valUnit) + startIndex + 3];
            uiDt[2] = listDt[(i * valUnit) + startIndex + 4];
            uiDt[3] = listDt[(i * valUnit) + startIndex + 6];
            uiDt[4] = listDt[(i * valUnit) + startIndex + 5];
            uiDt[5] = listDt[(i * valUnit) + startIndex + 8];
            uiDt[6] = listDt[(i * valUnit) + startIndex + 7];
            uiDt[7] = listDt[(i * valUnit) + startIndex];
            uiDt[8] = listDt[(i * valUnit) + startIndex + 9];
            listUiDtCom = objTrAy[i].GetComponent<QinRenBiSaiChangUI>();
            listUiDtCom.ShowListUiDt(uiDt);
        }
    }
}