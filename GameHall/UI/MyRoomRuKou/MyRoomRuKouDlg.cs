using UnityEngine;
using System.Collections;

class MyRoomRuKouDlg : MonoBehaviour
{
    /// <summary>
    /// 自建比赛Ui列表数据信息.
    /// </summary>
    public object[] ListUiDt;
    /// <summary>
    /// 自建房间数据列表.
    /// </summary>
    public GameObject MyRoomDtListObj;
    /// <summary>
    /// 比赛卡文本信息.
    /// </summary>
    public TextBase BiSaiKaTx;
    /// <summary>
    /// 显示服务端返回的自建比赛入口界面房间列表信息.
    /// </summary>
    public void ShowZiJianFangListInfo(object[] args = null)
    {
        ListUiDt = args;
        bool isHaveListDt = false;
        int listDtCount = 0;
        isHaveListDt = true; //test.
        listDtCount = 2; //test.
        if (!isHaveListDt)
        {
            return;
        }
        VerticalListUICtrl verListUI = GetComponent<VerticalListUICtrl>();
        VerticalListUICtrl.ObjListConfigDt configDt = new VerticalListUICtrl.ObjListConfigDt();
        configDt.CountObj = listDtCount;
        configDt.ObjName = "MyRoomDtUI";
        configDt.ObjPrefab = "Prefab/MyRoomRuKou/" + configDt.ObjName;
        verListUI.CreateObjList(configDt);
    }

    /// <summary>
    /// 点击创建房间按键.
    /// </summary>
    public void OnClickCreatMyRoom()
    {
    }

    /// <summary>
    /// 点击加入比赛按键.
    /// </summary>
    public void OnClickPlayGame()
    {
        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.OnPengYouBtn_Click();
    }
}