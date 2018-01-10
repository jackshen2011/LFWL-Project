using UnityEngine;
using System.Collections;

class HaiXuanPaiHangListCtrl : ShowListUiDtCtrl
{
    /// <summary>
    /// 显示海选赛排行榜数据信息.
    /// </summary>
    public override void ShowListUiDt(Transform[] objTrAy)
    {
        object[] uiDt = new object[4];
        object[] haiXuanDt = MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.HaiXuanPaiHangDt;
        HaiXuanPaiHangUI listUiDtCom = null;
        for (int i = 0; i < objTrAy.Length; i++)
        {
            uiDt[0] = i + 1;
            uiDt[1] = haiXuanDt[(i * 3) + 4];
            uiDt[2] = haiXuanDt[(i * 3) + 5];
            if ((int)haiXuanDt[(i * 3) + 3] == MainRoot._gPlayerData.nUserId && MainRoot._gPlayerData.nUserId != 0)
            {
                uiDt[3] = 1;
            }
            else
            {
                uiDt[3] = 0;
            }
            listUiDtCom = objTrAy[i].GetComponent<HaiXuanPaiHangUI>();
            listUiDtCom.ShowListUiDt(uiDt);

            if ((int)haiXuanDt[(i * 3) + 3] == MainRoot._gPlayerData.nUserId)
            {
                listUiDtCom.ChangeZhanJiTextType();
            }
        }
    }
}