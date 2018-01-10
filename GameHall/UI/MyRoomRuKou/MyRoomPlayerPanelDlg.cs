using UnityEngine;
using System.Collections;

class MyRoomPlayerPanelDlg : MonoBehaviour
{
    /// <summary>
    /// 服务器返回自建房玩家列表数据.
    /// </summary>
    public object[] ListUiDt;
    /// <summary>
    /// 纵向列表控制组件.
    /// </summary>
    [HideInInspector]
    public VerticalListUICtrl pVerListUI;
    /// <summary>
    /// 显示服务端返回的自建比赛房间玩家列表数据信息.
    /// </summary>
    public void ShowMyRoomPlayerListInfo(object[] args = null)
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
        pVerListUI = GetComponent<VerticalListUICtrl>();
        VerticalListUICtrl.ObjListConfigDt configDt = new VerticalListUICtrl.ObjListConfigDt();
        configDt.CountObj = listDtCount;
        configDt.ObjName = "MyRoomPlayerDt";
        configDt.ObjPrefab = "Prefab/MyRoomRuKou/" + configDt.ObjName;
        pVerListUI.CreateObjList(configDt);
    }
}