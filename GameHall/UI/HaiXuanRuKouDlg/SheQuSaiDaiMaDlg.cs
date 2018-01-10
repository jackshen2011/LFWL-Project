using UnityEngine;
using System.Collections;
using RoomCardNet;
using System.Collections.Generic;

/// <summary>
/// 社区赛代码验证对话框.
/// </summary>
class SheQuSaiDaiMaDlg : MonoBehaviour
{
    List<int> mSheQuDaiMaList = new List<int>() { 8080,
        9563,
        6289,
        5120,
        9280,
        6399,
        9860,
        5210,
        3698,
        2586
    };
    EnsureDlg SheQuSaiDlg;

    public void Init()
    {
        SheQuSaiDlg = GetComponent<EnsureDlg>();
    }

    /// <summary>
    /// 点击确认按键.
    /// </summary>
    public void OnClickQueRenBt()
    {
        string yanZhengMaVal = SheQuSaiDlg.p_input1.text;
        if (yanZhengMaVal == "")
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnYanZhengMaCuoWuMsg();
            return;
        }
        //Debug.Log("Unity: OnClickQueRenBt -> send yanZhengMa to server port! yanZhengMa is " + yanZhengMaVal);
        int result = System.Convert.ToInt32(yanZhengMaVal);
        if (!mSheQuDaiMaList.Contains(result))
        {
            //社区代码错误.
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSheQuDaiMaCuoWuMsg();
            return;
        }
        //社区代码正确.
        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSheQuDaiMaChengGongMsg();

        //此处向服务端发送社区赛代码正确信息.
        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallSetUserSheQuCode(yanZhengMaVal);
            MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.IsOpenSheQuSaiDaiMaYanZheng = false;
        }
        SheQuSaiDlg.DestroyThis();
    }
}