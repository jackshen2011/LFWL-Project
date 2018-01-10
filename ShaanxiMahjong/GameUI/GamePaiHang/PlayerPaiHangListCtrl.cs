using UnityEngine;
using System.Collections;

class PlayerPaiHangListCtrl : ShowListUiDtCtrl
{
    /// <summary>
    /// 显示比赛模式排行榜数据信息.
    /// </summary>
    public override void ShowListUiDt(Transform[] objTrAy)
    {
        object[] uiDt = new object[4];
        PlayerPaiHangUI listUiDtCom = null;
        for (int i = 0; i < objTrAy.Length; i++)
        {
            listUiDtCom = objTrAy[i].GetComponent<PlayerPaiHangUI>();
            listUiDtCom.ShowListUiDt(uiDt);
        }
    }
}